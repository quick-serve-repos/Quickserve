using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickServe.Application.DTOs.IngredientSessions
{
    public class DeleteIngredientInSessionRequest
    {
        public long IngredientId { get; set; }
    }
    public class DeleteIngredientInSessionRequestValidator : AbstractValidator<DeleteIngredientInSessionRequest>
    {
        public DeleteIngredientInSessionRequestValidator()
        {
            RuleFor(x => x.IngredientId)
                .GreaterThan(0).WithMessage("Id nguyên liệu phải lớn hơn 0.");
        }
    }
}
