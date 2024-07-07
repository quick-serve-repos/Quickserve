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
using QuickServe.Domain.IngredientSessions.Entities;

namespace QuickServe.Infrastructure.Persistence.Repositories;

public class IngredientSessionRepository :GenericRepository<IngredientSession>, IIngredientSessionRepository
{
    private readonly DbSet<IngredientSession> ingredientSessions;

    public IngredientSessionRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        ingredientSessions = dbContext.Set<IngredientSession>();
    }

    public async Task<IngredientSession> GetByIdAsync(long ingredientId, long sessionId)
    {
        return await ingredientSessions.FirstOrDefaultAsync(a => a.IngredientId == ingredientId && a.SessionId == sessionId);
    }
}