using AutoMapper;
using SchoolERP.Application.Common.Exceptions;
using SchoolERP.Application.Common.Helpers;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Common.Interfaces.Services;
using SchoolERP.Application.Common.Models;
using SchoolERP.Application.Features.ExamResult.DTOs;
using SchoolERP.Application.Features.ExamResult.Interfaces;
using SchoolERP.Application.Features.Result.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Services;

/// <summary>
/// Business logic for exam result calculation, ranking, publishing and
/// reporting. Aggregates Result (mark entry) rows into one
/// <see cref="ExamResult"/> per student per exam, computes Class/Section/
/// Merit positions, and builds every report/dashboard view on top of that
/// data. Orchestrates <see cref="IResultService"/> to lock/unlock the
/// underlying mark entries when results are published/unpublished.
/// </summary>
public class ExamResultService : IExamResultService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IResultService _resultService;
    private readonly ICurrentUserService _currentUserService;

    public ExamResultService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IResultService resultService,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _resultService = resultService;
        _currentUserService = currentUserService;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ExamResultDto>> CalculateAsync(int examId, CancellationToken cancellationToken = default)
    {
        var exam = await _unitOfWork.ExamRepository.GetByIdAsync(examId, cancellationToken)
            ?? throw new NotFoundException(nameof(Exam), examId);

        var marks = await _unitOfWork.ResultRepository.GetByExamAsync(examId, cancellationToken);

        if (marks.Count == 0)
        {
            throw new BadRequestException("No mark entries exist for this exam yet. Enter and submit marks before calculating results.");
        }

        var existingResults = await _unitOfWork.ExamResultRepository.GetByExamAsync(examId, cancellationToken);
        var existingByStudent = existingResults.ToDictionary(x => x.StudentId);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            foreach (var studentGroup in marks.GroupBy(x => x.StudentId))
            {
                var entries = studentGroup.ToList();

                var totalMarks = entries.Sum(x => x.MarksObtained + x.GraceMarks);
                var totalFullMarks = entries.Sum(x => x.ExamSchedule!.FullMarks);
                var percentage = totalFullMarks == 0 ? 0 : Math.Round(totalMarks / totalFullMarks * 100m, 2);
                var averageGradePoint = entries.Average(x => x.GPA ?? 0);
                var (grade, _) = GradeCalculator.FromAverageGradePoint(Math.Round(averageGradePoint, 2));
                var isPassed = entries.All(x => x.IsPassed);

                if (existingByStudent.TryGetValue(studentGroup.Key, out var existing))
                {
                    var tracked = await _unitOfWork.ExamResultRepository.GetByIdTrackedAsync(existing.Id, cancellationToken);
                    if (tracked is not null)
                    {
                        if (tracked.IsPublished)
                        {
                            throw new BadRequestException("This exam's results are already published and cannot be recalculated. Unpublish first.");
                        }

                        tracked.TotalMarks = totalMarks;
                        tracked.TotalFullMarks = totalFullMarks;
                        tracked.Percentage = percentage;
                        tracked.GPA = Math.Round(averageGradePoint, 2);
                        tracked.Grade = grade;
                        tracked.IsPassed = isPassed;

                        _unitOfWork.ExamResultRepository.Update(tracked);
                    }
                }
                else
                {
                    var newResult = new ExamResult
                    {
                        StudentId = studentGroup.Key,
                        ExamId = examId,
                        TotalMarks = totalMarks,
                        TotalFullMarks = totalFullMarks,
                        Percentage = percentage,
                        GPA = Math.Round(averageGradePoint, 2),
                        Grade = grade,
                        IsPassed = isPassed
                    };

                    await _unitOfWork.ExamResultRepository.AddAsync(newResult, cancellationToken);
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await RecalculateRankingsAsync(examId, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }

        return await GetByExamAsync(examId, null, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ExamResultDto>> PublishAsync(int examId, CancellationToken cancellationToken = default)
    {
        var results = await _unitOfWork.ExamResultRepository.GetByExamAsync(examId, cancellationToken);

        if (results.Count == 0)
        {
            throw new BadRequestException("Results have not been calculated for this exam yet. Calculate before publishing.");
        }

        if (results.Any(x => x.IsPublished))
        {
            throw new BadRequestException("This exam's results are already published.");
        }

        var now = DateTime.UtcNow;
        var publishedBy = _currentUserService.UserId;

        foreach (var result in results)
        {
            var tracked = await _unitOfWork.ExamResultRepository.GetByIdTrackedAsync(result.Id, cancellationToken);
            if (tracked is null)
                continue;

            tracked.IsPublished = true;
            tracked.PublishedAt = now;
            tracked.PublishedBy = publishedBy;

            _unitOfWork.ExamResultRepository.Update(tracked);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Lock the underlying mark entries for every schedule of this exam.
        var schedules = await _unitOfWork.ExamScheduleRepository.GetSchedulesByExamAsync(examId, cancellationToken);
        foreach (var schedule in schedules)
        {
            await _resultService.LockByExamScheduleAsync(schedule.Id, cancellationToken);
        }

        return await GetByExamAsync(examId, null, cancellationToken);
    }

    /// <inheritdoc />
    public async Task UnpublishAsync(int examId, CancellationToken cancellationToken = default)
    {
        var results = await _unitOfWork.ExamResultRepository.GetByExamAsync(examId, cancellationToken);

        if (results.Count == 0)
        {
            throw new NotFoundException(nameof(ExamResult), examId);
        }

        foreach (var result in results)
        {
            var tracked = await _unitOfWork.ExamResultRepository.GetByIdTrackedAsync(result.Id, cancellationToken);
            if (tracked is null)
                continue;

            tracked.IsPublished = false;
            tracked.PublishedAt = null;
            tracked.PublishedBy = null;

            _unitOfWork.ExamResultRepository.Update(tracked);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var schedules = await _unitOfWork.ExamScheduleRepository.GetSchedulesByExamAsync(examId, cancellationToken);
        foreach (var schedule in schedules)
        {
            await _resultService.UnlockByExamScheduleAsync(schedule.Id, cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task<StudentExamResultDto> GetStudentResultAsync(int studentId, int examId, CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.ExamResultRepository.GetByStudentAndExamAsync(studentId, examId, cancellationToken)
            ?? throw new NotFoundException(nameof(ExamResult), $"student {studentId}, exam {examId}");

        var marks = await _unitOfWork.ResultRepository.GetByStudentAndExamAsync(studentId, examId, cancellationToken);

        var subjects = marks.Select(x => new ExamResultDetailDto
        {
            SubjectId = x.ExamSchedule!.SubjectId,
            SubjectName = x.ExamSchedule.Subject?.Name ?? string.Empty,
            MarksObtained = x.MarksObtained,
            GraceMarks = x.GraceMarks,
            FullMarks = x.ExamSchedule.FullMarks,
            PassMarks = x.ExamSchedule.PassMarks,
            Grade = x.Grade ?? string.Empty,
            GPA = x.GPA ?? 0,
            IsPassed = x.IsPassed
        }).ToList();

        return new StudentExamResultDto
        {
            Summary = _mapper.Map<ExamResultDto>(result),
            Subjects = subjects
        };
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ExamResultDto>> GetByExamAsync(int examId, int? classId, CancellationToken cancellationToken = default)
    {
        var entities = classId.HasValue
            ? await _unitOfWork.ExamResultRepository.GetByExamAndClassAsync(examId, classId.Value, cancellationToken)
            : await _unitOfWork.ExamResultRepository.GetByExamAsync(examId, cancellationToken);

        return _mapper.Map<IReadOnlyList<ExamResultDto>>(entities);
    }

    /// <inheritdoc />
    public async Task<TabulationSheetDto> GetTabulationSheetAsync(int examId, int classId, CancellationToken cancellationToken = default)
    {
        var exam = await _unitOfWork.ExamRepository.GetByIdAsync(examId, cancellationToken)
            ?? throw new NotFoundException(nameof(Exam), examId);

        var schoolClass = await _unitOfWork.SchoolClassRepository.GetByIdAsync(classId, cancellationToken)
            ?? throw new NotFoundException(nameof(SchoolClass), classId);

        var results = await _unitOfWork.ExamResultRepository.GetByExamAndClassAsync(examId, classId, cancellationToken);
        var marks = await _unitOfWork.ResultRepository.GetByClassAndExamAsync(classId, examId, cancellationToken);

        var subjectNames = marks
            .Select(x => x.ExamSchedule!.Subject?.Name ?? string.Empty)
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        var marksByStudent = marks.GroupBy(x => x.StudentId).ToDictionary(g => g.Key, g => g.ToList());

        var rows = results.Select(result =>
        {
            var studentMarks = marksByStudent.TryGetValue(result.StudentId, out var list) ? list : new List<Domain.Entities.Result>();

            var subjectMarks = studentMarks.ToDictionary(
                x => x.ExamSchedule!.Subject?.Name ?? string.Empty,
                x => x.MarksObtained + x.GraceMarks);

            return new TabulationRowDto
            {
                StudentId = result.StudentId,
                StudentName = result.Student?.FullName ?? string.Empty,
                RollNo = result.Student?.RollNo ?? string.Empty,
                SubjectMarks = subjectMarks,
                TotalMarks = result.TotalMarks,
                GPA = result.GPA,
                Grade = result.Grade,
                IsPassed = result.IsPassed,
                ClassPosition = result.ClassPosition
            };
        })
        .OrderBy(x => x.ClassPosition ?? int.MaxValue)
        .ToList();

        return new TabulationSheetDto
        {
            ExamId = exam.Id,
            ExamName = exam.Name,
            ClassId = schoolClass.Id,
            ClassName = schoolClass.Name,
            SubjectNames = subjectNames,
            Rows = rows
        };
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<MeritEntryDto>> GetClassMeritListAsync(int examId, int classId, CancellationToken cancellationToken = default)
    {
        var results = await _unitOfWork.ExamResultRepository.GetByExamAndClassAsync(examId, classId, cancellationToken);
        return BuildMeritList(results, x => x.ClassPosition);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<MeritEntryDto>> GetSectionMeritListAsync(int examId, int sectionId, CancellationToken cancellationToken = default)
    {
        var results = await _unitOfWork.ExamResultRepository.GetByExamAndSectionAsync(examId, sectionId, cancellationToken);
        return BuildMeritList(results, x => x.SectionPosition);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<MeritEntryDto>> GetFailedStudentsAsync(int examId, int? classId, CancellationToken cancellationToken = default)
    {
        var results = classId.HasValue
            ? await _unitOfWork.ExamResultRepository.GetByExamAndClassAsync(examId, classId.Value, cancellationToken)
            : await _unitOfWork.ExamResultRepository.GetByExamAsync(examId, cancellationToken);

        return BuildMeritList(results.Where(x => !x.IsPassed).ToList(), x => x.MeritPosition);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<MeritEntryDto>> GetTopStudentsAsync(int examId, int? classId, int count, CancellationToken cancellationToken = default)
    {
        var results = classId.HasValue
            ? await _unitOfWork.ExamResultRepository.GetByExamAndClassAsync(examId, classId.Value, cancellationToken)
            : await _unitOfWork.ExamResultRepository.GetByExamAsync(examId, cancellationToken);

        var top = results
            .Where(x => x.IsPassed)
            .OrderByDescending(x => x.GPA)
            .ThenByDescending(x => x.Percentage)
            .Take(count)
            .ToList();

        return BuildMeritList(top, x => x.MeritPosition);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<SubjectStatisticsDto>> GetSubjectStatisticsAsync(int examId, CancellationToken cancellationToken = default)
    {
        var marks = await _unitOfWork.ResultRepository.GetByExamAsync(examId, cancellationToken);

        return marks
            .GroupBy(x => new { x.ExamSchedule!.SubjectId, SubjectName = x.ExamSchedule!.Subject?.Name ?? string.Empty })
            .Select(g =>
            {
                var totals = g.Select(x => x.MarksObtained + x.GraceMarks).ToList();
                var passCount = g.Count(x => x.IsPassed);
                var failCount = g.Count() - passCount;

                return new SubjectStatisticsDto
                {
                    SubjectId = g.Key.SubjectId,
                    SubjectName = g.Key.SubjectName,
                    TotalStudents = g.Count(),
                    HighestMarks = totals.Max(),
                    LowestMarks = totals.Min(),
                    AverageMarks = Math.Round(totals.Average(), 2),
                    PassCount = passCount,
                    FailCount = failCount,
                    PassRate = g.Count() == 0 ? 0 : Math.Round(passCount * 100m / g.Count(), 2)
                };
            })
            .OrderBy(x => x.SubjectName)
            .ToList();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<GradeDistributionItemDto>> GetGradeDistributionAsync(int examId, CancellationToken cancellationToken = default)
    {
        var results = await _unitOfWork.ExamResultRepository.GetByExamAsync(examId, cancellationToken);
        var total = results.Count;

        return results
            .GroupBy(x => x.Grade)
            .Select(g => new GradeDistributionItemDto
            {
                Grade = g.Key,
                Count = g.Count(),
                Percentage = total == 0 ? 0 : Math.Round(g.Count() * 100m / total, 2)
            })
            .OrderByDescending(x => x.Count)
            .ToList();
    }

    /// <inheritdoc />
    public async Task<ExamResultDashboardDto> GetDashboardAsync(int examId, CancellationToken cancellationToken = default)
    {
        var exam = await _unitOfWork.ExamRepository.GetByIdAsync(examId, cancellationToken)
            ?? throw new NotFoundException(nameof(Exam), examId);

        var schedules = await _unitOfWork.ExamScheduleRepository.GetSchedulesByExamAsync(examId, cancellationToken);
        var marks = await _unitOfWork.ResultRepository.GetByExamAsync(examId, cancellationToken);
        var results = await _unitOfWork.ExamResultRepository.GetByExamAsync(examId, cancellationToken);

        var classIds = schedules.Select(x => x.ClassId).Distinct().ToList();
        var allStudents = await _unitOfWork.StudentRepository.GetAllAsync(cancellationToken);
        var totalStudents = allStudents.Count(s => classIds.Contains(s.ClassId));

        var appearedStudents = marks
            .Where(x => x.AttendanceStatus == Domain.Enums.MarkAttendanceStatus.Present)
            .Select(x => x.StudentId)
            .Distinct()
            .Count();

        var fullySubmittedCount = 0;
        foreach (var schedule in schedules)
        {
            var scheduleMarks = marks.Where(x => x.ExamScheduleId == schedule.Id).ToList();
            if (scheduleMarks.Count > 0 && scheduleMarks.All(x => x.EntryStatus == Domain.Enums.MarkEntryStatus.Submitted))
            {
                fullySubmittedCount++;
            }
        }

        var subjectStatistics = await GetSubjectStatisticsAsync(examId, cancellationToken);

        return new ExamResultDashboardDto
        {
            ExamId = exam.Id,
            ExamName = exam.Name,
            TotalStudents = totalStudents,
            AppearedStudents = appearedStudents,
            AbsentStudents = Math.Max(0, totalStudents - appearedStudents),
            TotalScheduleCount = schedules.Count,
            FullySubmittedScheduleCount = fullySubmittedCount,
            CompletionPercentage = schedules.Count == 0 ? 0 : Math.Round(fullySubmittedCount * 100m / schedules.Count, 2),
            IsResultPublished = results.Any(x => x.IsPublished),
            PublishedResultCount = results.Count(x => x.IsPublished),
            PendingResultCount = results.Count(x => !x.IsPublished),
            SubjectStatistics = subjectStatistics
        };
    }

    /// <summary>
    /// Recomputes Merit (exam-wide), Class and Section positions for every
    /// ExamResult of an exam, ranking by GPA desc then Percentage desc.
    /// Only passed students are ranked; failed students get a null position.
    /// </summary>
    private async Task RecalculateRankingsAsync(int examId, CancellationToken cancellationToken)
    {
        var results = await _unitOfWork.ExamResultRepository.GetByExamAsync(examId, cancellationToken);

        await AssignPositionsAsync(results.Where(x => x.IsPassed), cancellationToken, (r, pos) => r.MeritPosition = pos);

        foreach (var classGroup in results.GroupBy(x => x.Student?.ClassId))
        {
            await AssignPositionsAsync(classGroup.Where(x => x.IsPassed), cancellationToken, (r, pos) => r.ClassPosition = pos);
        }

        foreach (var sectionGroup in results.GroupBy(x => x.Student?.SectionId))
        {
            await AssignPositionsAsync(sectionGroup.Where(x => x.IsPassed), cancellationToken, (r, pos) => r.SectionPosition = pos);
        }

        foreach (var failed in results.Where(x => !x.IsPassed))
        {
            var tracked = await _unitOfWork.ExamResultRepository.GetByIdTrackedAsync(failed.Id, cancellationToken);
            if (tracked is null)
                continue;

            tracked.MeritPosition = null;
            tracked.ClassPosition = null;
            tracked.SectionPosition = null;
            _unitOfWork.ExamResultRepository.Update(tracked);
        }
    }

    /// <summary>Ranks a group of results by GPA desc then Percentage desc, assigning sequential 1-based positions via the supplied setter.</summary>
    private async Task AssignPositionsAsync(
        IEnumerable<ExamResult> group,
        CancellationToken cancellationToken,
        Action<ExamResult, int> setPosition)
    {
        var ordered = group
            .OrderByDescending(x => x.GPA)
            .ThenByDescending(x => x.Percentage)
            .ToList();

        var position = 1;

        foreach (var result in ordered)
        {
            var tracked = await _unitOfWork.ExamResultRepository.GetByIdTrackedAsync(result.Id, cancellationToken);
            if (tracked is null)
                continue;

            setPosition(tracked, position);
            _unitOfWork.ExamResultRepository.Update(tracked);
            position++;
        }
    }

    /// <summary>Maps a list of ExamResult entities into a position-ordered MeritEntryDto list.</summary>
    private static IReadOnlyList<MeritEntryDto> BuildMeritList(IReadOnlyList<ExamResult> results, Func<ExamResult, int?> positionSelector)
    {
        return results
            .OrderBy(x => positionSelector(x) ?? int.MaxValue)
            .Select(x => new MeritEntryDto
            {
                Position = positionSelector(x) ?? 0,
                StudentId = x.StudentId,
                StudentName = x.Student?.FullName ?? string.Empty,
                RollNo = x.Student?.RollNo ?? string.Empty,
                ClassName = x.Student?.SchoolClass?.Name ?? string.Empty,
                SectionName = x.Student?.Section?.Name ?? string.Empty,
                TotalMarks = x.TotalMarks,
                GPA = x.GPA,
                Grade = x.Grade,
                IsPassed = x.IsPassed
            })
            .ToList();
    }
}
