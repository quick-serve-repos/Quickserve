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
        Got =5,
        Canceled = 6,
        Refund = 7,
        Failed = 8
    }
    public class PayOSEnum
    {
        public static string PayOS_ReturnUrl = "https://quickserve-api.azurewebsites.net/api/v1/payments/payos-call-back";
    }
}
