using MediatR;
using QuickServe.Application.DTOs.Orders.Response;
using QuickServe.Application.Wrappers;

namespace QuickServe.Application.Features.Orders.Queries.CancelOrder;

public class CancelOrderCommand : IRequest<BaseResult<OrderResponse>>
{
    public long OrderId { get; set; }
}