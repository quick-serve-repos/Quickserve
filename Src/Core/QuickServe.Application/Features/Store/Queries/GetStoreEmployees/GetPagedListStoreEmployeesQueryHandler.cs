using MediatR;
using QuickServe.Application.Interfaces;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Wrappers;
using QuickServe.Domain.Stores.Dtos;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace QuickServe.Application.Features.Store.Queries.GetStoreEmployees
{
    public class GetPagedListStoreEmployeesQueryHandler(IAccountRepository accountRepository, IAuthenticatedUserService authenticatedUserService, IStaffRepository staffRepository, ITranslator translator) : IRequestHandler<GetPagedListStoreEmployeesQuery, PagedResponse<EmployeeDto>>
    {
        public async Task<PagedResponse<EmployeeDto>> Handle(GetPagedListStoreEmployeesQuery request, CancellationToken cancellationToken)
        {
            var currentUser = await accountRepository.FindByIdAsync(Guid.Parse(authenticatedUserService.UserId));
            if (currentUser == null)
            {
                return new PagedResponse<EmployeeDto>(new Error(ErrorCode.NotFound, translator.GetString("Không tim thấy tài khoản"), nameof(authenticatedUserService.UserId)));
            }
            var result =  await staffRepository.GetPagedListStaffByStoreIdAsync(currentUser.Staff.StoreId, request.PageNumber, request.PageSize, request.Name, cancellationToken, request.Roles);
            return new PagedResponse<EmployeeDto>(result, request);
        }
    }
}
