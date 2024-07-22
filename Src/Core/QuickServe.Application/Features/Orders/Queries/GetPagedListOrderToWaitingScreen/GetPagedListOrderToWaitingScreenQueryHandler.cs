using MediatR;
using QuickServe.Application.DTOs.Orders.Response;
using QuickServe.Application.Interfaces;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Parameters;
using QuickServe.Application.Wrappers;
using QuickServe.Domain.Orders.Dtos;
using QuickServe.Domain.Sessions.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QuickServe.Application.Features.Orders.Queries.GetPagedListOrderToWaitingScreen
{
    public class GetPagedListOrderToWaitingScreenQueryHandler(IOrderRepository orderRepository, IAuthenticatedUserService authenticatedUserService, IAccountRepository accountRepository, ITranslator translator) : IRequestHandler<GetPagedListOrderToWaitingScreenQuery, PagedResponse<OderStatusResponse>>
    {
        public async Task<PagedResponse<OderStatusResponse>> Handle(GetPagedListOrderToWaitingScreenQuery request, CancellationToken cancellationToken)
        {
            var currentUser = await accountRepository.FindByIdAsync(Guid.Parse(authenticatedUserService.UserId));
            if (currentUser == null)
            {
                return new PagedResponse<OderStatusResponse>(new Error(ErrorCode.NotFound, translator.GetString("Không tim thấy tài khoản"), nameof(authenticatedUserService.UserId)));
            }
            var result = await orderRepository.GetOrdersToWaitingScreen(currentUser.Staff.StoreId, request.PageNumber, request.PageSize, request.Status);
            return new PagedResponse<OderStatusResponse>(result, request);
        }
    }
}
