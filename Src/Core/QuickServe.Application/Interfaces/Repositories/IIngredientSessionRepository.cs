using QuickServe.Domain.Ingredients.Entities;
using System.Threading.Tasks;
using QuickServe.Domain.IngredientSessions.Entities;

namespace QuickServe.Application.Interfaces.Repositories;

public interface IIngredientSessionRepository :IGenericRepository<IngredientSession>
{ 
    Task<IngredientSession> GetByIdAsync(long ingredientId, long sessionId);
}