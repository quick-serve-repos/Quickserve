using System;

namespace QuickServe.Application.Utils.Payments.Model
{
    public class VNPayOrderInfoModel
    {
        public long OrderId { get; set; }

        public string Title { get; set; }

        public long Amount { get; set; }

        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Time to generate request complete (CreateDate) GMT+7, format: yyyyMMddHHmmss
        /// </summary>
        public string VnPayCreateDate
        {
            get
            {
                return CreatedDate.VnPayFormatDate();
            }
        }

        public string BankCode { get; set; }

        public string CurrencyCode { get; set; }
    }
}
