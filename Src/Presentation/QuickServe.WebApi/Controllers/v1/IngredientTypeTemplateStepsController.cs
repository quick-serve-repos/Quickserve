using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickServe.Application.DTOs.IngredientTypeTemplateSteps.Request;
using QuickServe.Application.DTOs.ProductTemplates.Request;
using QuickServe.Application.Features.TemplateSteps.Commands.CreateTemplateStep;
using QuickServe.Application.Interfaces.IngredientTypeTemplateSteps;
using QuickServe.Application.Wrappers;
using System;
using System.Threading.Tasks;

namespace QuickServe.WebApi.Controllers.v1
{
    public class IngredientTypeTemplateStepsController : BaseApiController
    {
        private readonly IIngredientTypeTemplateStepService _service;
        public IngredientTypeTemplateStepsController(IIngredientTypeTemplateStepService service)
        {
            _service = service;
        }
        [HttpGet("all")]
        public async Task<BaseResult> GetAll([FromQuery] GetAllTemplateRequest model)
            => await _service.GetAll(model);
        [HttpGet("productemplate")]
        public async Task<BaseResult> GetProductTemplate([FromQuery] GetAllTemplateRequest model)
           => await _service.GetProductTemplate(model);
        [HttpGet("IngredientType/{id}")]
        public async Task<BaseResult> GetProductIngredients(long id)
           => await _service.GetIngredients(id);
        [HttpGet("{id}")]
        public async Task<BaseResult> GetTemplateById(long id)
            => await _service.GetById(new GetTemplateByIdRequest { TemplateStepId = id });

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Brand_Manager")]
        public async Task<BaseResult> CreateTemplate(CreateTemplateStepCommand request)
            => await _service.CreateTempalte(request);

        [HttpPut("{templateStepId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Brand_Manager")]
        public async Task<BaseResult> UpdateTemplate(long templateStepId, CreateTemplateRequest request)
        {
            request.TemplateStepId = templateStepId;
            return await _service.UpdateTempalte(request);
        }
           

        [HttpPut("{productTemplateId}/status")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Brand_Manager")]
        public async Task<BaseResult> UpdateTemplateStatus(long productTemplateId)
            => await _service.UpdateTemplateStatus(new UpdateTemplateStatusRequest { ProductTemplateId = productTemplateId});

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Brand_Manager")]
        public async Task<BaseResult> DeleteTemplate(long id)
            => await _service.DeleteTemplate(new DeleteTemplateRequest { TemplateStepId = id });
    }
}
