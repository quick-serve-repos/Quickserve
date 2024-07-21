using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuickServe.Application.DTOs;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Utils;
using QuickServe.Application.Utils.Enums;
using QuickServe.Domain.Categories.Dtos;
using QuickServe.Domain.Categories.Entities;
using QuickServe.Infrastructure.Persistence.Contexts;

namespace QuickServe.Infrastructure.Persistence.Repositories;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    private readonly DbSet<Category> categories;
    
    public CategoryRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        categories = dbContext.Set<Category>();
    }


    public async Task<bool> ExistsCategoryByNameAsync(string name)
    {
        return await categories.AnyAsync(c => c.Name.ToLower() == name.ToLower());
    }

    public async Task<Category> FindByIdAsync(long id)
    {
       return await categories.Include(c=>c.ProductTemplates)
            .ThenInclude(pt=> pt.Products)
            .FirstOrDefaultAsync(c=>c.Id == id);
    }

    public async Task<PagenationResponseDto<CategoryDto>> GetPagedListAsync(int pageNumber, int pageSize, string name)
    {
        var query = categories.OrderByDescending(c => c.Created).AsQueryable();
        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(c => c.Name.Contains(name));
        }

      
        return await Paged(
            query.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
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

    public async Task<PagenationResponseDto<CategoryDto>> GetPagedListByAcitveStatusAsync(int pageNumber, int pageSize, string name)
    {
        var query = categories.Where(c => c.Status ==(int)CategoryStatus.Active).OrderByDescending(c => c.Created).AsQueryable();
        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(c => c.Name.Contains(name));
        }


        return await Paged(
            query.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
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
}