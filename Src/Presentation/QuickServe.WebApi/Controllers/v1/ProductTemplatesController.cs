using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickServe.Application.DTOs.ProductTemplates;
using QuickServe.Application.Features.ProductTemplates.Commands.DeleteProductTemplate;
using QuickServe.Application.Features.ProductTemplates.Commands.UpdateProductTemplate;
using QuickServe.Application.Features.ProductTemplates.Queries.GetPagedListProductTemplate;
using QuickServe.Application.Features.ProductTemplates.Queries.GetPagedListProductTemplateByActiveStatus;
using QuickServe.Application.Features.ProductTemplates.Queries.GetProductTemplateById;
using QuickServe.Application.Interfaces.IProductTemplateServices;
using QuickServe.Application.Wrappers;
using QuickServe.Domain.ProductTemplates.Dtos;

namespace QuickServe.WebApi.Controllers.v1
{
    public class ProductTemplatesController : BaseApiController
    {
        private readonly IProductTemplateService _productTemplateService;

        public ProductTemplatesController(IProductTemplateService productTemplateService)
        {
            _productTemplateService = productTemplateService;
        }

        [HttpGet("paged")]
        public async Task<PagedResponse<ProductTemplateDto>> GetPagedListProductTemplate(
            [FromQuery] GetPagedListProductTemplateQuery model)
            => await Mediator.Send(model);

        [HttpGet("pagedByActiveStatus")]
        public async Task<PagedResponse<ProductTemplateDto>> GetPagedListProductTemplateByActiveStatus(
            [FromQuery] GetPagedListProductTemplateByActiveStatusQuery model)
            => await Mediator.Send(model);

        [HttpGet("{id}")]
        public async Task<BaseResult<ProductTemplateDto>> GetProductTemplateById(long id)
            => await Mediator.Send(new GetProductTemplateByIdQuery { Id = id });

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Brand_Manager")]
        public async Task<BaseResult> CreateProductTemplate([FromForm] CreateProductTemplateRequest request)
            => await _productTemplateService.CreateProductTemplateAsync(request);

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Brand_Manager")]
        public async Task<BaseResult> UpdateProductTemplate(long id, UpdateProductTemplateCommand model)
        {
            model.Id = id;
            return await Mediator.Send(model);
        }

        [HttpPut("{id}/image")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Brand_Manager")]
        public async Task<BaseResult> UpdateProductTemplateImage(long id, [FromForm] UpdateProductTemplateImageRequest request)
            => await _productTemplateService.UpdateProductTemplateImageAsync(id, request);

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Brand_Manager")]
        public async Task<BaseResult> DeleteProductTemplate(long id)
            => await Mediator.Send(new DeleteProductTemplateCommand { Id = id });
    }
}
