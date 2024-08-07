using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickServe.Application.Utils.Payments.Model
{
    public class GetPayOSResponse
    {
        public string OrderCode { get; set; }
        public string Code { get; set; }
        public string Cancel { get; set; }
        public string Status { get; set; }
    }
}
