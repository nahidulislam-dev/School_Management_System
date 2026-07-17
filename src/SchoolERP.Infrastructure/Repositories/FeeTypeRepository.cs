using SchoolERP.Application.Features.FeeType.Interfaces;
using SchoolERP.Domain.Entities;
using SchoolERP.Infrastructure.Persistence.Context;
using SchoolERP.Infrastructure.Repositories.Common;

namespace SchoolERP.Infrastructure.Repositories;

/// <summary>
/// EF Core repository implementation for <see cref="FeeType"/> entities.
/// Works only with the <see cref="FeeType"/> entity; never returns DTOs.
/// </summary>
public class FeeTypeRepository : GenericRepository<FeeType>, IFeeTypeRepository
{
    public FeeTypeRepository(SchoolERPDbContext context) : base(context)
    {
    }
}
