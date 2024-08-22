    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.JsonWebTokens;
    using Microsoft.IdentityModel.Tokens;
    using QuickServe.Application.DTOs;
    using QuickServe.Application.DTOs.Account.Requests;
    using QuickServe.Application.DTOs.Account.Responses;
using QuickServe.Application.DTOs.Ingredients.Request;
using QuickServe.Application.Features.Accounts.Commands.DeleteAccount;
    using QuickServe.Application.Features.Accounts.UpdateProfile;
    using QuickServe.Application.Helpers;
    using QuickServe.Application.Interfaces;
using QuickServe.Application.Interfaces.ImageInterfaces;
using QuickServe.Application.Interfaces.Repositories;
    using QuickServe.Application.Interfaces.UserInterfaces;
    using QuickServe.Application.Utils.Enums;
    using QuickServe.Application.Wrappers;
    using QuickServe.Domain.Accounts.Dtos;
    using QuickServe.Infrastructure.Identity.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using Error = QuickServe.Application.Wrappers.Error;

    namespace QuickServe.Infrastructure.Identity.Services
    {
        public class AccountServices(UserManager<ApplicationUser> userManager, IAuthenticatedUserService authenticatedUser, ITranslator translator, IConfiguration configuration, IAccountRepository accountRepository, IUnitOfWork unitOfWork, ICustomerRepository customerRepository, IImageService imageService) : IAccountServices
        {

            public async Task<BaseResult> ChangePassword(ChangePasswordRequest model)
            {
                var user = await userManager.FindByIdAsync(authenticatedUser.UserId);

                var token = await userManager.GeneratePasswordResetTokenAsync(user);

                var identityResult = await userManager.ResetPasswordAsync(user, token, model.Password);

                if (identityResult.Succeeded)
                    return new BaseResult();

                return new BaseResult(identityResult.Errors.Select(p => new Error(ErrorCode.ErrorInIdentity, p.Description)));
            }

            public async Task<BaseResult> ChangeUserName(ChangeUserNameRequest model)
            {
                var user = await userManager.FindByIdAsync(authenticatedUser.UserId);

                user.UserName = model.UserName;

                var acc = await accountRepository.FindByIdAsync(user.Id);
                if (await accountRepository.ExistByUsername(model.UserName) && acc.UserName.Trim() != model.UserName.Trim())
                {
                    return new BaseResult(new Error(ErrorCode.FieldDataInvalid, translator.GetString("Tên người dùng đã tồn tại.")));
                }
                var identityResult = await userManager.UpdateAsync(user);

                if (identityResult.Succeeded)
                {
                    acc.UserName = model.UserName;
                    acc.CreatedBy = acc.UserName;
                    acc.LastModified = DateTime.Now;
                    acc.LastModifiedBy = acc.UserName;
                    accountRepository.Update(acc);
                    return new BaseResult();
                }
                
                return new BaseResult(identityResult.Errors.Select(p => new Error(ErrorCode.ErrorInIdentity, p.Description)));
            }

            public async Task<BaseResult<AuthenticationResponse>> Authenticate(AuthenticationRequest login)
            {
                var user = await userManager.FindByEmailAsync(login.Email);
                if (user == null)
                {
                    return new BaseResult<AuthenticationResponse>(new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.AccountMessages.Tài_khoản_không_tìm_thấy_với_Email(login.Email)), nameof(login.Email)));
                }
                //var account = await accountRepository.FindByIdAsync(user.Id);
                //if(account.Status != (int) AccountStatus.Active)
                //{
                //    return new BaseResult<AuthenticationResponse>(new Error(
                //        ErrorCode.AccountLocked, translator.GetString("Tài khoản đã bị khoá hoặc không hoạt động")));
                //}
                var result = await userManager.CheckPasswordAsync(user, login.Password);
                if (!result)
                {
                    return new BaseResult<AuthenticationResponse>(new Error(ErrorCode.FieldDataInvalid, translator.GetString(TranslatorMessages.AccountMessages.Mật_khẩu_không_hợp_lệ()), nameof(login.Password)));
                }

                var rolesList = await userManager.GetRolesAsync(user).ConfigureAwait(false);

                var token = await CreateToken(user, true);

                AuthenticationResponse response = new AuthenticationResponse()
                {
                    Id = user.Id.ToString(),
                    AccessToken = token.AccessToken,
                    RefreshToken = token.RefreshToken,
                    Email = user.Email,
                    UserName = user.UserName,
                    Roles = rolesList.FirstOrDefault(),
                    IsVerified = user.EmailConfirmed,
                };

                return new BaseResult<AuthenticationResponse>(response);
            }

            public async Task<BaseResult<AuthenticationResponse>> AuthenticateByUserName(string username)
            {
                var user = await userManager.FindByNameAsync(username);
                if (user == null)
                {
                    return new BaseResult<AuthenticationResponse>(new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.AccountMessages.Tài_khoản_không_tìm_thấy_với_UserName(username)), nameof(username)));
                }

                var rolesList = await userManager.GetRolesAsync(user).ConfigureAwait(false);

                var token = await CreateToken(user, true);

                AuthenticationResponse response = new AuthenticationResponse()
                {
                    Id = user.Id.ToString(),
                    AccessToken = token.AccessToken,
                    RefreshToken = token.RefreshToken,
                    Email = user.Email,
                    UserName = user.UserName,
                    Roles = rolesList.FirstOrDefault(),
                    IsVerified = user.EmailConfirmed,
                };

                return new BaseResult<AuthenticationResponse>(response);
            }

            public async Task<BaseResult<string>> RegisterGostAccount()
            {
                var user = new ApplicationUser()
                {
                    UserName = GenerateRandomString(7)
                };

                var identityResult = await userManager.CreateAsync(user);

                if (identityResult.Succeeded)
                    return new BaseResult<string>(user.UserName);

                return new BaseResult<string>(identityResult.Errors.Select(p => new Error(ErrorCode.ErrorInIdentity, p.Description)));

                string GenerateRandomString(int length)
                {
                    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                    var random = new Random();
                    var result = new StringBuilder(length);

                    for (int i = 0; i < length; i++)
                    {
                        int index = random.Next(chars.Length);
                        result.Append(chars[index]);
                    }

                    return result.ToString();
                }
            }

            public async Task<TokenDto> CreateToken(ApplicationUser user, bool populateExp)
            {
                var signingCredentials = GetSigningCredentials();
                var claims = await GetClaims(user);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddMinutes(double.Parse(configuration["JWTSettings:DurationInMinutes"])),
                    SigningCredentials = signingCredentials,
                    Audience = configuration["JWTSettings:Audience"],
                    Issuer = configuration["JWTSettings:Issuer"],
                    TokenType = "Bearer",
                };

                var refreshToken = GenerateRefreshToken();

                user.RefreshToken = refreshToken;
                if (populateExp)
                    user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

                await userManager.UpdateAsync(user);
                await userManager.UpdateSecurityStampAsync(user);


                var tokenHandler = new JsonWebTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return new TokenDto
                {
                    AccessToken = token,
                    RefreshToken = refreshToken
                };
            }

            private SigningCredentials GetSigningCredentials()
            {
                return new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWTSettings:Key"])), SecurityAlgorithms.HmacSha256);
            }

            private async Task<List<Claim>> GetClaims(ApplicationUser user)
            {
                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new(ClaimTypes.Name, user.UserName),
                    new(ClaimTypes.Email, user.Email),
                    new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new(ClaimTypes.Role, string.Join(",", await userManager.GetRolesAsync(user))),
                    new(ClaimTypes.AuthenticationMethod, "Bearer"),
                    new(ClaimTypes.Expiration, DateTime.UtcNow.AddMinutes(double.Parse(configuration["JWTSettings:DurationInMinutes"])).ToString()),
                };

                return claims;
            }


            private string GenerateRefreshToken()
            {
                var random = new byte[32];
                using var generator = RandomNumberGenerator.Create();
                generator.GetBytes(random);
                return Convert.ToBase64String(random);
            }

            private async Task<ClaimsIdentity> GetPrincipalFromExpiredToken(string token)
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWTSettings:Key"])),
                    ValidateLifetime = true,
                    ValidIssuer = configuration["JWTSettings:Issuer"],
                    ValidAudience = configuration["JWTSettings:Audience"]
                };

                var tokenHandler = new JsonWebTokenHandler();
                var securityToken = await tokenHandler.ValidateTokenAsync(token, tokenValidationParameters);
                if (!securityToken.IsValid)
                {
                    throw new SecurityTokenException("Invalid token");
                }

                return securityToken.ClaimsIdentity;
            }

            public async Task<BaseResult<TokenDto>> RefreshToken(TokenDto token)
            {
                try
                {
                    var principal = await GetPrincipalFromExpiredToken(token.AccessToken);

                    var user = await userManager.FindByNameAsync(principal.Name);
                    if (user == null || user.RefreshToken != token.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                        throw new SecurityTokenException("Invalid token");
                    var newToken = await CreateToken(user, false);
                    return new BaseResult<TokenDto>(newToken);
                }
                catch (Exception ex)
                {
                    return new BaseResult<TokenDto>(new Error(ErrorCode.ErrorInIdentity, ex.Message));
                }

            }

            public async Task<BaseResult<Guid>> CreateAccount(CreateAccountRequest request)
            {
                // Assgin role to exist account
                var existUser = await userManager.FindByEmailAsync(request.Email);
                if (existUser != null)
                {
                    return new BaseResult<Guid>(new Error(ErrorCode.Duplicate, translator.GetString(TranslatorMessages.AccountMessages.Tài_khoản_đã_tồn_tại_với_Email(request.Email)), nameof(request.Email)));
                }

                // Create new account
                var user = new ApplicationUser()
                {
                    UserName = request.UserName,
                    Email = request.Email,
                    Name = request.Name
                };
                var identityResult = await userManager.CreateAsync(user, request.Password);
                if (identityResult.Succeeded)
                {
                    identityResult = await userManager.AddToRoleAsync(user, request.Role);
                }
                if (identityResult.Succeeded)
                    return new BaseResult<Guid>(user.Id);

                return new BaseResult<Guid>(identityResult.Errors.Select(p => new Error(ErrorCode.ErrorInIdentity, p.Description)));
            }

            public async Task<PagenationResponseDto<AccountDto>> GetPagedListAsync(int pageNumber, int pageSize, string name, string[] roles)
            {
                var listRoles = roles.ToList();
                var query = userManager.Users.OrderByDescending(a=> a.Created).AsQueryable()
                    .Select(c => new AccountDto
                    {
                        Id = c.Id,
                        UserName = c.UserName,
                        Email = c.Email,
                        Created = c.Created,
                        PhoneNumber = c.PhoneNumber,
                        Name = c.Name,
                        Avatar = null,
                        Address = null
                    });

                if (!string.IsNullOrEmpty(name))
                {
                    query = query.Where(c => c.UserName.Contains(name) || c.Email.Contains(name));
                }

                var count = await query.CountAsync();

                var accountInListRoles = new List<AccountDto>();
                var listAccount = query.ToList();
                foreach (var item in listAccount)
                {
                    var user = await userManager.FindByIdAsync(item.Id.ToString());
                    var rolesList = await userManager.GetRolesAsync(user).ConfigureAwait(false);
                    item.Roles = rolesList.FirstOrDefault();
                    if (listRoles.Any(p => item.Roles.Contains(p)))
                    {
                        accountInListRoles.Add(item);
                    }
                }

                var result = listAccount.AsQueryable();

                if (roles != null && roles.Length > 0)
                {
                    result = accountInListRoles.AsQueryable();
                }

                return new PagenationResponseDto<AccountDto>(result.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(), count);
            }

            public async Task<BaseResult<ProfileResponse>> GetAccountById(Guid id)
            {
                var user = await userManager.FindByIdAsync(id.ToString());
                var rolesList = await userManager.GetRolesAsync(user).ConfigureAwait(false);
            
                var account = await accountRepository.FindByIdAsync(user.Id);
                return new BaseResult<ProfileResponse>(new ProfileResponse
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Name = account.Name,
                    Email = user.Email,
                    Created = user.Created,
                    Phone = user.PhoneNumber,
                    Roles = rolesList.FirstOrDefault(),
                    Address = account.Address,
                    Avatar = account.Avatar,
                    Status = account.Status,
                    Birthday = account.Birthday,
                    CreatedBy = account.CreatedBy,
                    LastModified = account.LastModified,
                    LastModifiedBy = account.LastModifiedBy
                });
            }

            public async Task<BaseResult<AccountDto>> FindByEmailAsync(string email)
            {
                var account = await userManager.FindByEmailAsync(email);
                var rolesList = await userManager.GetRolesAsync(account).ConfigureAwait(false);
                if (account == null)
                    return new BaseResult<AccountDto>(new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.AccountMessages.Tài_khoản_không_tìm_thấy_với_Email(email)), nameof(email)));
                return new BaseResult<AccountDto>(new AccountDto
                {
                    Id = account.Id,
                    UserName = account.UserName,
                    Email = account.Email,
                    Created = account.Created,
                    PhoneNumber = account.PhoneNumber,
                    Name = account.Name,
                    Avatar = null,
                    Address = null,
                    Roles = rolesList.FirstOrDefault()
                });
            }
            public async Task<BaseResult> UpdateProfile(UpdateProfileCommand request)
            {
                var currentUser = await accountRepository.FindByIdAsync(Guid.Parse(authenticatedUser.UserId));
                if (currentUser == null)
                {
                    return new BaseResult<Guid>(new Error(ErrorCode.NotFound, translator.GetString("Không tim thấy tài khoản"), nameof(authenticatedUser.UserId)));
                }
                var user = await userManager.FindByIdAsync(request.Id.ToString());
                if (user == null)
                {
                    return new BaseResult(new Error(ErrorCode.NotFound, translator.GetString("Tài khoản không tồn tại.")));
                }
                var acc = await accountRepository.FindByIdAsync(user.Id);

                if(request.Email.Trim().ToLower() != acc.Email.ToLower() && accountRepository. ExistEmailAsync(request.Email.Trim().ToLower()).Result)
                {
                    return new BaseResult(new Error(ErrorCode.DuplicateData, translator.GetString("Email đã tồn tại.")));
                }
                if (request.Phone.Trim() != acc.PhoneNumber && accountRepository.ExistPhoneAsync(request.Phone.Trim()).Result)
                {
                    return new BaseResult(new Error(ErrorCode.DuplicateData, translator.GetString("Số điện thoại đã tồn tại.")));
                }
                acc.Name = request.Name.Trim();
                acc.PhoneNumber = request.Phone.Trim();
                acc.Email = request.Email.Trim();
                acc.Address = request.Address.Trim();
                acc.Birthday = request.Birthday;
                user.Name = request.Name.Trim();
                user.Email = request.Email.Trim();
                user.PhoneNumber = request.Phone.Trim();
                acc.LastModified = DateTime.Now;
                acc.LastModifiedBy = user.UserName;
               

                var identityResult = await userManager.UpdateAsync(user);
                if (identityResult.Succeeded)
                {
                    accountRepository.Update(acc);
                    await unitOfWork.SaveChangesAsync();
                    return new BaseResult();
                }

                return new BaseResult(identityResult.Errors.Select(p => new Error(ErrorCode.ErrorInIdentity, p.Description)));
            }

            public async Task<BaseResult> DeleteAccount(DeleteAccountCommand request)
            {
                var user = await userManager.FindByIdAsync(request.Id.ToString());
                var rolesList = await userManager.GetRolesAsync(user).ConfigureAwait(false);
           
                if (user == null)
                {
                    return new BaseResult(new Error(ErrorCode.NotFound, translator.GetString("Tài khoản không tồn tại.")));
                }
                if(rolesList.FirstOrDefault() != "Customer")
                {
                    return new BaseResult(new Error(ErrorCode.FieldDataInvalid, translator.GetString("Tài khoản không phải là khách không xoá được.")));
                }

                var customer = await customerRepository.GetByIdAsync(user.Id);
                if (customer.Orders == null)
                {
                    var identityResult = await userManager.DeleteAsync(user);
                    if (identityResult.Succeeded)
                    {
                        customerRepository.Delete(customer);
                        await unitOfWork.SaveChangesAsync();
                        return new BaseResult();
                    }
                    return new BaseResult(identityResult.Errors.Select(p => new Error(ErrorCode.ErrorInIdentity, p.Description)));

                }
                else
                {
                    return new BaseResult(new Error(ErrorCode.FieldDataInvalid, translator.GetString("Khách hàng đã đặt các đơn hàng không thể xoá.")));
                }
            }

        public async Task<BaseResult> UpdateImageAsync(Guid id, UpdateIngredientImageRequest request)
        {
            try
            {
                var acc = await accountRepository.FindByIdAsync(id);
                if (acc == null)
                {
                    return new BaseResult(new Error(ErrorCode.NotFound, translator.GetString("Tài khoản không tồn tại")));
                }
                if (request.Image != null)
                {
                    if (acc.Avatar != null) {
                        var imageUrl = await imageService.UpdateImageAsync(acc.Avatar, request.Image);
                        acc.Avatar = imageUrl;
                    }
                    else
                    {
                        var imageUrl = await imageService.UploadImageAsync(request.Image);
                        acc.Avatar = imageUrl;
                    }
                }
                
                accountRepository.Update(acc);
                await unitOfWork.SaveChangesAsync();
                return new BaseResult();
            }
            catch (Exception ex)
            {
                return new BaseResult($"An error occurred while creating the ingredient: {ex.Message}");
            }
        }
    }

        
    }
