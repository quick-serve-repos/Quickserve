using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using QuickServe.Application.DTOs.Orders.Response;
using QuickServe.Application.Interfaces;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Wrappers;

namespace QuickServe.Application.Features.Orders.Queries.GetOrdersForStaff;

public class GetOrderForStaffQueryHandler : IRequestHandler<GetOrdersForStaffQuery, PagedResponse<OderStatusResponse>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IAuthenticatedUserService _authenticatedUserService;
    private readonly IAccountRepository _accountRepository;
    private readonly ITranslator _translator;

    public GetOrderForStaffQueryHandler(IOrderRepository orderRepository, IAuthenticatedUserService authenticatedUserService, IAccountRepository accountRepository, ITranslator translator)
    {
        _orderRepository = orderRepository;
        _authenticatedUserService = authenticatedUserService;
        _accountRepository = accountRepository;
        _translator = translator;
    }

    public async Task<PagedResponse<OderStatusResponse>> Handle(GetOrdersForStaffQuery request, CancellationToken cancellationToken)
    {
        // Retrieve current user
        var currentUser = await _accountRepository.FindByIdAsync(Guid.Parse(_authenticatedUserService.UserId));
        if (currentUser == null)
        {
            return new PagedResponse<OderStatusResponse>(new Error(ErrorCode.NotFound, _translator.GetString("Không tìm thấy tài khoản"), nameof(_authenticatedUserService.UserId)));
        }

        // Retrieve orders for the current staff user
        var result = await _orderRepository.GetOrdersForStaff(currentUser.Staff.StoreId, request.PageNumber, request.PageSize, request.Status);

        return new PagedResponse<OderStatusResponse>(result, request);
    }
}