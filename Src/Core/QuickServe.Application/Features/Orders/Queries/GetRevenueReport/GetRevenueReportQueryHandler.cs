using MediatR;
using QuickServe.Application.DTOs.Orders.Response;
using QuickServe.Application.Interfaces.IOrderServices;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Wrappers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QuickServe.Application.Features.Orders.Queries.GetRevenueReport
{
    public class GetRevenueReportQueryHandler(IOrderRepository orderRepository) : IRequestHandler<GetRevenueReportQuery, BaseResult<RevenueReportDto>>
    {
        public async Task<BaseResult<RevenueReportDto>> Handle(GetRevenueReportQuery request, CancellationToken cancellationToken)
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
                return new BaseResult<RevenueReportDto>(new Error(ErrorCode.FieldDataInvalid, "Phạm vi ngày, tháng/năm hoặc ngày cụ thể không hợp lệ"));
            }
            startDate = startDate.ToUniversalTime();
            endDate = endDate.ToUniversalTime();


            var specificRevenue = await orderRepository.GetRevenueReportAsync(startDate, endDate, request.StoreId);
            var totalRevenue = await orderRepository.GetTotalRevenueAsync(request.StoreId);
            var specificOrderCount = await orderRepository.GetOrderCountAsync(startDate, endDate, request.StoreId);
            var totalOrderCount = await orderRepository.GetTotalOrderCountAsync(request.StoreId);

            List<MonthlyRevenueDto> monthlyRevenues = null;
            List<YearlyRevenueDto> yearlyRevenues = null;

            if (startDate.Year == endDate.Year)
            {
                monthlyRevenues = await GetMonthlyRevenues(request.StoreId, startDate, endDate);
            }
            else
            {
                yearlyRevenues = await GetYearlyRevenues(request.StoreId, startDate.Year, endDate.Year);
            }
            var result = new RevenueReportDto
            {
                StartDate = startDate,
                EndDate = endDate,
                SpecificRevenue = specificRevenue,
                TotalRevenue = totalRevenue,
                SpecificOrderCount = specificOrderCount,
                TotalOrderCount = totalOrderCount,
                MonthlyRevenues = monthlyRevenues,
                YearlyRevenues = yearlyRevenues
            };

            return new BaseResult<RevenueReportDto>(result);
        }
        private async Task<List<MonthlyRevenueDto>> GetMonthlyRevenues(long? storeId, DateTime startDate, DateTime endDate)
        {
            var monthlyRevenues = new List<MonthlyRevenueDto>();
            for (var date = startDate; date <= endDate; date = date.AddMonths(1))
            {
                var monthStart = new DateTime(date.Year, date.Month, 1);
                var monthEnd = monthStart.AddMonths(1).AddDays(-1);
                var revenue = await orderRepository.GetRevenueReportAsync(monthStart.ToUniversalTime(), monthEnd.ToUniversalTime(), storeId);
                var orderCount = await orderRepository.GetOrderCountAsync(monthStart.ToUniversalTime(), monthEnd.ToUniversalTime(), storeId);
                monthlyRevenues.Add(new MonthlyRevenueDto { Month = date.Month, Revenue = revenue, OrderCount = orderCount });
            }
            return monthlyRevenues;
        }

        private async Task<List<YearlyRevenueDto>> GetYearlyRevenues(long? storeId, int startYear, int endYear)
        {
            var yearlyRevenues = new List<YearlyRevenueDto>();
            for (int year = startYear; year <= endYear; year++)
            {
                var yearStart = new DateTime(year, 1, 1);
                var yearEnd = yearStart.AddYears(1).AddDays(-1);
                var revenue = await orderRepository.GetRevenueReportAsync(yearStart.ToUniversalTime(), yearEnd.ToUniversalTime(), storeId);
                var orderCount = await orderRepository.GetOrderCountAsync(yearStart.ToUniversalTime(), yearEnd.ToUniversalTime(), storeId);
                yearlyRevenues.Add(new YearlyRevenueDto { Year = year, Revenue = revenue, OrderCount = orderCount });
            }
            return yearlyRevenues;
        }
    }

}
