using MediatR;
using QuickServe.Application.Interfaces;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.DTOs.Payment;
using QuickServe.Application.Wrappers;
using System.Threading;
using System.Threading.Tasks;
using System;
using QuickServe.Application.Utils.Enums;

namespace QuickServe.Application.Features.Payments.Commands.CreatePayment;

public class CreatePayOSCommandHandler(IPaymentService paymentService, IOrderRepository orderRepository) : IRequestHandler<CreatePayOSCommand, BaseResult<PaymentResponse>>
{
    public async Task<BaseResult<PaymentResponse>> Handle(CreatePayOSCommand request, CancellationToken cancellationToken)
    {
        //Validate
        var order = await orderRepository.GetByIdAsync(request.OrderId);
        if (order == null)
            return new BaseResult<PaymentResponse>(new Error(ErrorCode.NotFound, "Order not found"));

        if (order.Amount != request.TotalPrice)
            return new BaseResult<PaymentResponse>(new Error(ErrorCode.NotFound, "Price not valid"));

        var paymentOS = new Utils.Payments.Model.CreatePaymentRequest()
        {
            Amount = (int)order.Amount,
            OrderCode = order.Id,
            ReturnUrl = PayOSEnum.PayOS_ReturnUrl
        };

        var paymentUrl = await paymentService.CreatePayOSPaymentAsync(paymentOS, cancellationToken);
        
        return new BaseResult<PaymentResponse>(new PaymentResponse { PaymentUrl = paymentUrl });
    }
}