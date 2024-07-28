using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickServe.Application.Utils.Enums
{
    public enum OrderStatus
    {
        Pending = 1,
        Paided = 2,
        Preparing = 3,
        Success = 4,
        Failed = 5,
    }
    public class PayOSEnum
    {
        public static string PayOS_ReturnUrl = "https://localhost:7233/api/v1/payments/payos-call-back";
    }
}
