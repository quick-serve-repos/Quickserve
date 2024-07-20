using System.Collections.Generic;
using QuickServe.Application.DTOs;
using QuickServe.Domain.Orders.Entities;
using System.Threading.Tasks;
using System;
using QuickServe.Application.DTOs.Orders.Response;
using QuickServe.Domain.Orders.Dtos;

namespace QuickServe.Application.Interfaces.Repositories;

public interface IOrderRepository : IGenericRepository<Order>
{
    Task<Order> GetByIdAsync(long id);

    Task<PagenationResponseDto<OrderDto>> GetOrderAsync(int pageNumber, int pageSize);
    Task<double> GetRevenueReportAsync(DateTime startDate, DateTime endDate, long? storeId);
    Task<double> GetTotalRevenueAsync(long? storeId);
    Task<int> GetOrderCountAsync(DateTime startDate, DateTime endDate, long? storeId);
    Task<int> GetTotalOrderCountAsync(long? storeId);
    Task<List<BestSellingReportDto>> GetBestSellingProductTemplatesAsync(DateTime startDate, DateTime endDate, long? storeId);
}