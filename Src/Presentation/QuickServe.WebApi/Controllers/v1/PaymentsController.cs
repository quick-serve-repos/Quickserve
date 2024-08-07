using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuickServe.Application.DTOs.Payment;
using QuickServe.Application.Features.Payments.Commands.CreatePayment;
using QuickServe.Application.Utils.Payments.Model;
using QuickServe.Application.Wrappers;
using QuickServe.Domain.Settings;
using System.Threading;
using System;
using System.Threading.Tasks;
using QuickServe.Application.Interfaces;
using System.Linq;
using Azure.Core;

namespace QuickServe.WebApi.Controllers.v1
{
    [ApiVersion("1")]
    public class PaymentsController : BaseApiController
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("CreateVNPayPayment")]
        public async Task<BaseResult<Application.DTOs.Payment.PaymentResponse>> CreateVNPayPaymentAsync([FromBody] CreatePaymentCommand command)
        {
            return await Mediator.Send(command);
        }

        [HttpGet("call-back")]
        public async Task<BaseResult<PaymentCallBackResult>> CallBack()
        {
            var payment = new GetVNPayPayment()
            {
                SecureHash = Request.Query["vnp_SecureHash"],
                TransDate = Request.Query["vnp_CreateDate"],
                TxnRef = Request.Query["vnp_TxnRef"]
            };

            var result = await _paymentService.VNPayCallBackResultAsync(payment, cancellationToken: default);

            return new BaseResult<PaymentCallBackResult>(result);
        }
        
        [HttpPost("submit-order/{orderId}")]
        public async Task<BaseResult<PaymentCallBackResult>> SubmitOrder(long orderId)
        {
            var result = await _paymentService.SubmitOrder(orderId);
            return new BaseResult<PaymentCallBackResult>(result);
        }

        [HttpPost("CreatePayOS")]
        public async Task<BaseResult<Application.DTOs.Payment.PaymentResponse>> CreatePayOSAsync([FromBody] CreatePayOSCommand command)
        {
            return await Mediator.Send(command);
        }

        [HttpGet("payos-call-back")]
        public async Task<BaseResult<PaymentCallBackResult>> PayOsCallBack()
        {
            var payment = new GetPayOSResponse()
            {
                Code = Request.Query["code"],
                Cancel = Request.Query["cancel"],
                Status = Request.Query["status"],
                OrderCode = Request.Query["orderCode"],
            };

            var result = await _paymentService.PayOSCallBackResultAsync(payment, cancellationToken: default);

            return new BaseResult<PaymentCallBackResult>(result);
        }
    }
}