using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Features.FeeStructure.Interfaces;

/// <summary>
/// Repository contract for <see cref="FeeStructure"/> entities.
/// Extends the generic repository with a FeeStructure-specific data access members
/// as they become necessary. Contains database operations only.
/// </summary>
public interface IFeeStructureRepository : IGenericRepository<SchoolERP.Domain.Entities.FeeStructure>
{
}
