using QuickServe.Application.DTOs.IngredientTypeTemplateSteps.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickServe.Application.DTOs.ProductTemplates.Response
{
    public class ProductTemplateResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public List<TemplateResponse> Templates { get; set; }
        public ProductTemplateResponse() { }
    }
}
