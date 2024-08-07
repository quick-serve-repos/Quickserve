using FluentValidation;
using QuickServe.Application.DTOs.IngredientTypes.Request;
using QuickServe.Application.Features.TemplateSteps.Commands.CreateTemplateStep;
using QuickServe.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickServe.Application.DTOs.IngredientTypeTemplateSteps.Request
{
    public class TemplateStepRequest
    {
        public string Name { get; set; }
        public List<IngredientTypeTemplates> IngredientTypes { get; set; }
    }
    public class CreateTemplateStepRequestValidator : AbstractValidator<TemplateStepRequest>
    {
        public CreateTemplateStepRequestValidator(ITranslator translator)
        {
            RuleFor(x => x.Name)
                 .NotEmpty().WithMessage(translator["Tên là bắt buộc."])
                 .Length(2, 40).WithMessage(translator["Tên phải có độ dài từ 2 đến 40 ký tự."]);
           
            RuleFor(x => x.IngredientTypes)
              .Must(ingredientTypes => ingredientTypes.Select(i => i.IngredientTypeId).Distinct().Count() == ingredientTypes.Count)
              .WithMessage("Mỗi loại nguyên liệu chỉ được xuất hiện một lần.");
        }
    }
}
