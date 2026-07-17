using AutoMapper;
using SchoolERP.Application.Common.Exceptions;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Features.FeeCollection.DTOs;
using SchoolERP.Application.Features.FeeCollection.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Services;

/// <summary>
/// Business logic for FeeCollection records. Calls the repository (via the Unit of Work),
/// applies business rules, and maps entities to/from DTOs using AutoMapper.
/// </summary>
public class FeeCollectionService : IFeeCollectionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public FeeCollectionService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<FeeCollectionDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.FeeCollectionRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<FeeCollectionDto>>(entities);
    }

    /// <inheritdoc />
    public async Task<FeeCollectionDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.FeeCollectionRepository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : _mapper.Map<FeeCollectionDto>(entity);
    }

    /// <inheritdoc />
    public async Task<FeeCollectionDto> CreateAsync(CreateFeeCollectionDto request, CancellationToken cancellationToken = default)
    {
        var entity = _mapper.Map<FeeCollection>(request);

        await _unitOfWork.FeeCollectionRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<FeeCollectionDto>(entity);
    }

    /// <inheritdoc />
    public async Task<FeeCollectionDto> UpdateAsync(int id, UpdateFeeCollectionDto request, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.FeeCollectionRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(FeeCollection), id);

        _mapper.Map(request, entity);

        _unitOfWork.FeeCollectionRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<FeeCollectionDto>(entity);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.FeeCollectionRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(FeeCollection), id);

        _unitOfWork.FeeCollectionRepository.Delete(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
