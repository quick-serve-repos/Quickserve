using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuickServe.Application.DTOs;
using QuickServe.Application.Interfaces;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Domain.Accounts.Dtos;
using QuickServe.Domain.Staffs.Entities;
using QuickServe.Domain.Stores.Dtos;
using QuickServe.Infrastructure.Identity.Models;
using QuickServe.Infrastructure.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace QuickServe.Infrastructure.Persistence.Repositories
{
    public class StaffRepository : GenericRepository<Employee>, IStaffRepository
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUnitOfWork unitOfWork;
        public StaffRepository(ApplicationDbContext context, UserManager<ApplicationUser> userManager) : base(context)
        {
            this.context = context;
            this.userManager = userManager;
        }

        public void AddStaffToStore(long storeId, Guid employeeId)
        {
            context.Add(new Employee { StoreId = storeId, EmployeeId = employeeId });
            context.SaveChanges();
        }

        public async Task<PagenationResponseDto<EmployeeDto>> GetPagedListStaffByStoreIdAsync(long storeId, int pageNumber, int pageSize, string name, CancellationToken cancellationToken, string[] roles)
        {
            var staffs = context.Staffs.Where(s => s.StoreId == storeId).OrderByDescending(s => s.Account.Created).AsQueryable();

            var staffUserIds = await staffs.Select(s => s.Account.Id).ToListAsync(cancellationToken);

            var listRoles = roles.ToList();
            var query = userManager.Users
                .Where(u => staffUserIds.Contains(u.Id))
                .Select(c => new AccountDto
                {
                    Id = c.Id,
                    UserName = c.UserName,
                    Email = c.Email,
                    Created = c.Created,
                    PhoneNumber = c.PhoneNumber,
                    Name = c.Name,
                    Avatar = null,
                    Address = null
                });

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.UserName.Contains(name) || c.Email.Contains(name));
            }

            var count = await query.CountAsync(cancellationToken);

            var accountInListRoles = new List<AccountDto>();
            var listAccount = await query.ToListAsync(cancellationToken);

            foreach (var item in listAccount)
            {
                var user = await userManager.FindByIdAsync(item.Id.ToString());
                var rolesList = await userManager.GetRolesAsync(user).ConfigureAwait(false);
                item.Role = rolesList.FirstOrDefault();
                if (listRoles.Any(p => item.Role.Contains(p)))
                {
                    accountInListRoles.Add(item);
                }
            }

            var result = listAccount.AsQueryable();
            if (roles != null && roles.Length > 0)
            {
                result = accountInListRoles.AsQueryable();
            }
            var paginatedResult = result
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var employeeDtos = paginatedResult.Select(e => new EmployeeDto
            {
                Id = e.Id,
                Name = e.Name,
                Email = e.Email,
                Roles = e.Role,
                PhoneNumber = e.PhoneNumber,
                Created = e.Created,
                UserName = e.UserName,
            }).ToList();

            return new PagenationResponseDto<EmployeeDto>(employeeDtos, count);
        }

        public async Task<int> CountStaffByStoreIdAndDateRangeAsync(long storeId, DateTime startDate, DateTime endDate)
        {
            return await context.Staffs
                .Where(s => s.StoreId == storeId &&
                            s.Account.Created >= startDate &&
                            s.Account.Created <= endDate)
                .CountAsync();
        }

        public async Task<int> CountStaffByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await context.Staffs
                .Where(s => s.Account.Created >= startDate &&
                            s.Account.Created <= endDate)
                .CountAsync();
        }
    }
}
