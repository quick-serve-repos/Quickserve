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

            var specificRevenue = await orderRepository.GetRevenueReportAsync(startDate, endDate, null);
            var totalRevenue = await orderRepository.GetTotalRevenueAsync(null);
            var specificOrderCount = await orderRepository.GetOrderCountAsync(startDate, endDate, null);
            var totalOrderCount = await orderRepository.GetTotalOrderCountAsync(null);

            var result = new RevenueReportDto
            {
                StartDate = startDate,
                EndDate = endDate,
                SpecificRevenue = specificRevenue,
                TotalRevenue = totalRevenue,
                SpecificOrderCount = specificOrderCount,
                TotalOrderCount = totalOrderCount
            };

            return new BaseResult<RevenueReportDto>(result);
        }
    }
}
