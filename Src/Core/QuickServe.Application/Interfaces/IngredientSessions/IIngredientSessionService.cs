using QuickServe.Application.DTOs.IngredientSessions;
using QuickServe.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickServe.Application.Interfaces.IngredientSessions
{
    public interface IIngredientSessionService
    {
        Task<BaseResult> CreateIngredientSessionAsync(CreateIngredientSessionRequest request);
        Task<BaseResult> UpdateIngredientSessionAsync(CreateIngredientSessionRequest request);
        Task<BaseResult> GetBySessionIdAsync(long sessionId);
        Task<BaseResult> DeleteIngredientSessionAsync(long sessionId, DeleteIngredientInSessionRequest request);
    }
}
