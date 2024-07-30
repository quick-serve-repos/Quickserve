using MediatR;
using QuickServe.Application.Helpers;
using QuickServe.Application.Interfaces;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Interfaces.UserInterfaces;
using QuickServe.Application.Utils.Enums;
using QuickServe.Application.Wrappers;
using QuickServe.Domain.Accounts.Entities;
using QuickServe.Utils.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace QuickServe.Application.Features.Accounts.Commands
{
    public class CreateAccountCommandHandler(IAuthenticatedUserService authenticated, IAccountServices accountServices, IStaffRepository staffRepository ,IStoreRepository storeRepository,IAccountRepository accountRepository, ITranslator translator) : IRequestHandler<CreateAccountCommand, BaseResult<Guid>>
    {
        public async Task<BaseResult<Guid>> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            var currentUser = await accountRepository.FindByIdAsync(Guid.Parse(authenticated.UserId));
            if (currentUser == null)
            {
                return new BaseResult<Guid>(new Error(ErrorCode.NotFound, translator.GetString("Không tim thấy tài khoản"), nameof(authenticated.UserId)));
            }
            var result = await accountServices.CreateAccount(new DTOs.Account.Requests.CreateAccountRequest
            {
                Email = request.Email,
                Password = request.Password,
                Role = request.Role,
                UserName = request.UserName,
                Name = request.Name
            });
            if (result.Success)
            {
                var account = new Account
                {
                    Email = request.Email,
                    UserName = request.UserName,
                    Id = result.Data,
                    Name = request.Name, 
                    Status =(int) AccountStatus.Active,
                    CreatedBy = authenticated.UserName
                    
                };

                await accountRepository.AddAsync(account);
                if (request.Role == AccountRole.Staff.ToString() ||
                    request.Role == AccountRole.Store_Manager.ToString()
                   )
                {
                    if(request.Role == AccountRole.Store_Manager.ToString())
                    {
                        var store = await storeRepository.GetByIdAsync(request.StoreId);
                        
                        if (store == null)
                        {
                            throw new Exception(translator
                                .GetString(TranslatorMessages.StoreMessages.Không_tìm_thấy_cửa_hàng(request.StoreId)));
                        }
                        if(store.StoreManager != null)
                        {
                            throw new Exception("Cửa hàng đã có quản lý");
                        }
                        store.StoreManager = request.UserName;
                    }
                    staffRepository.AddStaffToStore(request.StoreId, account.Id);
                }
               
                return new BaseResult<Guid>(account.Id);
            }
            return new BaseResult<Guid>(result.Errors);
        }
    }
}
