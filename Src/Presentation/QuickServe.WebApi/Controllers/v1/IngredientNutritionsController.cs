using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickServe.Application.DTOs.IngredientNutrions.Request;
using QuickServe.Application.Interfaces.IngredientNutritions;
using QuickServe.Application.Wrappers;
using QuickServe.Domain.Ingredients.Entities;
using System.Threading.Tasks;

namespace QuickServe.WebApi.Controllers.v1
{
    public class IngredientNutritionsController : BaseApiController
    {
        private readonly IIngredientNutritionService _service;
        public IngredientNutritionsController(IIngredientNutritionService service)
        {
            _service = service;
        }

        [HttpGet("{ingredientId}")]
        public async Task<BaseResult> GetByIngredientId(long ingredientId)
           => await _service.GetByIngredientIdAsync(ingredientId);

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Brand_Manager")]
        public async Task<BaseResult> CreateIngredientNutrition(CreateIngredientNutritionRequest request)
            => await _service.CreateIngredientNutritionAsync(request);

        [HttpPut("{ingredientId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Brand_Manager")]
        public async Task<BaseResult> UpdateIngredientNutrition(long ingredientId, CreateIngredientNutritionRequest request)
        {
           request.IngredientId = ingredientId;
           return await _service.UpdateIngredientNutritionAsync(request);
        }
            

        [HttpDelete("{ingredientId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Brand_Manager")]
        public async Task<BaseResult> DeleteIngredientNutrition(long ingredientId, DeleteNutritionInIngredientRequest request)
            => await _service.DeleteIngredientNutritionAsync(ingredientId, request);
    }
}
