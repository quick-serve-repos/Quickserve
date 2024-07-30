using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuickServe.Application.Features.Accounts.Commands.RegisterCustomerAccount;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Domain.Accounts.Entities;
using QuickServe.Infrastructure.Identity.Models;
using QuickServe.Infrastructure.Persistence.Contexts;
using System;
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

        public async Task<Account> FindByIdAsync(Guid id)
        {
            return await accounts.Include(c=>c.Staff)
                .ThenInclude(c=>c.Store)
            .FirstOrDefaultAsync(c=> c.Id == id);
        }

        public async Task<Account> RegisterCustomerAccount(RegisterCustomerAccountCommand command)
        {

            throw new NotImplementedException();
        }
    }
}
