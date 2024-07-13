using FluentValidation;
using QuickServe.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickServe.Application.DTOs.Ingredients.Request
{
    public class AddIngredientToSessionRequest
    {
        public long Id { get; set;}
        public int Quantity { get; set;}
    }

    public class AddIngredientToSessionRequestValidator : AbstractValidator<AddIngredientToSessionRequest>
    {
        public AddIngredientToSessionRequestValidator(ITranslator translator)
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(translator["Nguyên liệu là bắt buộc"])
                .NotNull().WithMessage(translator["Nguyên liệu là bắt buộc"]);

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage(translator["Số lượng phải lớn hơn 0"]);
        }
    }
}
