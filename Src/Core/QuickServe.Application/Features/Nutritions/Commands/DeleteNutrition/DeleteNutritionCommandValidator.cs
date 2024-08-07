using FluentValidation;
using QuickServe.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickServe.Application.Features.Nutritions.Commands.DeleteNutrition
{
    public class DeleteNutritionCommandValidator : AbstractValidator<DeleteNutritionCommand>
    {
        public DeleteNutritionCommandValidator(ITranslator translator)
        {
            RuleFor(p => p.Id)
                .NotNull()
                .NotEmpty()
                .GreaterThan(0)
                .WithMessage(p => translator["ID không được để trống và phải lớn hơn 0"])
                .WithName(p => translator[nameof(p.Id)]);
        }
    }

}
