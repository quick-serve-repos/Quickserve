
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickServe.Application.DTOs.IngredientSessions;
using QuickServe.Application.Interfaces.IngredientSessions;
using QuickServe.Application.Wrappers;
using System.Threading.Tasks;

namespace QuickServe.WebApi.Controllers.v1
{
    public class IngredientSessionsController : BaseApiController
    {
        private readonly IIngredientSessionService _service;
        public IngredientSessionsController(IIngredientSessionService service)
        {
            _service = service;
        }

        [HttpGet("{sessionId}")]
        public async Task<BaseResult> GetBySesionId(long sessionId)
          => await _service.GetBySessionIdAsync(sessionId);

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Store_Manager")]
        public async Task<BaseResult> CreateIngredientSession(CreateIngredientSessionRequest request)
            => await _service.CreateIngredientSessionAsync(request);

        [HttpPut("{sessionId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Store_Manager")]
        public async Task<BaseResult> UpdateIngredientSession(long sessionId, CreateIngredientSessionRequest request)
        {
            request.SessionId = sessionId;
            return await _service.UpdateIngredientSessionAsync(request);
        }


        [HttpDelete("{sessionId}/ingredient")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Store_Manager")]
        public async Task<BaseResult> DeleteIngredientSession(long sessionId, DeleteIngredientInSessionRequest request)
            => await _service.DeleteIngredientSessionAsync(sessionId, request);
        
        [HttpDelete("{sessionId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Store_Manager")]
        public async Task<BaseResult> DeleteAllIngredientInSession(long sessionId)
           => await _service.DeleteAllIngredientAsync(sessionId);
    }
}
