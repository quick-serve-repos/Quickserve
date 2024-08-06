using QuickServe.Application.DTOs;
using QuickServe.Application.DTOs.Account.Requests;
using QuickServe.Application.DTOs.Account.Responses;
using QuickServe.Application.Features.Accounts.Commands.DeleteAccount;
using QuickServe.Application.Features.Accounts.UpdateProfile;
using QuickServe.Application.Wrappers;
using QuickServe.Domain.Accounts.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuickServe.Application.Interfaces.UserInterfaces
{
    public interface IAccountServices
    {
        Task<BaseResult<string>> RegisterGostAccount();
        Task<BaseResult> ChangePassword(ChangePasswordRequest model);
        Task<BaseResult> ChangeUserName(ChangeUserNameRequest model);
        Task<BaseResult<AuthenticationResponse>> Authenticate(AuthenticationRequest login);
        Task<BaseResult> DeleteAccount(DeleteAccountCommand request);
        Task<BaseResult> UpdateProfile(UpdateProfileCommand request);
        Task<BaseResult<AuthenticationResponse>> AuthenticateByUserName(string username);
        Task<BaseResult<TokenDto>> RefreshToken(TokenDto token);
        Task<BaseResult<Guid>> CreateAccount(CreateAccountRequest request);
        Task<PagenationResponseDto<AccountDto>> GetPagedListAsync(int pageNumber, int pageSize, string name, string[] roles);
        Task<BaseResult<ProfileResponse>> GetAccountById(Guid id);
        Task<BaseResult<AccountDto>> FindByEmailAsync(string email);
    }
}
