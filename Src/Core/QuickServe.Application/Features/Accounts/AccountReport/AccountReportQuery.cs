using MediatR;
using QuickServe.Application.DTOs.Account.Responses;
using QuickServe.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickServe.Application.Features.Accounts.AccountReport
{
    public class AccountReportQuery : IRequest<BaseResult<AccountReportDto>>
    {
        public DateTime? SpecificDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public long? StoreId { get; set; }
    }
}
