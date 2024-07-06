using MediatR;
using QuickServe.Application.DTOs.Orders.Response;
using QuickServe.Application.DTOs.Payment;
using QuickServe.Application.Interfaces;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Wrappers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace QuickServe.Application.Features.Payments.Commands.CreatePayment;

public class CreatePaymentCommandHandler(IPaymentService paymentService, IOrderRepository orderRepository) : IRequestHandler<CreatePaymentCommand, BaseResult<PaymentResponse>>
{
    public async Task<BaseResult<PaymentResponse>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        //Validate
        var order = await orderRepository.GetByIdAsync(request.OrderId);
        if (order == null)
            return new BaseResult<PaymentResponse>(new Error(ErrorCode.NotFound, "Order not found"));

        if (order.Amount != request.TotalPrice)
            return new BaseResult<PaymentResponse>(new Error(ErrorCode.NotFound, "Price not valid"));

        var paymentUrl = await paymentService.CreateVNPayPaymentUrlAsync(request, cancellationToken);
        
        return new BaseResult<PaymentResponse>(new PaymentResponse { PaymentUrl = paymentUrl });
    }
}