using MediatR;
using QuickServe.Application.DTOs.Account.Responses;
using QuickServe.Application.DTOs.Orders.Response;
using QuickServe.Application.Features.Orders.Queries.GetStoreRevenueReport;
using QuickServe.Application.Interfaces;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QuickServe.Application.Features.Accounts.AccountReport
{
    public class AccountReportQueryHandler(IAccountRepository accountRepository, IStaffRepository staffRepository) : IRequestHandler<AccountReportQuery, BaseResult<AccountReportDto>>
    {
        public async Task<BaseResult<AccountReportDto>> Handle(AccountReportQuery request, CancellationToken cancellationToken)
        {
            DateTime startDate;
            DateTime endDate;

            if (request.SpecificDate.HasValue)
            {
                startDate = request.SpecificDate.Value.Date;
                endDate = request.SpecificDate.Value.Date.AddDays(1).AddTicks(-1);
            }
            else if (request.StartDate.HasValue && request.EndDate.HasValue)
            {
                startDate = request.StartDate.Value.Date;
                endDate = request.EndDate.Value.Date.AddDays(1).AddTicks(-1);
            }
            else if (request.Month.HasValue && request.Year.HasValue)
            {
                startDate = new DateTime(request.Year.Value, request.Month.Value, 1);
                endDate = startDate.AddMonths(1).AddDays(-1);
            }
            else if (request.Year.HasValue)
            {
                startDate = new DateTime(request.Year.Value, 1, 1);
                endDate = startDate.AddYears(1).AddDays(-1);
            }
            else
            {
                return new BaseResult<AccountReportDto>(new Error(ErrorCode.FieldDataInvalid, "Phạm vi ngày, tháng/năm hoặc ngày cụ thể không hợp lệ"));
            }

            //startDate = startDate.ToUniversalTime();
            //endDate = endDate.ToUniversalTime();
            // Chuyển đổi DateTime thành Unspecified để tránh lỗi khi lưu vào PostgreSQL
            startDate = DateTime.SpecifyKind(startDate, DateTimeKind.Unspecified);
            endDate = DateTime.SpecifyKind(endDate, DateTimeKind.Unspecified);

            // Thống kê tổng số tài khoản và số tài khoản theo vai trò
            var totalAccountsCount = await accountRepository.GetFilteredAccountsCountAsync(startDate, endDate);
            var roles = new[] { "Staff", "Admin", "Customer", "Store_Manager", "Brand_Manager" };
            var accountsByRole = new Dictionary<string, int>();

            foreach (var role in roles)
            {
                var roleCount = await accountRepository.GetFilteredAccountsCountByRoleAsync(startDate, endDate, role);
                accountsByRole[role] = roleCount;
            }

            // Thống kê số lượng nhân viên
            var totalStaffCount = await staffRepository.CountStaffByDateRangeAsync(startDate, endDate);
            var staffByStoreCount = request.StoreId.HasValue
                ? await staffRepository.CountStaffByStoreIdAndDateRangeAsync(request.StoreId.Value, startDate, endDate)
                : totalStaffCount;

            var result = new AccountReportDto
            {
                TotalAccounts = totalAccountsCount,
                AccountsByRole = accountsByRole,
                TotalEmployeeCount = totalStaffCount,
                EmployeeByStoreCount = staffByStoreCount
            };

            return new BaseResult<AccountReportDto>(result);
        }
    }
}
