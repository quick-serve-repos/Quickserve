using System;
using MediatR;
using QuickServe.Application.Parameters;
using QuickServe.Application.Wrappers;
using QuickServe.Domain.Orders.Dtos;

namespace QuickServe.Application.Features.Orders.Queries.GetOrders;

public class GetOrderByStoreIdQuery  : PagenationRequestParameter, IRequest<PagedResponse<OrderDtos>>
{
    public long StoreId { get; set; }
    public long? RefOrderId { get; set; }
    public DateTime? CreatedDate { get; set; }
    public bool Last7Days { get; set; }
    public int? SpecificMonth { get; set; }  // Tháng cụ thể
    public int? SpecificYear { get; set; }   // Năm cụ thể
}