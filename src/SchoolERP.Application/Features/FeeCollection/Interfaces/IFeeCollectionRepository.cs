using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Features.FeeCollection.Interfaces;

/// <summary>
/// Repository contract for <see cref="FeeCollection"/> entities.
/// Extends the generic repository with a FeeCollection-specific data access members
/// as they become necessary. Contains database operations only.
/// </summary>
public interface IFeeCollectionRepository : IGenericRepository<SchoolERP.Domain.Entities.FeeCollection>
{
}
