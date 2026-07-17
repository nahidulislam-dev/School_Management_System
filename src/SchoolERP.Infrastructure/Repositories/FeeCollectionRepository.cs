using SchoolERP.Application.Features.FeeCollection.Interfaces;
using SchoolERP.Domain.Entities;
using SchoolERP.Infrastructure.Persistence.Context;
using SchoolERP.Infrastructure.Repositories.Common;

namespace SchoolERP.Infrastructure.Repositories;

/// <summary>
/// EF Core repository implementation for <see cref="FeeCollection"/> entities.
/// Works only with the <see cref="FeeCollection"/> entity; never returns DTOs.
/// </summary>
public class FeeCollectionRepository : GenericRepository<FeeCollection>, IFeeCollectionRepository
{
    public FeeCollectionRepository(SchoolERPDbContext context) : base(context)
    {
    }
}
