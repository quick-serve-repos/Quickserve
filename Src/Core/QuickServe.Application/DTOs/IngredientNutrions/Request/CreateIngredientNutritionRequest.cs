using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickServe.Application.DTOs.IngredientNutrions.Request
{
    public class CreateIngredientNutritionRequest
    {
        public long IngredientId { get; set; }
        public List<long> NutritionIds { get; set; }
    }

    public class CreateIngredientNutritionRequestValidator : AbstractValidator<CreateIngredientNutritionRequest>
    {
        public CreateIngredientNutritionRequestValidator()
        {
            RuleFor(x => x.NutritionIds)
                .NotEmpty().WithMessage("Danh sách thành phần dinh dưỡng là bắt buộc.")
                .Must(nutritionIds => nutritionIds.Distinct().Count() == nutritionIds.Count)
                .WithMessage("Mỗi thành phần dinh dưỡng chỉ được xuất hiện một lần.");

            RuleForEach(x => x.NutritionIds)
                .GreaterThan(0).WithMessage("Thành phần dinh dưỡng phải lớn hơn 0.");
        }
    }
}
