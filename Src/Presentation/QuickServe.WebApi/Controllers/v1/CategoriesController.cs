using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickServe.Application.Features.Categories.Commands.CreateCategory;
using QuickServe.Application.Features.Categories.Commands.DeleteCategory;
using QuickServe.Application.Features.Categories.Commands.UpdateCategory;
using QuickServe.Application.Features.Categories.Commands.UpdateCategoryStatus;
using QuickServe.Application.Features.Categories.Queries.GetCategoryById;
using QuickServe.Application.Features.Categories.Queries.GetPagedListCategory;
using QuickServe.Application.Features.Categories.Queries.GetPagedListCategoryByActiveStatus;
using QuickServe.Application.Wrappers;
using QuickServe.Domain.Categories.Dtos;

namespace QuickServe.WebApi.Controllers.v1
{
    public class CategoriesController : BaseApiController
    {
        [HttpGet("paged")]
        public async Task<PagedResponse<CategoryDto>> GetPagedListCategory([FromQuery] GetPagedListCategoryQuery model)
            => await Mediator.Send(model);

        [HttpGet("active")]
        public async Task<PagedResponse<CategoryDto>> GetPagedListByActiveStatusCategory([FromQuery] GetPagedListCategoryByActiveStatusQuery model)
            => await Mediator.Send(model);

        [HttpGet("{id}")]
        public async Task<BaseResult<CategoryDto>> GetCategoryById(long id)
            => await Mediator.Send(new GetCategoryByIdQuery { Id = id });

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Brand_Manager")]
        public async Task<BaseResult> CreateCategory(CreateCategoryCommand model)
            => await Mediator.Send(model);

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Brand_Manager")]
        public async Task<BaseResult> UpdateCategory(long id, UpdateCategoryCommand model)
        {
            model.Id = id;
            return await Mediator.Send(model);
        }

        [HttpPut("{id}/status")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Brand_Manager")]
        public async Task<BaseResult> UpdateStatusCategory(long id)
            => await Mediator.Send(new UpdateCategoryStatusCommand { Id = id });

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Brand_Manager")]
        public async Task<BaseResult> DeleteCategory(long id)
            => await Mediator.Send(new DeleteCategoryCommand { Id = id });
    }
}
