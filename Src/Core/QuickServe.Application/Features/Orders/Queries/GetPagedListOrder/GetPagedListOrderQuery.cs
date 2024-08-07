using MediatR;
using QuickServe.Application.Parameters;
using QuickServe.Application.Wrappers;
using QuickServe.Domain.Orders.Dtos;

namespace QuickServe.Application.Features.Orders.Queries.GetPagedListOrder;

public class GetPagedListOrderQuery : PagenationRequestParameter, IRequest<PagedResponse<OrderDto>>
{
    
}