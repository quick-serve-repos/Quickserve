using MediatR;
using QuickServe.Application.Interfaces;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Interfaces.UserInterfaces;
using QuickServe.Application.Wrappers;
using QuickServe.Domain.Accounts.Entities;
using QuickServe.Domain.Customers.Entities;
using QuickServe.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QuickServe.Application.Features.Accounts.Commands.RegisterCustomerAccount
{
    public class RegisterCustomerAccountCommandHandler(IAccountServices _accountServices, 
        ICustomerRepository customerRepository, IUnitOfWork unitOfWork) 
    : IRequestHandler<RegisterCustomerAccountCommand, BaseResult<Guid>>
    {
        public async Task<BaseResult<Guid>> Handle(RegisterCustomerAccountCommand request, CancellationToken cancellationToken)
        {
            var result = await _accountServices.CreateAccount(new DTOs.Account.Requests.CreateAccountRequest
            {
                Email = request.Email,
                Password = request.Password,
                Role = AccountRole.Customer.ToString(),
                UserName = request.UserName,
                Name = request.Name
                
            });
            
            if (result.Success)
            {
                var customer = new Customer
                {
                    Email = request.Email,
                    UserName = request.UserName,
                    Id = result.Data,
                    Name = request.Name,
                    CreatedBy = request.UserName, 
                };

                await customerRepository.AddAsync(customer);
                return new BaseResult<Guid>(customer.Id);
            }

            return new BaseResult<Guid>(result.Errors);
        }
    }
}
