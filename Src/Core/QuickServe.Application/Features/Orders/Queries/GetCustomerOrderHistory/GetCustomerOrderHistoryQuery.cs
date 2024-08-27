using System;
using System.Collections.Generic;
using MediatR;
using QuickServe.Application.DTOs.Orders.Response;
using QuickServe.Application.Parameters;
using QuickServe.Application.Wrappers;

namespace QuickServe.Application.Features.Orders.Queries.GetCustomerOrderHistory;


public class GetCustomerOrderHistoryQuery : PagenationRequestParameter, IRequest<PagedResponse<OrderHistoryDto>>
{
    //public Guid CustomerId { get; set; }
    public string StoreName { get; set; }
    public DateTime? CreatedDate { get; set; }
    public bool Last7Days { get; set; }
    public int? SpecificMonth { get; set; }  // Tháng cụ thể
    public int? SpecificYear { get; set; }   // Năm cụ thể
}


