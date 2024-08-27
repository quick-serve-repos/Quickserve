using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickServe.Application.DTOs.Nutritions.Request;
using QuickServe.Application.Features.Nutritions.Commands.DeleteNutrition;
using QuickServe.Application.Features.Nutritions.Commands.UpdateNutrition;
using QuickServe.Application.Features.Nutritions.Queries.GetNutritionById;
using QuickServe.Application.Features.Nutritions.Queries.GetPagedListNutrition;
using QuickServe.Application.Interfaces.Nutritions;
using QuickServe.Application.Wrappers;
using QuickServe.Domain.Nutritions.Dtos;
using System.Threading.Tasks;

namespace QuickServe.WebApi.Controllers.v1
{
    public class NutritionsController : BaseApiController
    {
        private readonly INutritionService _nutritionService;

        public NutritionsController(INutritionService nutritionService)
        {
            _nutritionService = nutritionService;
        }


        [HttpGet("paged")]
        public async Task<PagedResponse<NutritionDTO>> GetPagedListNutrition([FromQuery] GetPagedListNutritionQuery model)
        => await Mediator.Send(model);

        [HttpGet("{id}")]
        public async Task<BaseResult<NutritionDTO>> GetNutritionById(long id)
            => await Mediator.Send(new GetNutritionByIdQuery { Id = id });

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Brand_Manager")]
        public async Task<BaseResult> CreateNutrition([FromForm] CreateNutritionRequest request)
            => await _nutritionService.CreateNutritionAsync(request);

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Brand_Manager")]
        public async Task<BaseResult> UpdateNutrition(UpdateNutritionCommand model)
        {
            return await Mediator.Send(model);
        }

        [HttpPut("{id}/image")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Brand_Manager")]
        public async Task<BaseResult> UpdateNutritionImage(long id, [FromForm] UpdateNutritionImageRequest request)
            => await _nutritionService.UpdateNutritionImageAsync(id,  request);

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Brand_Manager")]
        public async Task<BaseResult> DeleteNutrition(long id)
            => await Mediator.Send(new DeleteNutritionCommand { Id = id });
    }
}
