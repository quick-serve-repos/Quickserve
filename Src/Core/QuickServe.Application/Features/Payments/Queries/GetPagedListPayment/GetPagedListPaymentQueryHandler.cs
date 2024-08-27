using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Wrappers;
using QuickServe.Domain.Payments.Dtos;

namespace QuickServe.Application.Features.Payments.Queries.GetPagedListPayment;

public class GetPagedListPaymentQueryHandler : IRequestHandler<GetPagedListPaymentQuery, PagedResponse<PaymentDto>>
{
    private readonly IPaymentRepository _paymentRepository;

    public GetPagedListPaymentQueryHandler(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }


    public async Task<PagedResponse<PaymentDto>> Handle(GetPagedListPaymentQuery request,
        CancellationToken cancellationToken)
    {
        DateTime? createdDateUtc = null;

        if (request.CreatedDate.HasValue)
        {
            createdDateUtc = DateTime.SpecifyKind(request.CreatedDate.Value, DateTimeKind.Utc);
        }

        var result = await _paymentRepository.GetPagedListAsync(
            request.PageNumber,
            request.PageSize,
            request.StoreId,
            request.RefOrderId,
            createdDateUtc,
            request.Last7Days,
            request.SpecificMonth,
            request.SpecificYear
        );

        return new PagedResponse<PaymentDto>(result, request);
    }
}