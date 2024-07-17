using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickServe.Application.DTOs.Orders.Response
{
    public class RevenueReportDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double TotalRevenue { get; set; }
        public double SpecificRevenue { get; set; }
        public int TotalOrderCount { get; set; } 
        public int SpecificOrderCount { get; set; }
    }

}
