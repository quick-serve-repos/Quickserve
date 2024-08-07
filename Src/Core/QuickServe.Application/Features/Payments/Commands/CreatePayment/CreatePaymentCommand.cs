using System.Text.Json.Serialization;
using System;
using MediatR;
using QuickServe.Application.Wrappers;
using QuickServe.Application.DTOs.Payment;

namespace QuickServe.Application.Features.Payments.Commands.CreatePayment;

public class CreatePaymentCommand : IRequest<BaseResult<PaymentResponse>>
{
    public long OrderId { get; set; }

    public long? TotalPrice { get; set; }

    public string OrderInfo { get; set; }
    
}