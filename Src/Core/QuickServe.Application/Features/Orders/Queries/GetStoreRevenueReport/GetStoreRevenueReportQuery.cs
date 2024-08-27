using MediatR;
using QuickServe.Application.DTOs.Orders.Response;
using QuickServe.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickServe.Application.Features.Orders.Queries.GetStoreRevenueReport
{
    public class GetStoreRevenueReportQuery : IRequest<BaseResult<RevenueReportDto>>
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public DateTime? SpecificDate { get; set; }
    }
}
