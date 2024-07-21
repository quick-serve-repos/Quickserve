using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuickServe.Application.DTOs;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Domain.Stores.Dtos;
using QuickServe.Domain.Stores.Entities;
using QuickServe.Infrastructure.Persistence.Contexts;

namespace QuickServe.Infrastructure.Persistence.Repositories;

public class StoreRepository : GenericRepository<Store>, IStoreRepository
{
    private readonly DbSet<Store> stores;
    public StoreRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        stores = dbContext.Set<Store>();
    }

    public async Task<Store> FindByIdAsync(long id)
    {
        return await stores.Include(c=> c.Sessions).FirstOrDefaultAsync(c=> c.Id == id);
    }

    public async Task<PagenationResponseDto<StoreDto>> GetPagedListAsync(int pageNumber, int pageSize, string name)
    {
        var query = stores.OrderByDescending(s => s.Created).AsQueryable();
        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(s => s.Name.Contains(name));
        }

        return await Paged(
            query.Select(s => new StoreDto(s)),
            pageNumber,
            pageSize);
    }
}