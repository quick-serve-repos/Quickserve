using Microsoft.EntityFrameworkCore;
using QuickServe.Application.DTOs;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Domain.Categories.Entities;
using QuickServe.Domain.Orders.Dtos;
using QuickServe.Domain.Orders.Entities;
using QuickServe.Domain.ProductTemplates.Entities;
using QuickServe.Domain.Stores.Entities;
using QuickServe.Infrastructure.Persistence.Contexts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuickServe.Domain.Customers.Entities;
using System;
using System.Numerics;

namespace QuickServe.Infrastructure.Persistence.Repositories;

public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
{
    private readonly DbSet<Customer> customer;

    public CustomerRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        customer = dbContext.Set<Customer>();
    }

    public async Task<Customer> GetByIdAsync(Guid id)
    {
        return await customer.FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<Customer> GetByPhoneAsync(string phone)
    {
        return await customer.FirstOrDefaultAsync(o => o.PhoneNumber == phone);
    }
}