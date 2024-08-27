using Azure.Core;
using Microsoft.EntityFrameworkCore;
using QuickServe.Application.DTOs;
using QuickServe.Application.DTOs.Orders.Response;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Utils.Enums;
using QuickServe.Application.Wrappers;
using QuickServe.Domain.Ingredients.Dtos;
using QuickServe.Domain.Orders.Dtos;
using QuickServe.Domain.Orders.Entities;
using QuickServe.Domain.Products.Dtos;
using QuickServe.Domain.ProductTemplates.Entities;
using QuickServe.Domain.Stores.Entities;
using QuickServe.Infrastructure.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using QuickServe.Application.Interfaces;

namespace QuickServe.Infrastructure.Persistence.Repositories;

public class OrderRepository : GenericRepository<Order>, IOrderRepository
{
    private readonly DbSet<Order> orders;
    private readonly DbSet<Store> stores;
    private readonly ApplicationDbContext _context;
    private readonly IAuthenticatedUserService _authenticatedUserService;
    private readonly IAccountRepository _accountRepository;

    public OrderRepository(ApplicationDbContext dbContext, IAuthenticatedUserService authenticatedUserService,
        IAccountRepository accountRepository) : base(dbContext)
    {
        orders = dbContext.Set<Order>();
        stores = dbContext.Set<Store>();
        _authenticatedUserService = authenticatedUserService;
        _accountRepository = accountRepository;
        _context = dbContext;
    }

    public async Task<Order> GetByIdAsync(long id)
    {
        return await orders.AsNoTracking()
            .Include(o => o.Store)  
            .Include(x => x.OrderProducts)
            .ThenInclude(e => e.Product)
            .ThenInclude(a => a.IngredientProducts)
            .ThenInclude(b => b.Ingredient)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<PagenationResponseDto<OrderDto>> GetOrderAsync(int pageNumber, int pageSize)
    {
        var userId = _authenticatedUserService.UserId;

        var curretUser = await _accountRepository.FindByIdAsync(Guid.Parse(userId));

        var query = orders.AsNoTracking()
            .Include(x => x.OrderProducts)
            .ThenInclude(e => e.Product)
            .ThenInclude(a => a.IngredientProducts)
            .ThenInclude(b => b.Ingredient)
            .Where(u => u.StoreId == curretUser.Staff.StoreId)
            .OrderByDescending(x => x.Created);

        var totalCount = await query.CountAsync();
        var pagedOrders = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var orderDtos = new List<OrderDto>();
        foreach (var order in pagedOrders)
        {
            var orderDto = new OrderDto(order);
            var productList = new List<ProDuctsDto>();
            foreach (var item in order.OrderProducts)
            {
                if (item.Product == null)
                    continue;

                var productDto = new ProDuctsDto(item.Product);
                var ingredientList = new List<IngredientDTO>();
                foreach (var obj in item.Product.IngredientProducts)
                {
                    if (obj == null) continue;
                    ingredientList.Add(new IngredientDTO(obj.Ingredient));
                }

                productDto.Ingredients = ingredientList;
                productList.Add(productDto);
            }

            orderDto.Products = productList;
            orderDtos.Add(orderDto);
        }

        return new PagenationResponseDto<OrderDto>(orderDtos, totalCount);
    }

    public async Task<int> GetOrderCountAsync(DateTime startDate, DateTime endDate, long? storeId)
    {
        var orderCount = 0;
        if (storeId != null)
        {
            if (await stores.AnyAsync(c => c.Id == storeId) == false)
            {
                throw new Exception("Không tìm thấy cửa hàng.");
            }

            orderCount = await orders.AsNoTracking()
                .Where(o => o.Created >= startDate && o.Created <= endDate
                                                    && o.StoreId == storeId)
                .CountAsync();
        }
        else
        {
            orderCount = await orders.AsNoTracking()
                .Where(o => o.Created >= startDate && o.Created <= endDate
                                                   )
                .CountAsync();
        }


        return orderCount;
    }

    public async Task<int> GetTotalOrderCountAsync(long? storeId)
    {
        var totalOrderCount = 0;
        if (storeId != null)
        {
            if (await stores.AnyAsync(c => c.Id == storeId) == false)
            {
                throw new Exception("Không tìm thấy cửa hàng.");
            }

            totalOrderCount = await orders.AsNoTracking()
                .Include(x => x.Store)
                .Where(o => o.StoreId == storeId)
                .CountAsync();
        }
        else
        {
            totalOrderCount = await orders.AsNoTracking()
                .Include(x => x.Store)
                .CountAsync();
        }

        return totalOrderCount;
    }

    public async Task<double> GetRevenueReportAsync(DateTime startDate, DateTime endDate, long? storeId)
    {
        var totalRevenue = 0.0;
        if (storeId != null)
        {
            if (await stores.AnyAsync(c => c.Id == storeId) == false)
            {
                throw new Exception("Không tìm thấy cửa hàng.");
            }

            totalRevenue = await orders.AsNoTracking()
                .Include(x => x.Store)
                .Where(o => o.Created >= startDate && o.Created <= endDate
                                                   && (o.Status == (int)OrderStatus.Success || o.Status == (int)OrderStatus.Got)
                                                   && o.StoreId == storeId)
                .SumAsync(o => o.Amount);
        }
        else
        {
            totalRevenue = await orders.AsNoTracking()
                .Include(x => x.Store)
                .Where(o => o.Created >= startDate && o.Created <= endDate
                                                   && (o.Status == (int)OrderStatus.Success || o.Status == (int)OrderStatus.Got))
                .SumAsync(o => o.Amount);
        }

        return totalRevenue;
    }

    public async Task<double> GetTotalRevenueAsync(long? storeId)
    {
        var totalRevenue = 0.0;
        if (storeId != null)
        {
            if (await stores.AnyAsync(c => c.Id == storeId) == false)
            {
                throw new Exception("Không tìm thấy cửa hàng.");
            }

            totalRevenue = await orders.AsNoTracking()
                .Include(x => x.Store)
                .Where(o => (o.Status == (int)OrderStatus.Success || o.Status == (int)OrderStatus.Got) && o.StoreId == storeId)
                .SumAsync(o => o.Amount);
        }
        else
        {
            totalRevenue = await orders.AsNoTracking()
                .Include(x => x.Store)
                .Where(o => o.Status == (int)OrderStatus.Success ||o.Status ==  (int)OrderStatus.Got)
                .SumAsync(o => o.Amount);
        }

        return totalRevenue;
    }

    public async Task<BestSellingReportDto> GetBestSellingProductTemplatesAsync(DateTime startDate, DateTime endDate,
        long? storeId)
    {
        var bestSellingReport = new BestSellingReportDto
        {
            StartDate = startDate,
            EndDate = endDate,
            BestSellingProductTemplates = new List<ProductTemplateDTO>()
        };

        var query = orders.AsNoTracking()
            .Include(o => o.OrderProducts)
            .ThenInclude(op => op.Product)
            .ThenInclude(p => p.ProductTemplate)
            .Where(o => o.Created >= startDate && o.Created <= endDate && o.Status == (int)OrderStatus.Success);

        if (storeId != null)
        {
            if (await stores.AnyAsync(c => c.Id == storeId) == false)
            {
                throw new Exception("Không tìm thấy cửa hàng.");
            }

            query = query.Where(o => o.StoreId == storeId);
        }

        var productTemplates = await query
            .SelectMany(o => o.OrderProducts)
            .GroupBy(op => op.Product.ProductTemplate)
            .OrderByDescending(g => g.Sum(op => op.Quantity ?? 0))
            .Take(10)
            .Select(g => new ProductTemplateDTO
            {
                Id = g.Key.Id,
                Name = g.Key.Name,
                UrlImage = g.Key.ImageUrl,
                SellingQuantity = g.Sum(x => x.Quantity ?? 0),
                TotalOrders = g.Count(),
                TotalRevenue = (double)g.Sum(x => x.Quantity * x.Price)
            })
            .ToListAsync();

        bestSellingReport.BestSellingProductTemplates = productTemplates;

        return bestSellingReport;
    }

    public async Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(Guid customerId)
    {
        return await orders.AsNoTracking()
            .Include(o => o.OrderProducts)
            .ThenInclude(op => op.Product)
            .ThenInclude(p => p.IngredientProducts)
            .ThenInclude(ip => ip.Ingredient)
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.Created) // Sắp xếp theo thời gian tạo, mới nhất trước
            .ToListAsync();
    }

    public async Task<PagenationResponseDto<OderStatusResponse>> GetOrdersToWaitingScreen(long storeId, int pageNumber,
        int pageSize, int status)
    {
        if (await stores.AnyAsync(c => c.Id == storeId) == false)
        {
            throw new Exception("Không tìm thấy cửa hàng.");
        }

        var query = orders.AsNoTracking()
            .Where(o => (o.Status == (int)OrderStatus.Preparing || o.Status == (int)OrderStatus.Success)
                        && o.StoreId == storeId
                        && (status == 0 || o.Status == status))
            .Take(20)
            .OrderByDescending(x => x.Created)
            .AsQueryable();


        return await Paged(query.Select(c => new OderStatusResponse
            {
                Id = c.Id.ToString(),
                Status = c.Status,
                Created = c.Created,
                Platform = c.Platform
            }),
            pageNumber,
            pageSize);
    }

    public async Task<Dictionary<OrderStatus, int>> GetOrderStatusCountsAsync(DateTime startDate, DateTime endDate,
        long? storeId)
    {
        var query = orders.AsNoTracking()
            .Where(o => o.Created >= startDate && o.Created <= endDate);

        if (storeId.HasValue)
        {
            query = query.Where(o => o.StoreId == storeId);
        }

        var orderStatusCounts = await query
            .GroupBy(o => o.Status)
            .ToDictionaryAsync(g => (OrderStatus)g.Key, g => g.Count());

        foreach (OrderStatus status in Enum.GetValues(typeof(OrderStatus)))
        {
            if (!orderStatusCounts.ContainsKey(status))
            {
                orderStatusCounts[status] = 0;
            }
        }

        return orderStatusCounts;
    }

    public async Task<List<SoldIngredientDTO>> GetSoldIngredientsAsync(DateTime startDate, DateTime endDate,
        long? storeId)
    {
        var soldIngredientsQuery = _context.OrderProducts
            .Where(op => op.Order.Created >= startDate && op.Order.Created <= endDate);

        if (storeId.HasValue)
        {
            soldIngredientsQuery = soldIngredientsQuery.Where(op => op.Order.StoreId == storeId.Value);
        }

        var soldIngredients = await soldIngredientsQuery
            .Join(_context.IngredientProducts,
                op => op.ProductId,
                ip => ip.ProductId,
                (op, ip) => new { ip.Ingredient.Id, ip.Ingredient.Name, ip.Ingredient.ImageUrl, ip.Quantity })
            .GroupBy(x => new { x.Id, x.Name, x.ImageUrl })
            .Select(group => new SoldIngredientDTO
            {
                Id = group.Key.Id,
                Name = group.Key.Name,
                UrlImage = group.Key.ImageUrl,
                QuantitySold = group.Sum(x => x.Quantity)
            })
            .OrderByDescending(dto => dto.QuantitySold)
            .Take(10)
            .ToListAsync();

        return soldIngredients;
    }

    public async Task<PagenationResponseDto<OrderDtos>> GetOrderAsync(int pageNumber, int pageSize, long? storeId,
        long? refOrderId = null, DateTime? createdDate = null, bool last7Days = false, int? specificMonth = null,
        int? specificYear = null)
    {
        var query = orders.AsNoTracking()
            .Where(u => storeId == null || u.StoreId == storeId.Value)
            .OrderByDescending(x => x.Created)
            .AsQueryable();

        // Filter by RefOrderId if provided
        if (refOrderId.HasValue)
        {
            query = query.Where(p => p.Id == refOrderId.Value);
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

        var totalCount = await query.CountAsync();
        var pagedOrders = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var orderDtos = pagedOrders.Select(order => new OrderDtos(order)).ToList();

        return new PagenationResponseDto<OrderDtos>(orderDtos, totalCount);
    }

    public async Task<PagenationResponseDto<OrderDtos>> GetOrderByStoreIdAsync(int pageNumber, int pageSize,
         long? refOrderId = null, DateTime? createdDate = null, bool last7Days = false,
        int? specificMonth = null, int? specificYear = null)
    {
        
        var userId = _authenticatedUserService.UserId;

        var currentUser = await _accountRepository.FindByIdAsync(Guid.Parse(userId));
        var storeId = currentUser.Staff.StoreId;
        
        var query = orders.AsNoTracking()
            .Where(u => u.StoreId == storeId)
            .OrderByDescending(x => x.Created)
            .AsQueryable();

        // Filter by RefOrderId if provided
        if (refOrderId.HasValue)
        {
            query = query.Where(p => p.Id == refOrderId.Value);
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

        var totalCount = await query.CountAsync();
        var pagedOrders = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var orderDtos = pagedOrders.Select(order => new OrderDtos(order)).ToList();

        return new PagenationResponseDto<OrderDtos>(orderDtos, totalCount);
    }
    
    public async Task<PagenationResponseDto<OrderHistoryDto>> GetOrdersByCustomerIdAsync(Guid customerId, string storeName = null, DateTime? createdDate = null, bool last7Days = false, int? specificMonth = null, int? specificYear = null)
{
    var query = orders.AsNoTracking()
        .Include(o => o.Store)
        .Include(o => o.OrderProducts)
        .ThenInclude(op => op.Product)
        .ThenInclude(p => p.ProductTemplate)
        .Where(o => o.CustomerId == customerId && new[] { 2, 3, 4, 5, 6, 7 }.Contains(o.Status))
        .AsQueryable();

    if (!string.IsNullOrEmpty(storeName))
    {
        query = query.Where(o => o.Store.Name.Contains(storeName));
    }

    if (createdDate.HasValue)
    {
        var date = DateTime.SpecifyKind(createdDate.Value.Date, DateTimeKind.Utc);
        query = query.Where(o => o.Created.Date == date);
    }
    else if (last7Days)
    {
        var fromDate = DateTime.UtcNow.AddDays(-7);
        query = query.Where(o => o.Created >= fromDate);
    }
    else if (specificMonth.HasValue && specificYear.HasValue)
    {
        var firstDayOfMonth = new DateTime(specificYear.Value, specificMonth.Value, 1, 0, 0, 0, DateTimeKind.Utc);
        var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);
        query = query.Where(o => o.Created >= firstDayOfMonth && o.Created <= lastDayOfMonth);
    }
    else if (specificYear.HasValue && !specificMonth.HasValue)
    {
        var firstDayOfYear = new DateTime(specificYear.Value, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var lastDayOfYear = new DateTime(specificYear.Value, 12, 31, 23, 59, 59, DateTimeKind.Utc);
        query = query.Where(o => o.Created >= firstDayOfYear && o.Created <= lastDayOfYear);
    }

    var totalCount = await query.CountAsync();
    var pagedOrders = await query.ToListAsync();

    var orderHistoryDtos = pagedOrders.Select(order => new OrderHistoryDto(order)).ToList();

    return new PagenationResponseDto<OrderHistoryDto>(orderHistoryDtos, totalCount);
}
    
   public async Task<PagenationResponseDto<OderStatusResponse>> GetOrdersForStaff(long storeId, int pageNumber, int pageSize, int status)
   {
       if (await stores.AnyAsync(c => c.Id == storeId) == false)
       {
           throw new Exception("Không tìm thấy cửa hàng.");
       }

       // Retrieve all orders for the specified store and status
       var query = orders.AsNoTracking()
           .Where(o => o.StoreId == storeId && (status == 0 || o.Status == status)) // No longer filtering by specific statuses
           .OrderByDescending(x => x.Created)  // Order by creation date
           .AsQueryable();

       // Return paginated results
       return await Paged(query.Select(c => new OderStatusResponse
           {
               Id = c.Id.ToString(),
               Status = c.Status,
               Created = c.Created,
               Platform = c.Platform
           }),
           pageNumber,
           pageSize);
   }

   public async Task<List<Order>> GetOrdersWithStatusNotUpdatedAsync(int status, TimeSpan timeNotUpdated)
   {
       var timeThreshold = DateTime.UtcNow.Add(-timeNotUpdated);

       return await _context.Orders
           .Where(o => o.Status == status && o.LastModified < timeThreshold)
           .ToListAsync();
   }
}