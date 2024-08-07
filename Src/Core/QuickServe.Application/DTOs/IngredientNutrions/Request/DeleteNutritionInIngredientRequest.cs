using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickServe.Application.DTOs.IngredientNutrions.Request
{
    public class DeleteNutritionInIngredientRequest
    {
        public long NutritionId { get; set; }
    }
    public class DeleteNutritionInIngredientRequestValidator : AbstractValidator<DeleteNutritionInIngredientRequest>
    {
        public DeleteNutritionInIngredientRequestValidator()
        {
            RuleFor(x => x.NutritionId)
                .GreaterThan(0).WithMessage("Thành phần dinh dưỡng phải lớn hơn 0.");
        }
    }
}
