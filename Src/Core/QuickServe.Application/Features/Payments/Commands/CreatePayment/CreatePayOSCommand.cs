using System.Text.Json.Serialization;
using System;
using MediatR;
using QuickServe.Application.Wrappers;
using QuickServe.Application.DTOs.Payment;

namespace QuickServe.Application.Features.Payments.Commands.CreatePayment;

public class CreatePayOSCommand : IRequest<BaseResult<PaymentResponse>>
{
    public long OrderId { get; set; }

    public long? TotalPrice { get; set; }

    public string Note { get; set; }
}