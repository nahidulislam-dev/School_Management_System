using SchoolERP.Application.Features.FeeStructure.DTOs;

namespace SchoolERP.Application.Features.FeeStructure.Interfaces;

/// <summary>
/// Business/service contract for FeeStructure records. Services return DTOs only
/// and encapsulate all business rules for this feature.
/// </summary>
public interface IFeeStructureService
{
    /// <summary>Retrieves every FeeStructure record.</summary>
    Task<IReadOnlyList<FeeStructureDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Retrieves a single FeeStructure record by id, or null if it does not exist.</summary>
    Task<FeeStructureDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Creates a new FeeStructure record.</summary>
    Task<FeeStructureDto> CreateAsync(CreateFeeStructureDto request, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing FeeStructure record.</summary>
    Task<FeeStructureDto> UpdateAsync(int id, UpdateFeeStructureDto request, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes an existing FeeStructure record.</summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
