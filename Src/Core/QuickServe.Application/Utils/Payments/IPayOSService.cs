using Microsoft.AspNetCore.Http;
using Net.payOS.Types;
using QuickServe.Application.Utils.Payments.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickServe.Application.Utils.Payments
{
    public interface IPayOSService
    {
        Task<CreatePaymentResult> CreatePayment(CreatePaymentRequest request);

    }
}
