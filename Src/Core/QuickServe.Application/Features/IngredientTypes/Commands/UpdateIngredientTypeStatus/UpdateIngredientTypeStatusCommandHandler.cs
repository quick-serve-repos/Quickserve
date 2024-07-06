using MediatR;
using QuickServe.Application.Features.Categories.Commands.UpdateCategoryStatus;
using QuickServe.Application.Helpers;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Interfaces;
using QuickServe.Application.Utils.Enums;
using QuickServe.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using QuickServe.Domain.TemplateSteps.Entities;

namespace QuickServe.Application.Features.IngredientTypes.Commands.UpdateIngredientTypeStatus
{
    public class UpdateIngredientTypeStatusCommandHandler(IIngredientTypeRepository ingredientTypeRepository, IUnitOfWork unitOfWork, ITranslator translator) : IRequestHandler<UpdateIngredientTypeStatusCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(UpdateIngredientTypeStatusCommand request, CancellationToken cancellationToken)
        {
            var ingredientType = await ingredientTypeRepository.GetIngredientTypeByIdAsync(request.Id);

            if (ingredientType is null)
            {
                return new BaseResult(new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.IngredientTypeMessages.Không_tìm_thấy_loại_nguyên_liệu(request.Id)), nameof(request.Id)));
            }
            if (ingredientType.Status == (int)IngredientTypeStatus.Active)
            {
                ingredientType.Status = (int)IngredientTypeStatus.Inactive;
            }
            else
            {
                ingredientType.Status = (int)IngredientTypeStatus.Active;
            }
            if (ingredientType.Ingredients.Count != 0)
            {
                foreach (var t in ingredientType.Ingredients)
                {
                    t.Status = ingredientType.Status;
                }
            }
            if(ingredientType.IngredientTypeTemplateSteps.Count != 0 && ingredientType.Status ==(int) IngredientTypeStatus.Inactive)
            {
                foreach (var t in ingredientType.IngredientTypeTemplateSteps)
                {
                    t.TemplateStep.Status = (int) TemplateStepStatus.Inactive;
                    t.TemplateStep.ProductTemplate.Status = (int) TemplateStepStatus.Inactive;
                }
            }
            ingredientType.Update(ingredientType.Status);
            await unitOfWork.SaveChangesAsync();
            return new BaseResult();
        }
    }
}