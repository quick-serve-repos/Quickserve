using FluentValidation;

namespace QuickServe.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("PhoneNumber là bắt buộc.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name là bắt buộc.");

        RuleFor(x => x.Products)
            .NotEmpty().WithMessage("Products là bắt buộc.");
    }
}
