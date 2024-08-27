using FluentValidation;
using QuickServe.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickServe.Application.Features.TemplateSteps.Commands.CreateTemplateStep
{
    public class CreateTemplateStepCommandValidator : AbstractValidator<CreateTemplateStepCommand>
    {
        public CreateTemplateStepCommandValidator(ITranslator translator)
        {

            RuleFor(x => x.ProductTemplateId)
                .NotEmpty().WithMessage(translator["ProductTemplateId là bắt buộc."])
                .GreaterThan(0).WithMessage(translator["ProductTemplateId phải lớn hơn 0."]);
           
            RuleFor(x => x.Name)
             .NotEmpty().WithMessage(translator["Tên là bắt buộc."])
             .Length(2, 40).WithMessage(translator["Tên phải có độ dài từ 2 đến 40 ký tự."]);

            RuleFor(x => x.IngredientTypes)
              .Must(ingredientTypes => ingredientTypes.Select(i => i.IngredientTypeId).Distinct().Count() == ingredientTypes.Count)
              .WithMessage("Mỗi loại nguyên liệu chỉ được xuất hiện một lần.");
        }
    }
}
