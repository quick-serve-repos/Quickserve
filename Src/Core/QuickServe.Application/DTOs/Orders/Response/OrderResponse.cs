using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickServe.Application.DTOs.Orders.Response
{
    public class OrderResponse
    {
        public string OrderId { get; set; }
        public string BillCode { get; set; }
        public int Status { get; set; }
    }
}
