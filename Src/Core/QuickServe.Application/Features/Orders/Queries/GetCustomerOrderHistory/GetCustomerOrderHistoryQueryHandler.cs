using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using QuickServe.Application.DTOs.Orders.Response;
using QuickServe.Application.Features.Orders.Queries.GetPagedListOrder;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Wrappers;

namespace QuickServe.Application.Features.Orders.Queries.GetCustomerOrderHistory;

public class GetCustomerOrderHistoryQueryHandler : IRequestHandler<GetCustomerOrderHistoryQuery, PagedResponse<OrderHistoryDto>>
{
    private readonly IOrderRepository _orderRepository;

    public GetCustomerOrderHistoryQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<PagedResponse<OrderHistoryDto>> Handle(GetCustomerOrderHistoryQuery request, CancellationToken cancellationToken)
    {
        DateTime? createdDateUtc = null;

        if (request.CreatedDate.HasValue)
        {
            createdDateUtc = DateTime.SpecifyKind(request.CreatedDate.Value, DateTimeKind.Utc);
        }

        var result = await _orderRepository.GetOrdersByCustomerIdAsync(
            request.CustomerId, 
            request.StoreName, 
            createdDateUtc,
            request.Last7Days,
            request.SpecificMonth,
            request.SpecificYear
        );

        return new PagedResponse<OrderHistoryDto>(result, request);
    }
}
