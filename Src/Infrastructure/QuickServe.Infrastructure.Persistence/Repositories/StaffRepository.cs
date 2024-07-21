using QuickServe.Application.DTOs;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Domain.Staffs.Entities;
using QuickServe.Domain.Stores.Dtos;
using QuickServe.Infrastructure.Persistence.Contexts;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace QuickServe.Infrastructure.Persistence.Repositories
{
    public class StaffRepository : GenericRepository<Staff>, IStaffRepository
    {
        private readonly ApplicationDbContext context;

        public StaffRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }

        public void AddStaffToStore(long storeId, Guid employeeId)
        {
            context.Add(new Staff { StoreId = storeId, EmployeeId = employeeId });
            context.SaveChanges();
        }

        public async Task<PagenationResponseDto<EmployeeDto>> GetPagedListStaffByStoreIdAsync(long storeId, int pageNumber, int pageSize, string name, CancellationToken cancellationToken)
        {
            var staffs = context.Staffs.Where(s => s.StoreId == storeId).OrderByDescending(s => s.Account.Created).AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                staffs = staffs.Where(s => s.Account.Name.Contains(name));
            }

            return await Paged(staffs.Select(e => new EmployeeDto {
                Id = e.Account.Id,
                Name = e.Account.Name,
                Email = e.Account.Email,
                PhoneNumber = e.Account.PhoneNumber,
                Created = e.Account.Created,
                UserName = e.Account.UserName,
            }),
                pageNumber,
                pageSize);
        }
    }
}
