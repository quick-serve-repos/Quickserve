using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickServe.Application.DTOs.Orders.Response
{
    public class BestSellingReportDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<ProductTemplateDTO> BestSellingProductTemplates { get; set; }
      
    }
    public class ProductTemplateDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string UrlImage { get; set; }
        public int SellingQuantity { get; set; }
        public int TotalOrders { get; set; }
        public double TotalRevenue { get; set; }
    }
}
