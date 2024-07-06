using System;
using System.Collections.Generic;
using System.Text;

namespace QuickServe.Domain.Settings
{
    public class AppSettings
    {
        public PaymentSettings PaymentSettings { get; set; }
    }

    public class PaymentSettings
    {
        public VNPaySettings VNPaySettings { get; set; }
    }

    public class VNPaySettings
    {
        /// <summary>
        /// VNPAY payment endpoint
        /// </summary>
        public string VNPayUrl { get; set; }

        /// <summary>
        /// VNPAY query or refund endpoint
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// VNPAY version
        /// </summary>
        public string VNPayVersion { get; set; }

        /// <summary>
        /// TerminalId
        /// </summary>
        public string TerminalId { get; set; }

        /// <summary>
        /// SecretKey
        /// </summary>
        public string SecretKey { get; set; }

        public string CallBackUrl { get; set; }
    }
}
