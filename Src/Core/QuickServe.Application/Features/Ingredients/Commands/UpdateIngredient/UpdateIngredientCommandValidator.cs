using FluentValidation;
using QuickServe.Application.Features.Categories.Commands.UpdateCategory;
using QuickServe.Application.Interfaces;

namespace QuickServe.Application.Features.Ingredients.Commands.UpdateIngredient;

public class UpdateIngredientCommandValidator : AbstractValidator<UpdateIngredientCommand>
{
    public UpdateIngredientCommandValidator(ITranslator translator)
    {
        RuleFor(x => x.Name)
                .NotEmpty().WithMessage(translator["Tên là bắt buộc"])
                .NotNull().WithMessage(translator["Tên là bắt buộc"])
                .MaximumLength(40).WithMessage(translator["Tên không được vượt quá 40 ký tự"])
                .Must(name => char.IsUpper(name[0])).WithMessage(translator["Chữ cái đầu tiên của Tên phải viết hoa"])
                .WithName(p => translator[nameof(p.Name)]);

        RuleFor(p => p.Price)
            .GreaterThan(0).WithMessage(translator["Giá phải lớn hơn 0"])
            .WithName(p => translator[nameof(p.Price)]);

        RuleFor(p => p.Calo)
            .GreaterThanOrEqualTo(0).WithMessage(translator["Calo phải lớn hơn hoặc bằng 0"])
            .WithName(p => translator[nameof(p.Calo)]);

        RuleFor(p => p.Description)
            .NotEmpty().WithMessage(translator["Mô tả là bắt buộc"])
            .MaximumLength(200).WithMessage(translator["Mô tả không được vượt quá 200 ký tự"])
            .WithName(p => translator[nameof(p.Description)]);

        RuleFor(p => p.IngredientTypeId)
            .GreaterThan(0).WithMessage(translator["Loại nguyên liệu phải lớn hơn 0"])
            .WithName(p => translator[nameof(p.IngredientTypeId)]);
    }
}