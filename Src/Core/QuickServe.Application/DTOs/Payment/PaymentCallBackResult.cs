using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickServe.Application.DTOs.Payment
{
    public class PaymentCallBackResult
    {
        public string Id { get; set; }
        public string Name { get; set; } = null!;
        public int PaymentType { get; set; }
        public string RefOrderId { get; set; }
        public int Status { get; set;}
    }
}
