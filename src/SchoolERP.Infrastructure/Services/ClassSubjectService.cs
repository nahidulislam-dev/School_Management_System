using AutoMapper;
using SchoolERP.Application.Common.Exceptions;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Features.ClassSubject.DTOs;
using SchoolERP.Application.Features.ClassSubject.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Services;

/// <summary>
/// Business logic for the ClassSubject association. Calls the repository (via the Unit
/// of Work), applies business rules and maps entities to/from DTOs.
/// </summary>
public class ClassSubjectService : IClassSubjectService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ClassSubjectService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ClassSubjectDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.ClassSubjectRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<ClassSubjectDto>>(entities);
    }

    /// <inheritdoc />
    public async Task<ClassSubjectDto?> GetAsync(int classId, int subjectId, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.ClassSubjectRepository.GetAsync(classId, subjectId, cancellationToken);
        return entity is null ? null : _mapper.Map<ClassSubjectDto>(entity);
    }

    /// <inheritdoc />
    public async Task<ClassSubjectDto> AssignAsync(ClassSubjectDto request, CancellationToken cancellationToken = default)
    {
        var exists = await _unitOfWork.ClassSubjectRepository.ExistsAsync(request.ClassId, request.SubjectId, cancellationToken);
        if (exists)
        {
            throw new InvalidOperationException($"ClassSubject association ({request.ClassId}, {request.SubjectId}) already exists.");
        }

        var entity = _mapper.Map<ClassSubject>(request);

        await _unitOfWork.ClassSubjectRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ClassSubjectDto>(entity);
    }

    /// <inheritdoc />
    public async Task RemoveAsync(int classId, int subjectId, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.ClassSubjectRepository.GetAsync(classId, subjectId, cancellationToken)
            ?? throw new NotFoundException("ClassSubject", $"{classId},{subjectId}");

        _unitOfWork.ClassSubjectRepository.Remove(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
