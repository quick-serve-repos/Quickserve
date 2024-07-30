using MediatR;
using QuickServe.Application.DTOs.Account.Responses;
using QuickServe.Application.Interfaces;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Interfaces.UserInterfaces;
using QuickServe.Application.Wrappers;
using QuickServe.Domain.Stores.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QuickServe.Application.Features.Accounts.Queries.GetProfile
{
    public class GetProfileQueryHandler(IAccountServices accountServices, IAccountRepository accountRepository
        , IAuthenticatedUserService authenticatedUserService, ITranslator translator) : IRequestHandler<GetProfileQuery, BaseResult<ProfileResponse>>
    {
        public async Task<BaseResult<ProfileResponse>> Handle(GetProfileQuery request, CancellationToken cancellationToken)
        {
            var currentUser = await accountRepository.FindByIdAsync(Guid.Parse(authenticatedUserService.UserId));
            if (currentUser == null)
            {
                return new BaseResult<ProfileResponse>(new  Error(ErrorCode.NotFound, translator.GetString("Không tìm thấy tài khoản")));
            }
            return await accountServices.GetAccountById(currentUser.Id);
        }
    }
}
