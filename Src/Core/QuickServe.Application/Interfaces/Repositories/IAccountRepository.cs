using QuickServe.Domain.Accounts.Entities;
using System;
using System.Threading.Tasks;

namespace QuickServe.Application.Interfaces.Repositories
{
    public interface IAccountRepository : IGenericRepository<Account>
    {
        Task<Account> FindByIdAsync(Guid id);
        Task<bool> ExistEmailAsync(string email);
        Task<bool> ExistPhoneAsync(string phone);
        Task<bool> ExistByUsername(string username);
        Task<int> GetTotalAccountsAsync();
        Task<int> GetFilteredAccountsCountByRoleAsync(DateTime startDate, DateTime endDate, string role);
        Task<int> GetFilteredAccountsCountAsync(DateTime startDate, DateTime endDate);
    }
}
