using QuickServe.Application.DTOs.IngredientTypeTemplateSteps.Request;
using QuickServe.Application.DTOs.ProductTemplates.Request;
using QuickServe.Application.Features.TemplateSteps.Commands.CreateTemplateStep;
using QuickServe.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickServe.Application.Interfaces.IngredientTypeTemplateSteps
{
    public interface IIngredientTypeTemplateStepService
    {
        Task<BaseResult> CreateTempalte(CreateTemplateStepCommand request);
        Task<BaseResult> UpdateTempalte(CreateTemplateRequest request);
        Task<BaseResult> GetAll(GetAllTemplateRequest request);
        Task<BaseResult> GetProductTemplate(GetAllTemplateRequest request);
        Task<BaseResult> GetIngredients(long ingredientTypeId);
        Task<BaseResult> GetById(GetTemplateByIdRequest request);
        Task<BaseResult> UpdateTemplateStatus(UpdateTemplateStatusRequest request);
        Task<BaseResult> DeleteTemplate(DeleteTemplateRequest request);
    }
}
