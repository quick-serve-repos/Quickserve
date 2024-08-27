using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickServe.Application.DTOs.IngredientTypeTemplateSteps.Response
{
    public class GetProductTemplateResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public List<TemplateStepResponse> Steps { get; set; }
    }
    public class TemplateStepResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public List<IngredientTypeDto> IngredientTypes { get; set; }
    }
    public class IngredientTypeDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int QuantityMin { get; set; }
        public int QuantityMax { get; set; }
    }
}
