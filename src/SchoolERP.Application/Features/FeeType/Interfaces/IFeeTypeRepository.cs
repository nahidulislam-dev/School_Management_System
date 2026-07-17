using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Features.FeeType.Interfaces;

/// <summary>
/// Repository contract for <see cref="FeeType"/> entities.
/// Extends the generic repository with a FeeType-specific data access members
/// as they become necessary. Contains database operations only.
/// </summary>
public interface IFeeTypeRepository : IGenericRepository<SchoolERP.Domain.Entities.FeeType>
{
}
