using MediatR;
using QuickServe.Application.DTOs.Account.Responses;
using QuickServe.Application.Interfaces.UserInterfaces;
using QuickServe.Application.Wrappers;
using QuickServe.Domain.Accounts.Dtos;
using System.Threading;
using System.Threading.Tasks;

namespace QuickServe.Application.Features.Accounts.Queries.GetAccountById;

public class GetAccountByIdQueryHandlers(IAccountServices accountServices) : IRequestHandler<GetAccountByIdQuery, BaseResult<ProfileResponse>>
{
    public async Task<BaseResult<ProfileResponse>> Handle(GetAccountByIdQuery request, CancellationToken cancellationToken)
    {
        return await accountServices.GetAccountById(request.Id);
    }
}