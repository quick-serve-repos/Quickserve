using System;

namespace QuickServe.Domain.Payments.Dtos
{
    public class PaymentDto
    {
        public long Id { get; set; }
        public DateTime Created { get; set; }
        public string PaymentType { get; set; }
        public string RefOrderId { get; set; }
        public double Amount { get; set; }
        public long StoreId { get; set; }
        public int Status { get; set; }
    }
}