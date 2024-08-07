using Microsoft.AspNetCore.Http;
using Net.payOS.Types;
using QuickServe.Application.DTOs.Payment;
using QuickServe.Application.Features.Payments.Commands.CreatePayment;
using QuickServe.Application.Utils.Payments.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QuickServe.Application.Interfaces
{
    public interface IPaymentService
    {
        /// <summary>
        /// Create VNPay Payment
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<string> CreateVNPayPaymentUrlAsync(CreatePaymentCommand request, CancellationToken cancellationToken);
        /// <summary>
        /// Get Query Payment call-back
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Utils.Payments.Model.PaymentResponse> GetVNPayPaymentAsync(GetVNPayPayment request, CancellationToken cancellationToken);
        Task<PaymentCallBackResult> VNPayCallBackResultAsync(GetVNPayPayment request, CancellationToken cancellationToken);
        Task<PaymentCallBackResult> SubmitOrder(long orderId);
        Task<string> CreatePayOSPaymentAsync(CreatePaymentRequest request, CancellationToken cancellationToken);
        Task<PaymentCallBackResult> PayOSCallBackResultAsync(GetPayOSResponse request, CancellationToken cancellationToken);
    }
}
