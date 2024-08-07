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

namespace QuickServe.Application.Features.Ingredients.Commands.UpdateIngredientStatus
{
    public class UpdateIngredientStatusCommandHandler(IIngredientRepository ingredientRepository, IUnitOfWork unitOfWork, ITranslator translator) : IRequestHandler<UpdateIngredientStatusCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(UpdateIngredientStatusCommand request, CancellationToken cancellationToken)
        {
            var ingredient = await ingredientRepository.GetIngredientByIdAsync(request.Id);

            if (ingredient is null)
            {
                return new BaseResult(new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.IngredientMessages.Không_tìm_thấy_nguyên_liệu(request.Id)), nameof(request.Id)));
            }
            if (ingredient.Status == (int)IngredientStatus.Active)
            {
                ingredient.Status = (int)IngredientStatus.Inactive;
            }
            else
            {
                ingredient.Status = (int)IngredientStatus.Active;
            }
            if (ingredient.IngredientProducts.Count != 0 
                && ingredient.Status == (int) IngredientStatus.Inactive)
            {
                foreach (var ip in ingredient.IngredientProducts)
                {
                    ip.Product.Price -= ingredient.Price; 
                    ingredient.IngredientProducts.Remove(ip);
                }
            }
            if (ingredient.IngredientSessions.Count != 0
                && ingredient.Status == (int)IngredientStatus.Inactive)
            {
                foreach (var iss in ingredient.IngredientSessions)
                {
                    ingredient.IngredientSessions.Remove(iss);
                }
            }
            await unitOfWork.SaveChangesAsync();
            return new BaseResult();
        }
    }
}