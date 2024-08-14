using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuickServe.Application.DTOs;
using QuickServe.Application.Interfaces;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Domain.Payments.Dtos;
using QuickServe.Domain.Payments.Entities;
using QuickServe.Infrastructure.Persistence.Contexts;

namespace QuickServe.Infrastructure.Persistence.Repositories;

public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
{
    private readonly DbSet<Payment> _payments;
    private readonly IAuthenticatedUserService _authenticatedUserService;
    private readonly IAccountRepository _accountRepository;
    private readonly ApplicationDbContext _context;
    public PaymentRepository(ApplicationDbContext dbContext, IAuthenticatedUserService authenticatedUserService, IAccountRepository accountRepository ) : base(dbContext)
    {
        _payments = dbContext.Set<Payment>();
        _authenticatedUserService = authenticatedUserService;
        _accountRepository = accountRepository;
        _context = dbContext;
    }

    public async Task<PagenationResponseDto<PaymentDto>> GetPagedListAsync(int pageNumber, int pageSize, long? storeId,
        long? refOrderId = null, DateTime? createdDate = null, bool last7Days = false, int? specificMonth = null,
        int? specificYear = null)
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
        else if (specificMonth.HasValue && specificYear.HasValue)
        {
            var firstDayOfMonth = new DateTime(specificYear.Value, specificMonth.Value, 1, 0, 0, 0, DateTimeKind.Utc);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);
            query = query.Where(p => p.Created >= firstDayOfMonth && p.Created <= lastDayOfMonth);
        }
        else if (specificYear.HasValue && !specificMonth.HasValue)
        {
            var firstDayOfYear = new DateTime(specificYear.Value, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var lastDayOfYear = new DateTime(specificYear.Value, 12, 31, 23, 59, 59, DateTimeKind.Utc);
            query = query.Where(p => p.Created >= firstDayOfYear && p.Created <= lastDayOfYear);
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


    public async Task<PagenationResponseDto<PaymentDto>> GetPagedListByStoreIdAsync(int pageNumber, int pageSize,
        long storeId, long? refOrderId = null, DateTime? createdDate = null, bool last7Days = false,
        bool lastMonth = false)
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


    public async Task<PagenationResponseDto<PaymentDto>> GetPagedListByStoreIdAsync(
        int pageNumber,
        int pageSize,
        long? refOrderId = null,
        DateTime? createdDate = null,
        bool last7Days = false,
        int? specificMonth = null,
        int? specificYear = null)
    {
        
        var userId = _authenticatedUserService.UserId;

        var currentUser = await _accountRepository.FindByIdAsync(Guid.Parse(userId));
        var storeId = currentUser.Staff.StoreId;
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
        else if (specificMonth.HasValue && specificYear.HasValue)
        {
            var firstDayOfMonth = new DateTime(specificYear.Value, specificMonth.Value, 1, 0, 0, 0, DateTimeKind.Utc);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);
            query = query.Where(p => p.Created >= firstDayOfMonth && p.Created <= lastDayOfMonth);
        }
        else if (specificYear.HasValue && !specificMonth.HasValue)
        {
            var firstDayOfYear = new DateTime(specificYear.Value, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var lastDayOfYear = new DateTime(specificYear.Value, 12, 31, 23, 59, 59, DateTimeKind.Utc);
            query = query.Where(p => p.Created >= firstDayOfYear && p.Created <= lastDayOfYear);
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