using MediatR;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Interfaces.UserInterfaces;
using QuickServe.Application.Interfaces;
using QuickServe.Application.Wrappers;
using QuickServe.Utils.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;
using QuickServe.Application.Features.Accounts.Commands;

namespace QuickServe.Application.Features.Store.Commands.AddEmployee
{
    public class AddEmployeeCommandHandler(IAccountRepository accountRepository , ITranslator translator, IMediator mediator, IAuthenticatedUserService authenticatedUserService) : IRequestHandler<AddEmployeeCommand, BaseResult<Guid>>
    {
        public async Task<BaseResult<Guid>> Handle(AddEmployeeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var currentUser = await accountRepository.FindByIdAsync(Guid.Parse(authenticatedUserService.UserId));
                if (currentUser == null)
                {
                    return new BaseResult<Guid>(new Error(ErrorCode.NotFound, translator.GetString("Không tim thấy tài khoản"), nameof(authenticatedUserService.UserId)));
                }
                var result = await mediator.Send(new CreateAccountCommand { Email = request.Email, Name = request.Name,UserName = request.UserName, Password = request.Password, Role = AccountRole.Staff.ToString(), StoreId = currentUser.Staff.StoreId }, cancellationToken);
                if (!result.Success)
                {
                    return new BaseResult<Guid>(result.Errors);
                }

                return new BaseResult<Guid>(result.Data);
            }
            catch (Exception ex)
            {
                return new BaseResult<Guid>(new Error(ErrorCode.DatabaseCommitException, translator.GetString(ex.Message), nameof(AddEmployeeCommand)));
            }
        }
    }
}
