using MediatR;
using QuickServe.Application.DTOs.IngredientTypes.Request;
using QuickServe.Application.DTOs.IngredientTypeTemplateSteps.Request;
using QuickServe.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickServe.Application.Features.TemplateSteps.Commands.CreateTemplateStep
{
    public class CreateTemplateStepCommand : IRequest<BaseResult>
    {
        public long ProductTemplateId { get; set; }
        public string Name { get; set; }
        public List<IngredientTypeTemplates> IngredientTypes { get; set; }
    }

}