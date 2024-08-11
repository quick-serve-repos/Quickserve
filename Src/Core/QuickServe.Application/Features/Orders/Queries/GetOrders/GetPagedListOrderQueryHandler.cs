using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Wrappers;
using QuickServe.Domain.Orders.Dtos;

namespace QuickServe.Application.Features.Orders.Queries.GetOrders;

public class GetPagedListOrderQueryHandler : IRequestHandler<GetPagedOrderListQuery, PagedResponse<OrderDtos>>
{
    private readonly IOrderRepository _orderRepository;

    public GetPagedListOrderQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<PagedResponse<OrderDtos>> Handle(GetPagedOrderListQuery request, CancellationToken cancellationToken)
    {
        DateTime? createdDateUtc = null;

        if (request.CreatedDate.HasValue)
        {
            createdDateUtc = DateTime.SpecifyKind(request.CreatedDate.Value, DateTimeKind.Utc);
        }

        var result = await _orderRepository.GetOrderAsync(
            request.PageNumber, 
            request.PageSize, 
            request.StoreId,
            request.RefOrderId,
            createdDateUtc,
            request.Last7Days,
            request.SpecificMonth,
            request.SpecificYear
        );

        return new PagedResponse<OrderDtos>(result, request);
    }
}
