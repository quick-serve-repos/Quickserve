using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickServe.Application.DTOs.IngredientTypeTemplateSteps.Response
{
    public class GetIngredientsResponse
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public int DefaultQuantity { get; set; }
        public int QuantityMax { get; set; }
        public string ImageUrl { get; set; } = null!;
        public bool IsSold { get; set; }
        public int RemainingQuantity { get; set; }
    }

}
