using FluentValidation;
using QuickServe.Application.DTOs.Ingredients.Request;
using QuickServe.Application.Interfaces;
using QuickServe.Domain.IngredientSessions.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickServe.Application.DTOs.IngredientSessions
{
    public class CreateIngredientSessionRequest
    {
        public long SessionId { get; set; }  
        public List<AddIngredientToSessionRequest> Ingredients { get; set; }


    }
    public class CreateIngredientSessionRequestValidator : AbstractValidator<CreateIngredientSessionRequest>
    {
        public CreateIngredientSessionRequestValidator(ITranslator translator)
        {
            RuleFor(x => x.SessionId)
                .GreaterThan(0).WithMessage(translator["Ca làm việc là bắt buộc"])
                .WithName(p => translator[nameof(p.SessionId)]);

            RuleFor(x => x.Ingredients)
                .NotNull().WithMessage(translator["Danh sách nguyên liệu là bắt buộc"])
                .Must(ingredients => ingredients != null && ingredients.All(i => i != null)).WithMessage(translator["Các nguyên liệu không được null"])
                .Must(ingredients => ingredients.Select(i => i.Id).Distinct().Count() == ingredients.Count).WithMessage(translator["Các nguyên liệu không được trùng nhau"])
                .ForEach(ingredient =>
                {
                    ingredient.SetValidator(new AddIngredientToSessionRequestValidator(translator));
                });
        }
    }
 }
