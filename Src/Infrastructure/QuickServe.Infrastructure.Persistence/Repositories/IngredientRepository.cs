using Azure.Core;
using Microsoft.EntityFrameworkCore;
using QuickServe.Application.DTOs;
using QuickServe.Application.Helpers;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Utils;
using QuickServe.Application.Utils.Enums;
using QuickServe.Application.Wrappers;
using QuickServe.Domain.Ingredients.Dtos;
using QuickServe.Domain.Ingredients.Entities;
using QuickServe.Domain.IngredientTypes.Dtos;
using QuickServe.Infrastructure.Persistence.Contexts;
using QuickServe.Infrastructure.Resources.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickServe.Infrastructure.Persistence.Repositories;

public class IngredientRepository : GenericRepository<Ingredient>, IIngredientRepository
{
    private readonly DbSet<Ingredient> ingredients;

    public IngredientRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        ingredients = dbContext.Set<Ingredient>();
    }


    public async Task<bool> ExistByNameAsync(string name, long id)
    {
        return await ingredients.AnyAsync(c => c.Name.ToLower() == name.ToLower() && c.IngredientTypeId == id);
    }

    public async Task<Ingredient> GetIngredientByIdAsync(long id)
    {
        return await ingredients.Include(i=> i.IngredientType)
            .ThenInclude(i=> i.IngredientTypeTemplateSteps)
            .ThenInclude(i=> i.TemplateStep)
            .ThenInclude(i=> i.ProductTemplate)
            .Include(i=>i.IngredientSessions)
            .ThenInclude(s=> s.Session)
            .Include(i=>i.IngredientNutritions)
            .Include(i=>i.IngredientProducts).ThenInclude(ip=> ip.Product)
            .FirstOrDefaultAsync(i=>i.Id == id);
        
    }

    public async Task<PagenationResponseDto<IngredientDTO>> GetPagedListAsync(int pageNumber, int pageSize, string name)
    {
        var query = ingredients.OrderByDescending(c => c.Created).AsQueryable();
        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(c => c.Name.Contains(name));
        }


        return await Paged(
            query.Select(c => new IngredientDTO
            {
                Id = c.Id,
                Name = c.Name,
                Price = c.Price,
                Calo = c.Calo,
                Description = c.Description,
                ImageUrl = c.ImageUrl,
                DefaultQuantity = c.DefaultQuantity,
                QuantityMax = c.QuantityMax,
                IngredientType = new SimpleIngredietTypeRespone(c.IngredientType),
                IngredientTypeId = c.IngredientTypeId,
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

    public async Task<PagenationResponseDto<IngredientDTO>> GetPagedListByAcitveStatusAsync(int pageNumber, int pageSize, string name)
    {
        var query = ingredients.Where(c => c.Status == (int)IngredientStatus.Active).OrderBy(c => c.Created).AsQueryable();
        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(c => c.Name.Contains(name));
        }


        return await Paged(
            query.Select(c => new IngredientDTO
            {
                Id = c.Id,
                Name = c.Name,
                Price = c.Price,
                Calo = c.Calo,
                Description = c.Description,
                QuantityMax = c.QuantityMax,
                ImageUrl = c.ImageUrl,
                IngredientType = new SimpleIngredietTypeRespone(c.IngredientType),
                IngredientTypeId = c.IngredientTypeId,
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