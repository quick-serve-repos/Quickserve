using FluentValidation;
using QuickServe.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickServe.Application.Features.Sessions.Queries.GetSessionById
{
    public class GetSessionByIdQueryValidator : AbstractValidator<GetSessionByIdQuery>
    {
        public GetSessionByIdQueryValidator(ITranslator translator)
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage(translator["Id phải lớn hơn 0"])
                .WithName(p => translator[nameof(p.Id)]);
        }
    }
}
