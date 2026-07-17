using SchoolERP.Application.Features.FeeStructure.Interfaces;
using SchoolERP.Domain.Entities;
using SchoolERP.Infrastructure.Persistence.Context;
using SchoolERP.Infrastructure.Repositories.Common;

namespace SchoolERP.Infrastructure.Repositories;

/// <summary>
/// EF Core repository implementation for <see cref="FeeStructure"/> entities.
/// Works only with the <see cref="FeeStructure"/> entity; never returns DTOs.
/// </summary>
public class FeeStructureRepository : GenericRepository<FeeStructure>, IFeeStructureRepository
{
    public FeeStructureRepository(SchoolERPDbContext context) : base(context)
    {
    }
}
