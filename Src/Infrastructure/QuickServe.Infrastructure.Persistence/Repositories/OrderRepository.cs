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
using QuickServe.Domain.Stores.Entities;
using QuickServe.Infrastructure.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickServe.Infrastructure.Persistence.Repositories;

public class OrderRepository : GenericRepository<Order>, IOrderRepository
{
    private readonly DbSet<Order> orders;
    private readonly DbSet<Store> stores;

    public OrderRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        orders = dbContext.Set<Order>();
        stores = dbContext.Set<Store>();
    }

    public async Task<Order> GetByIdAsync(long id)
    {
        return await orders.AsNoTracking()
            .Include(x => x.OrderProducts)
            .ThenInclude(e => e.Product)
            .ThenInclude(a => a.IngredientProducts)
            .ThenInclude(b => b.Ingredient)
            .FirstOrDefaultAsync(o => o.Id == id);
    }
    public async Task<PagenationResponseDto<OrderDto>> GetOrderAsync(int pageNumber, int pageSize)
    {
        var query = orders.AsNoTracking()
                .Include(x => x.OrderProducts)
                .ThenInclude(e => e.Product)
                .ThenInclude(a => a.IngredientProducts)
                .ThenInclude(b => b.Ingredient)
                .OrderByDescending(x=> x.Created);

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
          && o.Status == (int)OrderStatus.Success && o.StoreId == storeId)
          .CountAsync();
        }
        else
        {
            orderCount = await orders.AsNoTracking()
           .Where(o => o.Created >= startDate && o.Created <= endDate
           && o.Status == (int)OrderStatus.Success)
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
            .Where(o => o.Status == (int)OrderStatus.Success && o.StoreId == storeId)
            .CountAsync();
        }
        else
        {
            totalOrderCount = await orders.AsNoTracking()
            .Include(x => x.Store)
            .Where(o => o.Status == (int)OrderStatus.Success)
            .CountAsync();
        }
        return totalOrderCount;
    }
    public async Task<double> GetRevenueReportAsync(DateTime startDate, DateTime endDate, long? storeId)
    {
        var totalRevenue = 0.0;
        if(storeId != null)
        {
            if (await stores.AnyAsync(c => c.Id == storeId) == false)
            {
                throw new Exception("Không tìm thấy cửa hàng.");
            }
            totalRevenue = await orders.AsNoTracking()
            .Include(x => x.Store)
           .Where(o => o.Created >= startDate && o.Created <= endDate
           && o.Status == (int)OrderStatus.Success && o.StoreId == storeId)
           .SumAsync(o => o.Amount);
        }
        else
        {
            totalRevenue = await orders.AsNoTracking()
          .Include(x => x.Store)
         .Where(o => o.Created >= startDate && o.Created <= endDate
         && o.Status == (int)OrderStatus.Success)
         .SumAsync(o => o.Amount);
        }

        return totalRevenue;
    }
    public async Task<double> GetTotalRevenueAsync(long? storeId)
    {
        var totalRevenue = 0.0;
        if(storeId != null)
        {
            if (await stores.AnyAsync(c => c.Id == storeId) == false)
            {
                throw new Exception("Không tìm thấy cửa hàng.");
            }
            totalRevenue = await orders.AsNoTracking()
                .Include(x => x.Store)
                .Where(o => o.Status == (int)OrderStatus.Success && o.StoreId == storeId)
               .SumAsync(o => o.Amount);
        }
        else
        {
            totalRevenue = await orders.AsNoTracking()
            .Include(x => x.Store)
            .Where(o => o.Status == (int)OrderStatus.Success)
           .SumAsync(o => o.Amount);
        }
      
        return totalRevenue;
    }
    public async Task<List<BestSellingReportDto>> GetBestSellingProductTemplatesAsync(DateTime startDate, DateTime endDate, long? storeId)
    {
        var bestSellingProducts = new List<BestSellingReportDto>();
        if(storeId!=null)
        {
            if (await stores.AnyAsync(c => c.Id == storeId) == false)
            {
                throw new Exception("Không tìm thấy cửa hàng.");
            }
            bestSellingProducts = await orders.AsNoTracking()
           .Include(o => o.OrderProducts)
           .ThenInclude(op => op.Product)
           .ThenInclude(p => p.ProductTemplate)
           .Where(o => o.Created >= startDate && o.Created <= endDate && o.Status == (int)OrderStatus.Success && o.StoreId == storeId)
           .SelectMany(o => o.OrderProducts)
           .GroupBy(op => op.Product.ProductTemplate)
           .OrderByDescending(g => g.Sum(op => op.Quantity ?? 0))
           .Take(10)
           .Select(g => new BestSellingReportDto
           {
               StartDate = startDate,
               EndDate = endDate,
               BestSellingProductTemplates = g.GroupBy(x => x.Product.ProductTemplate.Id).Select(gp => new ProductTemplateDTO
               {
                   Id = gp.Key,
                   Name = gp.First().Product.ProductTemplate.Name,
                   UrlImage = gp.First().Product.ProductTemplate.ImageUrl,
                   SellingQuantity = gp.Sum(x => x.Quantity ?? 0),
                   TotalOrders = gp.Count(),
                   TotalRevenue = (double)gp.Sum(x => x.Quantity * x.Price)
               }).ToList()
           })
           .ToListAsync();
        }
        else
        {
            bestSellingProducts = await orders.AsNoTracking()
           .Include(o => o.OrderProducts)
           .ThenInclude(op => op.Product)
           .ThenInclude(p => p.ProductTemplate)
           .Where(o => o.Created >= startDate && o.Created <= endDate && o.Status == (int)OrderStatus.Success)
           .SelectMany(o => o.OrderProducts)
           .GroupBy(op => op.Product.ProductTemplate)
           .OrderByDescending(g => g.Sum(op => op.Quantity ?? 0))
           .Take(10)
           .Select(g => new BestSellingReportDto
           {
               StartDate = startDate,
               EndDate = endDate,
               BestSellingProductTemplates = g.GroupBy(x => x.Product.ProductTemplate.Id).Select(gp => new ProductTemplateDTO
               {
                   Id = gp.Key,
                   Name = gp.First().Product.ProductTemplate.Name,
                   UrlImage = gp.First().Product.ProductTemplate.ImageUrl,
                   SellingQuantity = gp.Sum(x => x.Quantity ?? 0),
                   TotalOrders = gp.Count(),
                   TotalRevenue = (double)gp.Sum(x => x.Quantity * x.Price)
               }).ToList()
           })
           .ToListAsync();
        }

        return bestSellingProducts;
    }

    public async Task<PagenationResponseDto<OderStatusResponse>> GetOrdersToWaitingScreen(long storeId, int pageNumber, int pageSize, int status)
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


        return await Paged(query.Select(c=> new OderStatusResponse
        {
            Id = c.Id.ToString(),
            Status = c.Status
        }),
            pageNumber,
            pageSize);
    }
}