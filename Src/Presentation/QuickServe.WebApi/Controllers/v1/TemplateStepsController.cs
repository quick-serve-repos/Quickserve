using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickServe.Application.Features.TemplateSteps.Commands.CreateTemplateStep;
using QuickServe.Application.Features.TemplateSteps.Commands.DeleteTemplateStep;
using QuickServe.Application.Features.TemplateSteps.Commands.UpdateTemplateStep;
using QuickServe.Application.Features.TemplateSteps.Queries.GetPagedListTemplateStep;
using QuickServe.Application.Features.TemplateSteps.Queries.GetPagedListTemplateStepByActiveStatus;
using QuickServe.Application.Features.TemplateSteps.Queries.GetTemplateStepById;
using QuickServe.Application.Wrappers;
using QuickServe.Domain.TemplateSteps.Dtos;
using System;
using System.Threading.Tasks;

namespace QuickServe.WebApi.Controllers.v1
{
    public class TemplateStepsController : BaseApiController
    {
        [HttpGet("paged")]
        public async Task<PagedResponse<TemplateStepDTO>> GetPagedListTemplateStep([FromQuery] GetPagedListTemplateStepQuery model)
            => await Mediator.Send(model);

        [HttpGet("active")]
        public async Task<PagedResponse<TemplateStepDTO>> GetPagedListByActiveStatus([FromQuery] GetPagedListTemplateStepByActiveStatusQuery model)
            => await Mediator.Send(model);

        [HttpGet("{id}")]
        public async Task<BaseResult<TemplateStepDTO>> GetTemplateStepById(long id)
            => await Mediator.Send(new GetTemplateStepByIdQuery { Id = id });


        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Brand_Manager")]
        public async Task<BaseResult> UpdateTemplateStep(long id, UpdateTemplateStepCommand model)
        {
            model.Id = id;
            return await Mediator.Send(model);
        }

        
    }
}
