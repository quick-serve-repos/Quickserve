using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickServe.Application.Utils.Payments.Model
{
    public class PaymentResponse
    {
        public decimal Amount { get; set; }

        public string BankCode { get; set; }

        public string Message { get; set; }

        public string OrderInfo { get; set; }

        public string CreateDate { get; set; }

        public string ResponseCode { get; set; }

        public string TransactionNo { get; set; }

        public string TransactionStatus { get; set; }

        public string TransactionType { get; set; }

        public string OrderId { get; set; }
    }
}
