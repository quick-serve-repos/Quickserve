using QuickServe.Application.Utils.Enums;
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
        public List<MonthlyRevenueDto> MonthlyRevenues { get; set; }
        public List<YearlyRevenueDto> YearlyRevenues { get; set; }
        public Dictionary<OrderStatus, int> OrderStatusCounts { get; set; }
    }
    public class MonthlyRevenueDto
    {
        public int Month { get; set; }
        public int OrderCount { get; set; }
        public double Revenue { get; set; }
    }

    public class YearlyRevenueDto
    {
        public int Year { get; set; }
        public int OrderCount { get; set; }
        public double Revenue { get; set; }
    }
}
