using System.Threading.Tasks;
using QuickServe.Domain.Customers.Entities;
using System;

namespace QuickServe.Application.Interfaces.Repositories;

public interface ICustomerRepository : IGenericRepository<Customer>
{
    Task<Customer> GetByPhoneAsync(string phone);
    Task<Customer> GetByIdAsync(Guid id);
}