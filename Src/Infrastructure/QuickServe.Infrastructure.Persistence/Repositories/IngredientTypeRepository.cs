using Microsoft.EntityFrameworkCore;
using QuickServe.Application.DTOs;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Utils;
using QuickServe.Application.Utils.Enums;
using QuickServe.Domain.IngredientTypes.Dtos;
using QuickServe.Domain.IngredientTypes.Entities;
using QuickServe.Infrastructure.Persistence.Contexts;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace QuickServe.Infrastructure.Persistence.Repositories;

public class IngredientTypeRepository : GenericRepository<IngredientType>, IIngredientTypeRepository
{
    private readonly DbSet<IngredientType> ingredientTypes;

    public IngredientTypeRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        ingredientTypes = dbContext.Set<IngredientType>();
    }


    public async Task<bool> ExistByNameAsync(string name)
    {
        return await ingredientTypes.AnyAsync(c => c.Name.ToLower() == name.ToLower());
    }

    public async Task<IngredientType> GetIngredientTypeByIdAsync(long id)
    {
        return await ingredientTypes.Include(c=> c.Ingredients)
            .Include(c=> c.IngredientTypeTemplateSteps)
            .ThenInclude(s=> s.TemplateStep)
            .ThenInclude(p=>p.ProductTemplate)
            .FirstOrDefaultAsync(c=> c.Id == id);
    }

    public async Task<PagenationResponseDto<IngredientTypeDTO>> GetPagedListAsync(int pageNumber, int pageSize, string name)
    {
        var query = ingredientTypes.OrderBy(c => c.Created).AsQueryable();
        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(c => c.Name.Contains(name));
        }


        return await Paged(
            query.Select(c => new IngredientTypeDTO
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

    public async Task<PagenationResponseDto<IngredientTypeDTO>> GetPagedListByAcitveStatusAsync(int pageNumber, int pageSize, string name)
    {
        var query = ingredientTypes.Where(c => c.Status == (int)IngredientTypeStatus.Active).OrderBy(c => c.Created).AsQueryable();
        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(c => c.Name.Contains(name));
        }


        return await Paged(
            query.Select(c => new IngredientTypeDTO
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