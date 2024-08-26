using Microsoft.EntityFrameworkCore;
using QuickServe.Application.Interfaces;
using QuickServe.Application.Utils.Enums;
using QuickServe.Infrastructure.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickServe.Infrastructure.Persistence.Services
{
    public class SessionService : ISessionService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public SessionService(ApplicationDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
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
            }
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
