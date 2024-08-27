using MediatR;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Wrappers;
using QuickServe.Domain.ProductTemplates.Dtos;
using System.Threading.Tasks;
using System.Threading;
using QuickServe.Domain.Sessions.Dtos;
using QuickServe.Application.Interfaces;
using System;

namespace QuickServe.Application.Features.Sessions.Queries.GetPagedListSession;

public class GetPagedListSessionQueryHandler(ISessionRepository sessionRepository, IAuthenticatedUserService authenticatedUserService, IAccountRepository accountRepository, ITranslator translator) : IRequestHandler<GetPagedListSessionQuery, PagedResponse<SessionDto>>
{
    public async Task<PagedResponse<SessionDto>> Handle(GetPagedListSessionQuery request, CancellationToken cancellationToken)
    {
        var currentUser = await accountRepository.FindByIdAsync(Guid.Parse(authenticatedUserService.UserId));
        if (currentUser == null)
        {
            return new PagedResponse<SessionDto>(new Error(ErrorCode.NotFound, translator.GetString("Không tim thấy tài khoản"), nameof(authenticatedUserService.UserId)));
        }
        var result = await sessionRepository.GetPagedListAsyncByStore(currentUser.Staff.StoreId ,request.PageNumber, request.PageSize, request.Name);

        return new PagedResponse<SessionDto>(result, request);
    }
}