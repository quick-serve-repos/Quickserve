using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuickServe.Application.DTOs;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Utils;
using QuickServe.Application.Utils.Enums;
using QuickServe.Domain.Categories.Dtos;
using QuickServe.Domain.Ingredients.Dtos;
using QuickServe.Domain.Ingredients.Entities;
using QuickServe.Domain.IngredientTypes.Dtos;
using QuickServe.Domain.ProductTemplates.Dtos;
using QuickServe.Domain.ProductTemplates.Entities;
using QuickServe.Infrastructure.Persistence.Contexts;

namespace QuickServe.Infrastructure.Persistence.Repositories;

public class ProductTemplateRepository : GenericRepository<ProductTemplate>, IProductTemplateRepository
{
    private readonly DbSet<ProductTemplate> _productTemplates;

    public ProductTemplateRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _productTemplates = dbContext.Set<ProductTemplate>();
    }

    public async Task<bool> ExistByNameAsync(string name)
    {
        return await _productTemplates.AnyAsync(c => c.Name.ToLower() == name.ToLower());
    }

    public async Task<PagenationResponseDto<ProductTemplateDto>> GetPagedListAsync(int pageNumber, int pageSize,
        string name)
    {
        var query = _productTemplates.OrderByDescending(p => p.Created).AsQueryable();
        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(s => s.Name.Contains(name));
        }

        return await Paged(
             query.Select(c => new ProductTemplateDto
             {
                 Id = c.Id,
                 Name = c.Name,
                 Price = c.Price,
                 Size = c.Size,
                 Description = c.Description,
                 ImageUrl = c.ImageUrl,
                 Category = new CategoryResponse(c.Category),
                 CategoryId = c.CategoryId,
                 Status = c.Status,
                 Created = TimeZoneConverter.ConvertToUserTimeZone(c.Created),
                 CreatedBy = c.CreatedBy,
                 LastModified = c.LastModified.HasValue
                 ? TimeZoneConverter.ConvertToUserTimeZone(c.LastModified.Value)
                 : (DateTime?)null,  // Xử lý giá trị NULL
                 LastModifiedBy = c.LastModifiedBy ?? null // Xử lý giá trị NULL
             }),
             pageNumber,
             pageSize);
    }

    public async Task<PagenationResponseDto<ProductTemplateDto>> GetPagedListByAcitveStatusAsync(int pageNumber, int pageSize, string name, long? storeId)
    {
        var query = _productTemplates.Where(c => c.Status == (int)ProductTemplateStatus.Active).OrderByDescending(p => p.Created).AsQueryable();
        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(s => s.Name.Contains(name));
        }

        if (storeId != 0)
        {
            var currentTime = DateTime.UtcNow.AddHours(7).TimeOfDay;
            query = query.Where(pt => pt.TemplateSteps.Any(ts =>
                ts.IngredientTypeTemplateSteps.Any(itts =>
                    itts.IngredientType.Ingredients.Any(i =>
                        i.IngredientSessions.Any(isess =>
                            !(isess.Session.StoreId == storeId.Value &&
                            isess.Quantity == isess.SoldQuantity &&
                            itts.QuantityMin > 0 &&
                            isess.Session.StartTime <= currentTime &&
                            isess.Session.EndTime >= currentTime )
                        )
                    )
                )
            ));
        }

        return await Paged(
             query.Select(c => new ProductTemplateDto
             {
                 Id = c.Id,
                 Name = c.Name,
                 Price = c.Price,
                 Size = c.Size,
                 Description = c.Description,
                 ImageUrl = c.ImageUrl,
                 Category = new CategoryResponse(c.Category),
                 CategoryId = c.CategoryId,
                 Status = c.Status,
                 Created = TimeZoneConverter.ConvertToUserTimeZone(c.Created),
                 CreatedBy = c.CreatedBy,
                 LastModified = c.LastModified.HasValue
                 ? TimeZoneConverter.ConvertToUserTimeZone(c.LastModified.Value)
                 : (DateTime?)null,  // Xử lý giá trị NULL
                 LastModifiedBy = c.LastModifiedBy ?? null // Xử lý giá trị NULL
             }),
             pageNumber,
             pageSize);
    }

    public async Task<ProductTemplate> GetProductTemplateByIdAsync(long id)
    {
        return await _productTemplates.Include(c=> c.Category).Include(p => p.Products)
            .Include(c=> c.TemplateSteps)
            .FirstOrDefaultAsync(c=> c.Id == id);
    }
}