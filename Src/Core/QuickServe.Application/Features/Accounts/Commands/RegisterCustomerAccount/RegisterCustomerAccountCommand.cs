using MediatR;
using QuickServe.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickServe.Application.Features.Accounts.Commands.RegisterCustomerAccount
{
    public class RegisterCustomerAccountCommand : IRequest<BaseResult<Guid>>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
    }
}
