using FluentValidation;
using QuickServe.Application.Interfaces;

namespace QuickServe.Application.Features.Nutritions.Commands.UpdateNutrition;

public class UpdateNutritionCommandValidator : AbstractValidator<UpdateNutritionCommand>
{
    public UpdateNutritionCommandValidator(ITranslator translator)
    {
        RuleFor(x => x.Name)
                    .NotEmpty().WithMessage(translator["Tên là bắt buộc."])
                    .Must(name => char.IsUpper(name[0])).WithMessage(translator["Chữ cái đầu tiên của tên phải viết hoa"])
                    .Length(2, 40).WithMessage(translator["Tên phải từ 2 đến 40 ký tự."]);
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage(translator["Mô tả là bắt buộc."])
            .MaximumLength(100).WithMessage(translator["Mô tả không được vượt quá 100 ký tự."]);
        RuleFor(x => x.Vitamin)
           .NotEmpty().WithMessage(translator["Viatamin là bắt buộc."])
           .Length(2, 40).WithMessage(translator["Tên phải từ 2 đến 40 ký tự."]);
        RuleFor(x => x.HealthValue)
           .NotEmpty().WithMessage(translator["Tên là bắt buộc."])
           .Length(2, 100).WithMessage(translator["Tên phải từ 2 đến 100 ký tự."]);
    }
}