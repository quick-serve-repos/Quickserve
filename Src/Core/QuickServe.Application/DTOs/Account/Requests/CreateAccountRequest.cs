using FluentValidation;
using QuickServe.Application.Helpers;
using QuickServe.Application.Interfaces;
using QuickServe.Utils.Enums;
using QuickServe.Utils.Helpers;

namespace QuickServe.Application.DTOs.Account.Requests
{
    public class CreateAccountRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
    }

    public class CreateAccountRequestValidator : AbstractValidator<CreateAccountRequest>
    {
        public CreateAccountRequestValidator(ITranslator translator)
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .NotNull()
                .WithName(p => translator[nameof(p.Email)]);

            RuleFor(x => x.Password)
                .NotEmpty()
                .NotNull()
                .Matches(Regexs.Password)
                .WithName(p => translator[nameof(p.Password)]);

            RuleFor(x => x.UserName)
                .NotEmpty()
                .NotNull()
                .WithName(p => translator[nameof(p.UserName)]);
            
            RuleFor(x => x.Name)
               .NotEmpty()
               .NotNull()
               .WithName(p => translator[nameof(p.Name)]);
            
            RuleFor(x => x.Role)
                .NotEmpty()
                .NotNull()
                .Must(x => EnumHelper.IsEnumValid<AccountRole>(x))
                .WithName(p => translator[nameof(p.Role)]);
        }
    }
}
