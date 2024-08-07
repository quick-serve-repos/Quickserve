using Microsoft.EntityFrameworkCore;
using QuickServe.Application.DTOs;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Utils;
using QuickServe.Domain.Ingredients.Entities;
using QuickServe.Domain.Nutritions.Dtos;
using QuickServe.Domain.Nutritions.Entities;
using QuickServe.Infrastructure.Persistence.Contexts;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace QuickServe.Infrastructure.Persistence.Repositories;

public class NutritionRepository : GenericRepository<Nutrition>, INutritionRepository
{
    private readonly DbSet<Nutrition> nutritions;

    public NutritionRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        nutritions = dbContext.Set<Nutrition>();
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await nutritions.AnyAsync(c => c.Name.ToLower() == name.ToLower());
    }

    public async Task<Nutrition> FindByIdAsync(long id)
    {
        return await nutritions.Include(c => c.IngredientNutritions)
           .ThenInclude(pt => pt.Ingredient)
           .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<PagenationResponseDto<NutritionDTO>> GetPagedListAsync(int pageNumber, int pageSize, string name)
    {
        var query = nutritions.OrderByDescending(c => c.Created).AsQueryable();
        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(c => c.Name.Contains(name));
        }


        return await Paged(
            query.Select(c => new NutritionDTO
            {
                Id = c.Id,
                Name = c.Name,
                Vitamin = c.Vitamin,
                HealthValue = c.HealthValue,
                Description = c.Description,
                ImageUrl = c.ImageUrl,
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