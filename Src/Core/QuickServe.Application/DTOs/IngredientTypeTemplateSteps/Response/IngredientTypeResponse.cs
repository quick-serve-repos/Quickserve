using QuickServe.Application.DTOs.Ingredients.Responses;
using QuickServe.Domain.IngredientTypeTemplateSteps.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickServe.Application.DTOs.IngredientTypeTemplateSteps.Response
{
    public class IngredientTypeResponse
    {
        public long IngredientTypeId { get; set; }
        public string Name { get; set; }
        public int QuantityMin { get; set; }
        public int QuantityMax { get; set; }
        public List<IngredientInfoResponse> Ingredients { get; set; }
        public IngredientTypeResponse() { }
        public IngredientTypeResponse(IngredientTypeTemplateStep ingredientTypeTemplateStep)
        {
            IngredientTypeId = ingredientTypeTemplateStep.IngredientTypeId;
            QuantityMin = ingredientTypeTemplateStep.QuantityMin;
            QuantityMax = ingredientTypeTemplateStep.QuantityMax;
            Name = ingredientTypeTemplateStep.IngredientType.Name;
        }
    }
}
