using AutoMapper;
using SchoolERP.Application.Common.Exceptions;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Features.FeeStructure.DTOs;
using SchoolERP.Application.Features.FeeStructure.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Services;

/// <summary>
/// Business logic for FeeStructure records. Calls the repository (via the Unit of Work),
/// applies business rules, and maps entities to/from DTOs using AutoMapper.
/// </summary>
public class FeeStructureService : IFeeStructureService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public FeeStructureService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<FeeStructureDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.FeeStructureRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<FeeStructureDto>>(entities);
    }

    /// <inheritdoc />
    public async Task<FeeStructureDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.FeeStructureRepository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : _mapper.Map<FeeStructureDto>(entity);
    }

    /// <inheritdoc />
    public async Task<FeeStructureDto> CreateAsync(CreateFeeStructureDto request, CancellationToken cancellationToken = default)
    {
        var entity = _mapper.Map<FeeStructure>(request);

        await _unitOfWork.FeeStructureRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<FeeStructureDto>(entity);
    }

    /// <inheritdoc />
    public async Task<FeeStructureDto> UpdateAsync(int id, UpdateFeeStructureDto request, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.FeeStructureRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(FeeStructure), id);

        _mapper.Map(request, entity);

        _unitOfWork.FeeStructureRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<FeeStructureDto>(entity);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.FeeStructureRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(FeeStructure), id);

        _unitOfWork.FeeStructureRepository.Delete(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
