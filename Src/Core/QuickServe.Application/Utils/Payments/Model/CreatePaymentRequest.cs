using Net.payOS.Types;
using System.Collections.Generic;

namespace QuickServe.Application.Utils.Payments.Model
{
    public class CreatePaymentRequest
    {
        public long OrderCode { get; set; }
        public int Amount { get; set; }
        public string Description { get; set; }
        public List<ItemData> Items { get; set; }
        public string CancelUrl { get; set; }
        public string ReturnUrl { get; set; }
        public string? Signature { get; set; }
        public string? BuyerName { get; set; }
        public string? BuyerEmail { get; set; }
        public string? BuyerPhone { get; set; }
        public string? BuyerAddress { get; set; }
        public int? ExpiredAt { get; set; }
    }
}
