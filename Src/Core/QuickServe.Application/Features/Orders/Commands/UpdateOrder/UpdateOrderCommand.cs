using MediatR;
using QuickServe.Application.DTOs.Orders.Response;
using QuickServe.Application.Wrappers;

namespace QuickServe.Application.Features.Orders.Commands.UpdateOrder;

public class UpdateOrderCommand : IRequest<BaseResult<OrderResponse>>
{
    public long OrderId { get; set; }
    public int Status { get; set; }
}