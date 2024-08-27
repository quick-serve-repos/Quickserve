using Microsoft.EntityFrameworkCore;
using QuickServe.Application.DTOs;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Utils;
using QuickServe.Domain.Sessions.Dtos;
using QuickServe.Domain.Sessions.Entities;
using QuickServe.Domain.Stores.Entities;
using QuickServe.Infrastructure.Persistence.Contexts;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickServe.Infrastructure.Persistence.Repositories
{
    public class SessionRepository : GenericRepository<Session>, ISessionRepository
    {
        private readonly DbSet<Session> sessions;
        private readonly DbSet<Store> stores;

        public SessionRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            sessions = dbContext.Sessions;
            stores = dbContext.Stores;
        }

        public async Task<bool> ExistsByNameAsync(long storeId, string name)
        {
            return await sessions.AnyAsync(c => c.StoreId == storeId && c.Name.ToLower() == name.ToLower());
        }

        public async Task<bool> ExistsByTimeAsync(long storeId, TimeSpan startTime, TimeSpan endTime)
        {
            return await sessions.AnyAsync(c=> c.StoreId ==  storeId && (c.StartTime < endTime && c.EndTime > startTime));
        }

        public async Task<Session> FindByIdAsync(long id)
        {
            return await sessions.Include(c => c.IngredientSessions)
             .Include(c=> c.Store)
            .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<PagenationResponseDto<SessionDto>> GetPagedListAsyncByStore(long storeId, int pageNumber, int pageSize, string name)
        {
            if(await stores.AnyAsync(c=> c.Id == storeId) == false)
            {
                throw new Exception("Không tìm thấy cửa hàng.");
            }
            var query = sessions.Where(c=>c.StoreId == storeId).OrderBy(c => c.Created).AsQueryable();
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.Name.Contains(name));
            }

            return await Paged(
                query.Select(c => new SessionDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    StoreId = c.StoreId,
                    StartTime = c.StartTime,
                    EndTime = c.EndTime,
                    Status = c.Status,
                    Created = TimeZoneConverter.ConvertToUserTimeZone(c.Created),
                    CreatedBy = c.CreatedBy,
                    LastModified = c.LastModified.HasValue
                    ? TimeZoneConverter.ConvertToUserTimeZone(c.LastModified.Value)
                    : (DateTime?)null,  // Xử lý giá trị NULL
                    LastModifiedBy = c.LastModifiedBy ?? null // Xử lý giá trị NULL
                }),
                pageNumber,
                pageSize);
        }

    }
}
