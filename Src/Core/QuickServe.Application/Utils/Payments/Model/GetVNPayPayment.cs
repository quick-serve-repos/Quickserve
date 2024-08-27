using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickServe.Application.Utils.Payments.Model
{
    public class GetVNPayPayment
    {
        public string TransDate { get; set; }

        public string TxnRef { get; set; }

        public string SecureHash { get; set; }
    }
}
