using FluentValidation;

namespace QuickServe.Application.Features.Orders.Commands.UpdateOrder;

public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
{
    public UpdateOrderCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("OrderId là bắt buộc.");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status là bắt buộc.");
    }
}