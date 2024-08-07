using MediatR;
using QuickServe.Application.DTOs.Orders.Response;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QuickServe.Application.Features.Orders.Queries.GetBestSellingProductTemplates
{
    public class GetBestSellingProductTemplatesQueryHandler(IOrderRepository _orderRepository) : IRequestHandler<GetBestSellingProductTemplatesQuery, BaseResult<BestSellingReportDto>>
    {
        public async Task<BaseResult<BestSellingReportDto>> Handle(GetBestSellingProductTemplatesQuery request, CancellationToken cancellationToken)
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
                return new BaseResult<BestSellingReportDto>(new Error(ErrorCode.FieldDataInvalid, "Phạm vi ngày, tháng/năm hoặc ngày cụ thể không hợp lệ"));
            }

            var soldIngredients = await _orderRepository.GetSoldIngredientsAsync(startDate.ToUniversalTime(), endDate.ToUniversalTime(), request.StoreId);
            var reportDto = await _orderRepository.GetBestSellingProductTemplatesAsync(startDate.ToUniversalTime(), endDate.ToUniversalTime(), request.StoreId);


            if (request.Top.HasValue)
            {
                reportDto.SoldIngredients = soldIngredients
                    .OrderByDescending(i => i.QuantitySold)
                    .Take(request.Top.Value)
                    .ToList();
            }
            else
            {
                reportDto.SoldIngredients = soldIngredients.ToList();
            }

            if (request.Top.HasValue)
            {
                reportDto.BestSellingProductTemplates = reportDto.BestSellingProductTemplates
                    .OrderByDescending(p => p.SellingQuantity)
                    .Take(request.Top.Value)
                    .ToList();
            }
            else
            {
                reportDto.BestSellingProductTemplates = reportDto.BestSellingProductTemplates.ToList();
            }



            return new BaseResult<BestSellingReportDto>(reportDto);
        }
    }
}
