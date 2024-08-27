using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using QuickServe.Application.Utils.Payments.Model;
using QuickServe.Domain.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using QuickServe.Utils.Helpers;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Web;
using MediatR;
using Net.payOS;
using Net.payOS.Types;

namespace QuickServe.Application.Utils.Payments
{
    public class PayOSService : IPayOSService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly PayOS _payOS;

        public PayOSService(
            IHttpContextAccessor httpContextAccessor,
            PayOS payOS)
        {
            _httpContextAccessor = httpContextAccessor;
            _payOS = payOS;
        }

        public async Task<CreatePaymentResult> CreatePayment(CreatePaymentRequest request)
        {
            PaymentData paymentData = new PaymentData(request.OrderCode, request.Amount, request.Description, request.Items, request.CancelUrl, request.ReturnUrl, request.Signature, request.BuyerName, request.BuyerEmail, request.BuyerPhone, request.BuyerAddress, request.ExpiredAt);

            CreatePaymentResult createPayment = await _payOS.createPaymentLink(paymentData);

            return createPayment;
        }
    }
}
