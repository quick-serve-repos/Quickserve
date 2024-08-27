using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using QuickServe.Application.DTOs.Payment;
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

namespace QuickServe.Application.Utils.Payments
{
    public class VNPayService : IVNPayService
    {
        private readonly AppSettings _appSettings;
        private readonly VNPaySettings _vnPaySettings;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public VNPayService(
            IOptions<AppSettings> appSettings,
            IHttpContextAccessor httpContextAccessor)
        {
            _appSettings = appSettings.Value;
            _vnPaySettings = _appSettings.PaymentSettings.VNPaySettings;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Create VNPAY payment url
        /// Docs: https://sandbox.vnpayment.vn/apis/docs/thanh-toan-pay/pay.html
        /// </summary>
        /// <param name="config"></param>
        /// <param name="order"></param>
        /// <param name="locale"></param>
        /// <returns>string: payment url</returns>
        public Task<string> CreatePaymentUrlAsync(
            VNPayConfigModel config,
            VNPayOrderInfoModel order,
            string locale,
            string returnUrl)
        {
            /// VNPay configs
            string vnpEndPoint = _vnPaySettings.VNPayUrl;
            string vnpTerminalId = config.TerminalId;
            string vnpHashSecret = config.SecretKey;

            /// VNPay config validation
            if (string.IsNullOrEmpty(vnpTerminalId) || string.IsNullOrEmpty(vnpHashSecret))
            {
                return Task.FromResult(string.Empty);
            }

            /// Build URL for VNPAY
            var vnpay = new VNPayLibrary();
            vnpay.AddRequestData("vnp_Version", _vnPaySettings.VNPayVersion);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnpTerminalId);
            /// Payment amount. Amount does not carry decimal separators, thousandths, currency characters. To send a payment amount of 100,000 VND (one hundred thousand VND), merchant needs to multiply 100 times (decimal), then send to VNPAY: 100 000 00
            /// Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 100 000 00
            vnpay.AddRequestData("vnp_Amount", (order.Amount * 100).ToString());
            if (!string.IsNullOrEmpty(order.BankCode))
            {
                vnpay.AddRequestData("vnp_BankCode", order.BankCode);
            }
            vnpay.AddRequestData("vnp_CreateDate", order.VnPayCreateDate);
            vnpay.AddRequestData("vnp_CurrCode", order.CurrencyCode);
            vnpay.AddRequestData("vnp_IpAddr", _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString());
            vnpay.AddRequestData("vnp_Locale", locale);
            vnpay.AddRequestData("vnp_OrderInfo", order.Title);
            vnpay.AddRequestData("vnp_OrderType", "other"); /// default value: other
            vnpay.AddRequestData("vnp_ReturnUrl", returnUrl);
            vnpay.AddRequestData("vnp_TxnRef", order.OrderId.ToString());

            string paymentUrl = vnpay.CreateRequestUrl(vnpEndPoint, vnpHashSecret);

            return Task.FromResult(paymentUrl);
        }

        /// <summary>
        /// Query payment status
        /// Docs: https://sandbox.vnpayment.vn/apis/docs/thanh-toan-pay/pay.html#truy-van-ket-qua-thanh-toan-PAY
        /// </summary>
        /// <param name="config"></param>
        /// <param name="orderInfo"></param>
        /// <returns></returns>
        public Task<Model.PaymentResponse> QueryAsync(VNPayConfigModel config,
            string orderId,
            string orderInfo,
            string transDate,
            DateTime createDate,
            IQueryCollection queryList
            )
        {
            var vnpayEndPoint = _vnPaySettings.BaseUrl;
            var vnpHashSecret = config.SecretKey;
            var vnpTmnCode = config.TerminalId;


            /// Build URL for VNPAY
            var vnpay = new VNPayLibrary();
            foreach (var aParameter in queryList)
            {
                if (!string.IsNullOrEmpty(aParameter.Key) && aParameter.Key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(aParameter.Key, aParameter.Value);
                }
            }

            var paymentResponse = new Model.PaymentResponse()
            {
                TransactionNo = vnpay.GetResponseData("vnp_TransactionNo"),
                TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus"),
                OrderId = orderId,
                OrderInfo = orderInfo,
                Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")),
                BankCode = vnpay.GetResponseData("vnp_BankCode"),
                ResponseCode = vnpay.GetResponseData("vnp_ResponseCode")
            };

            if (paymentResponse != null)
                return Task.FromResult(paymentResponse);
            return Task.FromResult(new Model.PaymentResponse());

        }

        /// <summary>
        /// Refund
        /// Docs: https://sandbox.vnpayment.vn/apis/docs/truy-van-hoan-tien/querydr&refund.html#hoan-tien-thanh-toan-PAY
        /// </summary>
        /// <param name="config"></param>
        /// <param name="orderId"></param>
        /// <param name="amount"></param>
        /// <param name="orderInfo"></param>
        /// <param name="transDate"></param>
        /// <param name="transactionType"></param>
        /// <param name="createBy"></param>
        /// <param name="createDate"></param>
        /// <returns></returns>
        public Task<VNPayRefundResponseModel> RefundAsync(VNPayConfigModel config, VNPayRefundRequestModel vnPayRefundModel)
        {
            var vnpayEndPoint = _vnPaySettings.BaseUrl;
            var vnpHashSecret = config.SecretKey;
            var vnpTmnCode = config.TerminalId;

            var vnpay = new VNPayLibrary();
            vnpay.AddRequestData("vnp_RequestId", $"{vnPayRefundModel.RequestId}");
            vnpay.AddRequestData("vnp_Version", _vnPaySettings.VNPayVersion);
            vnpay.AddRequestData("vnp_Command", "refund");
            vnpay.AddRequestData("vnp_TmnCode", vnpTmnCode);
            vnpay.AddRequestData("vnp_TransactionType", vnPayRefundModel.TransactionType);
            vnpay.AddRequestData("vnp_TxnRef", $"{vnPayRefundModel.TransactionId}");
            vnpay.AddRequestData("vnp_Amount", $"{vnPayRefundModel.Amount}");
            vnpay.AddRequestData("vnp_OrderInfo", $"{vnPayRefundModel.OrderInfo}");
            vnpay.AddRequestData("vnp_TransactionNo", $"{vnPayRefundModel.TransactionNo}");
            vnpay.AddRequestData("vnp_TransactionDate", $"{vnPayRefundModel.VnPayCreateDate}");
            vnpay.AddRequestData("vnp_CreateBy", $"{vnPayRefundModel.CreateBy}");
            vnpay.AddRequestData("vnp_CreateDate", $"{vnPayRefundModel.VnPayCreateDate}");

            var ipAddress = _httpContextAccessor.HttpContext.Connection.LocalIpAddress.ToString();
            vnpay.AddRequestData("vnp_IpAddr", ipAddress);

            var query_VNPAY = vnpay.CreateRequestUrl(vnpayEndPoint, vnpHashSecret);
            var request = (HttpWebRequest)WebRequest.Create(query_VNPAY);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using var httpWebResponse = (HttpWebResponse)request.GetResponse();
            using var stream = httpWebResponse.GetResponseStream();
            if (stream != null)
            {
                using var reader = new StreamReader(stream);
                var strData = reader.ReadToEnd();

                /// convert query string to json object
                var json = strData.ParseQueryStringToJson();

                var vnPayRefundResponseModel = JsonConvert.DeserializeObject<VNPayRefundResponseModel>(json);
                vnPayRefundResponseModel.vnp_URL = query_VNPAY;

                return Task.FromResult(vnPayRefundResponseModel);
            }

            return Task.FromResult(new VNPayRefundResponseModel());
        }

        /// <summary>
        /// This method is used to verify the VNPAY signature.
        /// </summary>
        /// <param name="inputHash">The value from the URL parameter vnp_SecureHash.</param>
        /// <param name="secretKey">The value from the URL parameter vnp_HashSecret.</param>
        /// <returns></returns>
        public bool ValidateSignature(IQueryCollection vnpayData, string inputHash, string secretKey)
        {
            var vnpay = new VNPayLibrary();

            foreach (var aParameter in vnpayData)
            {
                if (!string.IsNullOrEmpty(aParameter.Key) && aParameter.Key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(aParameter.Key, aParameter.Value);
                }
            }

            return vnpay.ValidateSignature(inputHash, secretKey);
        }
    }
}
