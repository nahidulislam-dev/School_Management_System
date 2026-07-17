using SchoolERP.Application.Features.FeeCollection.DTOs;

namespace SchoolERP.Application.Features.FeeCollection.Interfaces;

/// <summary>
/// Business/service contract for FeeCollection records. Services return DTOs only
/// and encapsulate all business rules for this feature.
/// </summary>
public interface IFeeCollectionService
{
    /// <summary>Retrieves every FeeCollection record.</summary>
    Task<IReadOnlyList<FeeCollectionDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Retrieves a single FeeCollection record by id, or null if it does not exist.</summary>
    Task<FeeCollectionDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Creates a new FeeCollection record.</summary>
    Task<FeeCollectionDto> CreateAsync(CreateFeeCollectionDto request, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing FeeCollection record.</summary>
    Task<FeeCollectionDto> UpdateAsync(int id, UpdateFeeCollectionDto request, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes an existing FeeCollection record.</summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
