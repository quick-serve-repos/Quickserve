using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickServe.Application.DTOs.Ingredients.Request;
using QuickServe.Application.Features.Ingredients.Commands.DeleteIngredient;
using QuickServe.Application.Features.Ingredients.Commands.UpdateIngredient;
using QuickServe.Application.Features.Ingredients.Queries.GetIngredientById;
using QuickServe.Application.Features.Ingredients.Queries.GetPagedListIngredient;
using QuickServe.Application.Features.Ingredients.Queries.GetPagedListIngredientByActiveStatus;
using QuickServe.Application.Interfaces.IngredientInterfaces;
using QuickServe.Application.Wrappers;
using QuickServe.Domain.Ingredients.Dtos;
using System;
using System.Threading.Tasks;

namespace QuickServe.WebApi.Controllers.v1
{
    public class IngredientsController : BaseApiController
    {
        private readonly IIngredientService _ingredientService;

        public IngredientsController(IIngredientService ingredientService)
        {
            _ingredientService = ingredientService;
        }

        [HttpGet("paged")]
        public async Task<PagedResponse<IngredientDTO>> GetPagedListIngredient([FromQuery] GetPagedListIngredientQuery model)
            => await Mediator.Send(model);

        [HttpGet("pagedByActiveStatus")]
        public async Task<PagedResponse<IngredientDTO>> GetPagedListByActiveStatus([FromQuery] GetPagedListIngredientByActiveStatusQuery model)
            => await Mediator.Send(model);

        [HttpGet("{id}")]
        public async Task<BaseResult<IngredientDTO>> GetIngredientById(long id)
            => await Mediator.Send(new GetIngredientByIdQuery { Id = id });

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Brand_Manager")]
        public async Task<BaseResult> CreateIngredient([FromForm] CreateIngredientRequest request)
            => await _ingredientService.CreateIngredientAsync(request);

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Brand_Manager")]
        public async Task<BaseResult> UpdateIngredient(long id, UpdateIngredientCommand model)
        {
            model.Id = id;
            return await Mediator.Send(model);
        }

        [HttpPut("{id}/image")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Brand_Manager")]
        public async Task<BaseResult> UpdateIngredientImage(long id, [FromForm] UpdateIngredientImageRequest request)
            => await _ingredientService.UpdateIngredientImageAsync(id, request);

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Brand_Manager")]
        public async Task<BaseResult> DeleteIngredient(long id)
            => await Mediator.Send(new DeleteIngredientCommand { Id = id });
    }
}
