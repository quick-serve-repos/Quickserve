using System;
using MediatR;
using QuickServe.Application.Parameters;
using QuickServe.Application.Wrappers;
using QuickServe.Domain.Payments.Dtos;

namespace QuickServe.Application.Features.Payments.Queries.GetPaymentById;

public class GetPaymentByStoreIdQuery : PagenationRequestParameter, IRequest<PagedResponse<PaymentDto>>
{
    
    public long? RefOrderId { get; set; }
    public DateTime? CreatedDate { get; set; }
    public bool Last7Days { get; set; }
    public int? SpecificMonth { get; set; } // Tháng cụ thể
    public int? SpecificYear { get; set; } // Năm cụ thể
}