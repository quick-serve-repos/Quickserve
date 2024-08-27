using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using QuickServe.Application.Interfaces;
using QuickServe.Application.Utils.Enums;
using QuickServe.Infrastructure.Persistence.Contexts;
using QuickServe.Infrastructure.Persistence.Services.SignalR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickServe.Infrastructure.Persistence.Services
{
    public class SessionService : ISessionService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<NotificationHub> _hubContext;
        public SessionService(ApplicationDbContext context, IUnitOfWork unitOfWork, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _hubContext = hubContext;
        }

        public async Task UpdateSessionStatus()
        {
            var today = DateTime.UtcNow.AddHours(7).Date;

            var sessionsToCheck = await _context.Sessions
                .Where(s =>  s.Status ==(int) SessionStatus.Active)
                .ToListAsync();

            foreach (var session in sessionsToCheck)
            {
                var hasIngredientUpdatedToday = await _context.IngredientSessions
                    .AnyAsync(i => i.SessionId == session.Id && (i.LastModified.HasValue 
                    && i.LastModified.Value.Date == today || i.Created.Date == today));

                if (!hasIngredientUpdatedToday)
                {
                    session.Status = (int) SessionStatus.Inactive;
                    _context.Sessions.Update(session);
                }
                /*var storeManager = await _context.Staffs
                    .Include(s=> s.Store)
                    .Include(s=> s.Account)
                    .Where(s => s.StoreId == session.StoreId && s.Account.UserName == s.Store.StoreManager)
                    .FirstOrDefaultAsync();

                if (storeManager != null)
                {
                    await _hubContext.Clients.User(storeManager.EmployeeId.ToString())
                        .SendAsync("ReceiveNotification", "Ca làm việc "+ session.Name +" chưa được cập nhật");
                }*/
            }
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
