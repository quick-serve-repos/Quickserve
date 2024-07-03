using QuickServe.Application.DTOs;
using QuickServe.Domain.Categories.Entities;
using QuickServe.Domain.Orders.Dtos;
using QuickServe.Domain.Orders.Entities;
using System.Threading.Tasks;
using QuickServe.Domain.Customers.Entities;

namespace QuickServe.Application.Interfaces.Repositories;

public interface ICustomerRepository : IGenericRepository<Customer>
{
    Task<Customer> GetByPhoneAsync(string phone);
}