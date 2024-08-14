using System;
using System.Threading.Tasks;
using QuickServe.Application.DTOs;
using QuickServe.Domain.Payments.Dtos;
using QuickServe.Domain.Payments.Entities;

namespace QuickServe.Application.Interfaces.Repositories;

public interface IPaymentRepository : IGenericRepository<Payment>
{
    Task<PagenationResponseDto<PaymentDto>> GetPagedListAsync(int pageNumber, int pageSize, long? storeId,
        long? refOrderId, DateTime? createdDate, bool last7Days, int? specificMonth, int? specificYear);

    Task<PagenationResponseDto<PaymentDto>> GetPagedListByStoreIdAsync(int pageNumber, int pageSize, 
        long? refOrderId, DateTime? createdDate, bool last7Days, int? specificMonth,
        int? specificYear);
}