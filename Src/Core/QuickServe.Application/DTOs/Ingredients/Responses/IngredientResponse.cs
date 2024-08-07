using QuickServe.Application.DTOs.Nutritions.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickServe.Application.DTOs.Ingredients.Responses
{
    public class IngredientResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public List<NutritionResponse> Nutritions { get; set; }
    }
}
