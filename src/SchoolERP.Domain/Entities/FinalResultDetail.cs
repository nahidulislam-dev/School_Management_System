using SchoolERP.Domain.Common;

namespace SchoolERP.Domain.Entities;

/// <summary>
/// Subject-wise breakdown of a <see cref="FinalResult"/>: the weighted marks
/// contributed by each exam, combined per subject.
/// </summary>
public class FinalResultDetail : BaseEntity
{
    public int FinalResultId { get; set; }
    public FinalResult? FinalResult { get; set; }

    public int SubjectId { get; set; }
    public Subject? Subject { get; set; }

    /// <summary>Weighted marks for this subject: sum over exams of (subject % in that exam) x (exam weight %).</summary>
    public decimal FinalMarks { get; set; }

    /// <summary>Letter grade derived from <see cref="FinalMarks"/> for this subject.</summary>
    public string FinalGradeLabel { get; set; } = string.Empty;

    /// <summary>Grade point derived from <see cref="FinalMarks"/> for this subject.</summary>
    public decimal FinalGradePoint { get; set; }
}
