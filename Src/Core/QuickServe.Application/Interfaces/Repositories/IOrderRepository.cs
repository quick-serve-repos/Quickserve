using System.Collections.Generic;
using QuickServe.Application.DTOs;
using QuickServe.Domain.Orders.Entities;
using System.Threading.Tasks;
using System;
using QuickServe.Application.DTOs.Orders.Response;
using QuickServe.Domain.Orders.Dtos;
using QuickServe.Application.Utils.Enums;

namespace QuickServe.Application.Interfaces.Repositories;

public interface IOrderRepository : IGenericRepository<Order>
{
    Task<Order> GetByIdAsync(long id);
    Task<PagenationResponseDto<OderStatusResponse>> GetOrdersToWaitingScreen(long storeId, int pageNumber, int pageSize, int status);
    Task<PagenationResponseDto<OrderDto>> GetOrderAsync(int pageNumber, int pageSize);
    Task<double> GetRevenueReportAsync(DateTime startDate, DateTime endDate, long? storeId);
    Task<double> GetTotalRevenueAsync(long? storeId);
    Task<int> GetOrderCountAsync(DateTime startDate, DateTime endDate, long? storeId);
    Task<int> GetTotalOrderCountAsync(long? storeId);
    Task<BestSellingReportDto> GetBestSellingProductTemplatesAsync(DateTime startDate, DateTime endDate, long? storeId);
   // Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(Guid customerId);
    Task<Dictionary<OrderStatus, int>> GetOrderStatusCountsAsync(DateTime startDate, DateTime endDate, long? storeId);
    Task<List<SoldIngredientDTO>> GetSoldIngredientsAsync(DateTime startDate, DateTime endDate, long? storeId);

    Task<PagenationResponseDto<OrderDtos>> GetOrderAsync(int pageNumber, int pageSize, long? storeId,
        long? refOrderId , DateTime? createdDate , bool last7Days , int? specificMonth,
        int? specificYear);

    Task<PagenationResponseDto<OrderDtos>> GetOrderByStoreIdAsync(int pageNumber, int pageSize, long storeId,
        long? refOrderId , DateTime? createdDate , bool last7Days , int? specificMonth ,
        int? specificYear);

    Task<PagenationResponseDto<OrderHistoryDto>> GetOrdersByCustomerIdAsync(Guid customerId, string storeName ,
        DateTime? createdDate, bool last7Days , int? specificMonth , int? specificYear);
}