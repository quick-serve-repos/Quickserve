using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Wrappers;
using QuickServe.Domain.Payments.Dtos;

namespace QuickServe.Application.Features.Payments.Queries.GetPaymentById;

public class GetPaymentByStoreIdQueryHandler : IRequestHandler<GetPaymentByStoreIdQuery, PagedResponse<PaymentDto>>
{
    private readonly IPaymentRepository _paymentRepository;

    public GetPaymentByStoreIdQueryHandler(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<PagedResponse<PaymentDto>> Handle(GetPaymentByStoreIdQuery request,
        CancellationToken cancellationToken)
    {
        DateTime? createdDateUtc = null;

        if (request.CreatedDate.HasValue)
        {
            createdDateUtc = DateTime.SpecifyKind(request.CreatedDate.Value, DateTimeKind.Utc);
        }

        var result = await _paymentRepository.GetPagedListByStoreIdAsync(
            request.PageNumber,
            request.PageSize,
            request.StoreId,
            request.RefOrderId,
            createdDateUtc,
            request.Last7Days,
            request.SpecificMonth, // Tháng cụ thể
            request.SpecificYear // Năm cụ thể
        );

        return new PagedResponse<PaymentDto>(result, request);
    }
}