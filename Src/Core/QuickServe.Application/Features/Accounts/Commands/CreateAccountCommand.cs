using MediatR;
using QuickServe.Application.Wrappers;
using System;

namespace QuickServe.Application.Features.Accounts.Commands
{
    public class CreateAccountCommand : IRequest<BaseResult<Guid>>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public long StoreId { get; set; }
    }
}
