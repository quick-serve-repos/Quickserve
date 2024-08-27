using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickServe.Application.DTOs.IngredientTypes.Request
{
    public class IngredientTypeTemplates
    {
        public int IngredientTypeId { get; set; }
        public int QuantityMin { get; set; }
        public int QuantityMax { get; set; }
    }
    public class IngredientTypeTemplateStepsValidator : AbstractValidator<IngredientTypeTemplates>
    {
        public IngredientTypeTemplateStepsValidator()
        {
            RuleFor(x => x.IngredientTypeId)
                 .NotEmpty().WithMessage("Loại nguyên liệu là bắt buộc.")
                 .GreaterThan(0).WithMessage("Loại nguyên liệu phải lớn hơn 0.");

            RuleFor(x => x.QuantityMin)
                .GreaterThanOrEqualTo(0).WithMessage("Số lượng tối thiểu phải lớn hơn hoặc bằng 0.");

            RuleFor(x => x.QuantityMax)
                .GreaterThan(0).WithMessage("Số lượng tối đa phải lớn hơn 0.")
                .GreaterThanOrEqualTo(x => x.QuantityMin).WithMessage("Số lượng tối đa phải lớn hơn hoặc bằng số lượng tối thiểu.");
        }
    }
}
