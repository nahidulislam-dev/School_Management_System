using SchoolERP.Application.Features.FeeType.DTOs;

namespace SchoolERP.Application.Features.FeeType.Interfaces;

/// <summary>
/// Business/service contract for FeeType records. Services return DTOs only
/// and encapsulate all business rules for this feature.
/// </summary>
public interface IFeeTypeService
{
    /// <summary>Retrieves every FeeType record.</summary>
    Task<IReadOnlyList<FeeTypeDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Retrieves a single FeeType record by id, or null if it does not exist.</summary>
    Task<FeeTypeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Creates a new FeeType record.</summary>
    Task<FeeTypeDto> CreateAsync(CreateFeeTypeDto request, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing FeeType record.</summary>
    Task<FeeTypeDto> UpdateAsync(int id, UpdateFeeTypeDto request, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes an existing FeeType record.</summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
