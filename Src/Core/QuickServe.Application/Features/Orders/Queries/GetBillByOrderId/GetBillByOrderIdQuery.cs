using MediatR;
using QuickServe.Application.DTOs.Bill;
using QuickServe.Application.Wrappers;

namespace QuickServe.Application.Features.Orders.Queries.GetBillByOrderId;

public class GetBillByOrderIdQuery : IRequest<BaseResult<BillDto>>
{
    public long OrderId { get; set; }
}