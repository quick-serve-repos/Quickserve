using FluentValidation;
using QuickServe.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickServe.Application.Features.Accounts.Commands.RegisterCustomerAccount
{
    public class RegisterCustomerAccountCommandValidator : AbstractValidator<RegisterCustomerAccountCommand>
    {
        public RegisterCustomerAccountCommandValidator(ITranslator translator)
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(translator["Email là bắt buộc"])
                .NotNull().WithMessage(translator["Email là bắt buộc"])
                .EmailAddress().WithMessage(translator["Email không hợp lệ"]);

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(translator["Mật khẩu là bắt buộc"])
                .NotNull().WithMessage(translator["Mật khẩu là bắt buộc"])
                .MinimumLength(6).WithMessage(translator["Mật khẩu phải có ít nhất 6 ký tự"]);

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage(translator["Tên người dùng là bắt buộc"])
                .NotNull().WithMessage(translator["Tên người dùng là bắt buộc"])
                .MaximumLength(40).WithMessage(translator["Tên người dùng không được vượt quá 40 ký tự"]);

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(translator["Tên là bắt buộc"])
                .NotNull().WithMessage(translator["Tên là bắt buộc"])
                .MaximumLength(40).WithMessage(translator["Tên không được vượt quá 40 ký tự"]);

          

            RuleFor(x => x.Name)
                .Must(name => char.IsUpper(name[0])).WithMessage(translator["Chữ cái đầu tiên của tên phải là chữ in hoa"])
                .WithName(p => translator[nameof(p.Name)]);
        }
    }
}
