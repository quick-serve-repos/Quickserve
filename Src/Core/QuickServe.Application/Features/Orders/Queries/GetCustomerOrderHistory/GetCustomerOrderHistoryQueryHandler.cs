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

public class GetCustomerOrderHistoryQueryHandler : IRequestHandler<GetCustomerOrderHistoryQuery, BaseResult<List<OrderHistoryDto>>>
{
    private readonly IOrderRepository _orderRepository;

    public GetCustomerOrderHistoryQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<BaseResult<List<OrderHistoryDto>>> Handle(GetCustomerOrderHistoryQuery request, CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetOrdersByCustomerIdAsync(request.CustomerId);
        if (orders == null || !orders.Any())
        {
            return new BaseResult<List<OrderHistoryDto>>(new List<OrderHistoryDto>());
        }

        var orderHistoryDtos = orders.Select(order => new OrderHistoryDto(order)).ToList();
        return new BaseResult<List<OrderHistoryDto>>(orderHistoryDtos);
    }
    
    
    
    
}