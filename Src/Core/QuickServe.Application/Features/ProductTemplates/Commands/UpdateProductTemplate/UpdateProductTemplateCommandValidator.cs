using FluentValidation;
using QuickServe.Application.Features.Ingredients.Commands.UpdateIngredient;
using QuickServe.Application.Interfaces;

namespace QuickServe.Application.Features.ProductTemplates.Commands.UpdateProductTemplate;

public class UpdateProductTemplateCommandValidator : AbstractValidator<UpdateProductTemplateCommand>
{
    public UpdateProductTemplateCommandValidator(ITranslator translator)
    {
        RuleFor(x => x.Name)
                 .NotEmpty().WithMessage(translator["Tên là bắt buộc"])
                 .NotNull().WithMessage(translator["Tên là bắt buộc"])
                 .MaximumLength(40).WithMessage(translator["Tên không vượt quá 40 ký tự"])
                 .Must(name => char.IsUpper(name[0])).WithMessage(translator["Chữ cái đầu tiên của tên phải viết hoa"])
                 .WithName(p => translator[nameof(p.Name)]);


        RuleFor(x => x.Description)
            .NotEmpty().WithMessage(translator["Mô tả là bắt buộc"])
            .MaximumLength(200).WithMessage(translator["Mô tả không được vượt quá 200 ký tự"]);

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage(translator["Category là bắt buộc"]);

        RuleFor(x => x.Size)
            .NotEmpty()
            .NotNull()
            .MaximumLength(10).WithMessage(translator["Kích thước không được vượt quá 10 ký tự"]);
    }
}