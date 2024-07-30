using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using QuickServe.Application.Features.Accounts.Commands.RegisterCustomerAccount;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Domain.Accounts.Entities;
using QuickServe.Infrastructure.Identity.Models;
using QuickServe.Infrastructure.Persistence.Contexts;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace QuickServe.Infrastructure.Persistence.Repositories
{
    public class AccountRepository : GenericRepository<Account>, IAccountRepository
    {
        private readonly DbSet<Account> accounts;
        private readonly UserManager<ApplicationUser> userManager;
        public AccountRepository(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager) : base(dbContext)
        {
            accounts = dbContext.Set<Account>();
            this.userManager = userManager;
        }

        public async Task<bool> ExistByUsername(string username)
        {
            return await accounts.AnyAsync(ac => ac.UserName == username);
        }

        public async Task<bool> ExistEmailAsync(string email)
        {
            return await accounts.AnyAsync(ac => ac.Email == email);
        }

        public async Task<bool> ExistPhoneAsync(string phone)
        {
            return await accounts.AnyAsync(ac => ac.PhoneNumber == phone);
        }

        public async Task<Account> FindByIdAsync(Guid id)
        {
            return await accounts.Include(c=>c.Staff)
                .ThenInclude(c=>c.Store)
            .FirstOrDefaultAsync(c=> c.Id == id);
        }

        public async Task<int> GetTotalAccountsAsync()
        {
            return await accounts.CountAsync();
        }

        public async Task<int> GetFilteredAccountsCountByRoleAsync(DateTime startDate, DateTime endDate, string role)
        {
            var userIds = await userManager.GetUsersInRoleAsync(role);
            return await accounts
                .Where(ac => ac.Created >= startDate && ac.Created <= endDate && userIds.Select(u => u.Id).Contains(ac.Id))
                .CountAsync();
        }

        public async Task<int> GetFilteredAccountsCountAsync(DateTime startDate, DateTime endDate)
        {
            return await accounts
                .Where(ac => ac.Created >= startDate && ac.Created <= endDate)
                .CountAsync();
        }

    }
}
