using AutoMapper;
using SchoolERP.Application.Common.Exceptions;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Features.FeeType.DTOs;
using SchoolERP.Application.Features.FeeType.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Services;

/// <summary>
/// Business logic for FeeType records. Calls the repository (via the Unit of Work),
/// applies business rules, and maps entities to/from DTOs using AutoMapper.
/// </summary>
public class FeeTypeService : IFeeTypeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public FeeTypeService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<FeeTypeDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.FeeTypeRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<FeeTypeDto>>(entities);
    }

    /// <inheritdoc />
    public async Task<FeeTypeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.FeeTypeRepository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : _mapper.Map<FeeTypeDto>(entity);
    }

    /// <inheritdoc />
    public async Task<FeeTypeDto> CreateAsync(CreateFeeTypeDto request, CancellationToken cancellationToken = default)
    {
        var entity = _mapper.Map<FeeType>(request);

        await _unitOfWork.FeeTypeRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<FeeTypeDto>(entity);
    }

    /// <inheritdoc />
    public async Task<FeeTypeDto> UpdateAsync(int id, UpdateFeeTypeDto request, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.FeeTypeRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(FeeType), id);

        _mapper.Map(request, entity);

        _unitOfWork.FeeTypeRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<FeeTypeDto>(entity);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.FeeTypeRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(FeeType), id);

        _unitOfWork.FeeTypeRepository.Delete(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
