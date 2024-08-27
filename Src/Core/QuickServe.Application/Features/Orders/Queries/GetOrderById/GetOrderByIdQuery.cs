using MediatR;
using QuickServe.Application.Parameters;
using QuickServe.Application.Wrappers;
using QuickServe.Domain.Orders.Dtos;
using QuickServe.Domain.ProductTemplates.Dtos;

namespace QuickServe.Application.Features.Orders.Queries.GetOrderById;

public class GetOrderByIdQuery : IRequest<BaseResult<OrderDto>>
{
    public long Id { get; set; }
}