using MediatR;
using QuickServe.Application.DTOs.Account.Responses;
using QuickServe.Application.Parameters;
using QuickServe.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickServe.Application.Features.Accounts.Queries.GetProfile
{
    public class GetProfileQuery : IRequest<BaseResult<ProfileResponse>>
    {
    }
}
