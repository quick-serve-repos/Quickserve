using System.Collections.Generic;
using QuickServe.Application.DTOs;
using QuickServe.Domain.Categories.Entities;
using QuickServe.Domain.Orders.Dtos;
using QuickServe.Domain.Orders.Entities;
using System.Threading.Tasks;

namespace QuickServe.Application.Interfaces.Repositories;

public interface IOrderRepository : IGenericRepository<Order>
{
    Task<Order> GetByIdAsync(long id);

    Task<PagenationResponseDto<Order>> GetOrderAsync(int pageNumber, int pageSize);
}