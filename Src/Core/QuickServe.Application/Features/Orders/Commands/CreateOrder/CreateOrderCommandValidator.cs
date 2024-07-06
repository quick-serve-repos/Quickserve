using FluentValidation;

namespace QuickServe.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.Products)
            .NotEmpty().WithMessage("Products là bắt buộc.");
    }
}
