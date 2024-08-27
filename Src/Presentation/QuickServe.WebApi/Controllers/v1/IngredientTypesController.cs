using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickServe.Application.Features.IngredientTypes.Commands.CreateIngredientType;
using QuickServe.Application.Features.IngredientTypes.Commands.DeleteIngredientType;
using QuickServe.Application.Features.IngredientTypes.Commands.UpdateIngredientType;
using QuickServe.Application.Features.IngredientTypes.Commands.UpdateIngredientTypeStatus;
using QuickServe.Application.Features.IngredientTypes.Queries.GetIngredientTypeById;
using QuickServe.Application.Features.IngredientTypes.Queries.GetPagedListIngredientType;
using QuickServe.Application.Features.IngredientTypes.Queries.GetPagedListIngredientTypeByActiveStatus;
using QuickServe.Application.Wrappers;
using QuickServe.Domain.IngredientTypes.Dtos;
using System;
using System.Threading.Tasks;

namespace QuickServe.WebApi.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class IngredientTypesController : BaseApiController
    {
        [HttpGet("paged")]
        public async Task<PagedResponse<IngredientTypeDTO>> GetPagedListIngredientType([FromQuery] GetPagedListIngredientTypeQuery model)
            => await Mediator.Send(model);

        [HttpGet("pagedByActiveStatus")]
        public async Task<PagedResponse<IngredientTypeDTO>> GetPagedListByActiveStatus([FromQuery] GetPagedListIngredientTypeByActiveStatusQuery model)
            => await Mediator.Send(model);

        [HttpGet("{id}")]
        public async Task<BaseResult<IngredientTypeDTO>> GetIngredientTypeById(long id)
            => await Mediator.Send(new GetIngredientTypeByIdQuery { Id = id });

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Brand_Manager")]
        public async Task<BaseResult> CreateIngredientType(CreateIngredientTypeCommand model)
            => await Mediator.Send(model);

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Brand_Manager")]
        public async Task<BaseResult> UpdateIngredientType(long id, UpdateIngredientTypeCommand model)
        {
            model.Id = id;
            return await Mediator.Send(model);
        }
           

        [HttpPut("{id}/status")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Brand_Manager")]
        public async Task<BaseResult> UpdateIngredientTypeStatus(long id)
             => await Mediator.Send(new UpdateIngredientTypeStatusCommand { Id = id });

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Brand_Manager")]
        public async Task<BaseResult> DeleteIngredientType(long id)
            => await Mediator.Send(new DeleteIngredientTypeCommand { Id = id });
    }
}
