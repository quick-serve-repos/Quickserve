using FluentValidation;
using QuickServe.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickServe.Application.Features.Sessions.Commands.DeleteSession
{
    public class DeleteSessionCommandValidator : AbstractValidator<DeleteSessionCommand>
    {
        public DeleteSessionCommandValidator(ITranslator translator)
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage(translator["Id phải lớn hơn 0"])
                .WithName(p => translator[nameof(p.Id)]);
        }
    }
}
