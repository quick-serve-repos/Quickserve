using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuickServe.Application.DTOs;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Domain.Payments.Dtos;
using QuickServe.Domain.Payments.Entities;
using QuickServe.Infrastructure.Persistence.Contexts;

namespace QuickServe.Infrastructure.Persistence.Repositories;

public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
{
    private readonly DbSet<Payment> _payments;

    public PaymentRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _payments = dbContext.Set<Payment>();
    }

    /*public async Task<PagenationResponseDto<PaymentDto>> GetPagedListAsync(int pageNumber, int pageSize, long? storeId)
    {
        var query = _payments.Include(p => p.Order).OrderByDescending(p => p.Created).AsQueryable();

        if (storeId.HasValue)
        {
            query = query.Where(p => p.Order.StoreId == storeId.Value);
        }

        var result = await Paged(
            query.Select(p => new PaymentDto
            {
                Id = p.Id,
                Created = p.Created,
                PaymentType = p.PaymentType.ToString(),
                RefOrderId = p.RefOrderId.ToString(),
                Amount = p.Order.Amount,
                StoreId = p.Order.StoreId,
                Status = p.Order.Status
            }),
            pageNumber,
            pageSize);

        return result;
    }*/
    public async Task<PagenationResponseDto<PaymentDto>> GetPagedListAsync(int pageNumber, int pageSize, long? storeId, long? refOrderId = null, DateTime? createdDate = null, bool last7Days = false, bool lastMonth = false)
    {
        var query = _payments
            .Include(p => p.Order)
            .OrderByDescending(p => p.Created)
            .AsQueryable();

        // Filter by StoreId if provided
        if (storeId.HasValue)
        {
            query = query.Where(p => p.Order.StoreId == storeId.Value);
        }

        // Filter by RefOrderId if provided
        if (refOrderId.HasValue)
        {
            query = query.Where(p => p.RefOrderId == refOrderId.Value);
        }

        // Ensure that the DateTime is handled correctly
        if (createdDate.HasValue)
        {
            var date = DateTime.SpecifyKind(createdDate.Value.Date, DateTimeKind.Utc);
            query = query.Where(p => p.Created.Date == date);
        }
        else if (last7Days)
        {
            var fromDate = DateTime.UtcNow.AddDays(-7);
            query = query.Where(p => p.Created >= fromDate);
        }
        else if (lastMonth)
        {
            var firstDayOfLastMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).AddMonths(-1);
            var lastDayOfLastMonth = firstDayOfLastMonth.AddMonths(1).AddDays(-1);
            query = query.Where(p => p.Created >= firstDayOfLastMonth && p.Created <= lastDayOfLastMonth);
        }

        var result = await Paged(
            query.Select(p => new PaymentDto
            {
                Id = p.Id,
                Created = p.Created,
                PaymentType = p.PaymentType.ToString(),
                RefOrderId = p.RefOrderId.ToString(),
                Amount = p.Order.Amount,
                StoreId = p.Order.StoreId,
                Status = p.Order.Status
            }),
            pageNumber,
            pageSize);

        return result;
    }

    
   /* public async Task<PagenationResponseDto<PaymentDto>> GetPagedListByStoreIdAsync(int pageNumber, int pageSize, long storeId)
    {
        var query = _payments
            .Include(p => p.Order)
            .Where(p => p.Order.StoreId == storeId)
            .OrderByDescending(p => p.Created)
            .AsQueryable();

        var result = await Paged(
            query.Select(p => new PaymentDto
            {
                Id = p.Id,
                Created = p.Created,
                PaymentType = p.PaymentType.ToString(),
                RefOrderId = p.RefOrderId.ToString(),
                Amount = p.Order.Amount,
                StoreId = p.Order.StoreId,
                Status = p.Order.Status
            }),
            pageNumber,
            pageSize);

        return result;
    }*/
   public async Task<PagenationResponseDto<PaymentDto>> GetPagedListByStoreIdAsync(int pageNumber, int pageSize, long storeId, long? refOrderId = null, DateTime? createdDate = null, bool last7Days = false, bool lastMonth = false)
   {
       var query = _payments
           .Include(p => p.Order)
           .Where(p => p.Order.StoreId == storeId)
           .OrderByDescending(p => p.Created)
           .AsQueryable();

       // Filter by RefOrderId if provided
       if (refOrderId.HasValue)
       {
           query = query.Where(p => p.RefOrderId == refOrderId.Value);
       }

       // Ensure that the DateTime is handled correctly
       if (createdDate.HasValue)
       {
           var date = DateTime.SpecifyKind(createdDate.Value.Date, DateTimeKind.Utc);
           query = query.Where(p => p.Created.Date == date);
       }
       else if (last7Days)
       {
           var fromDate = DateTime.UtcNow.AddDays(-7);
           query = query.Where(p => p.Created >= fromDate);
       }
       else if (lastMonth)
       {
           var firstDayOfLastMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).AddMonths(-1);
           var lastDayOfLastMonth = firstDayOfLastMonth.AddMonths(1).AddDays(-1);
           query = query.Where(p => p.Created >= firstDayOfLastMonth && p.Created <= lastDayOfLastMonth);
       }

       var result = await Paged(
           query.Select(p => new PaymentDto
           {
               Id = p.Id,
               Created = p.Created,
               PaymentType = p.PaymentType.ToString(),
               RefOrderId = p.RefOrderId.ToString(),
               Amount = p.Order.Amount,
               StoreId = p.Order.StoreId,
               Status = p.Order.Status
           }),
           pageNumber,
           pageSize);

       return result;
   }


}