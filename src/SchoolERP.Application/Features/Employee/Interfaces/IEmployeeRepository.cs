using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Features.Employee.Interfaces;

/// <summary>
/// Repository contract for <see cref="Employee"/> entities.
/// Extends the generic repository with an Employee-specific data access members
/// as they become necessary. Contains database operations only.
/// </summary>
public interface IEmployeeRepository : IGenericRepository<SchoolERP.Domain.Entities.Employee>
{
}
