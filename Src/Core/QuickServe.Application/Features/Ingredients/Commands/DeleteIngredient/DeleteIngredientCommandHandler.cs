using MediatR;
using QuickServe.Application.Helpers;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Interfaces;
using QuickServe.Application.Wrappers;
using System.Threading.Tasks;
using System.Threading;

namespace QuickServe.Application.Features.Ingredients.Commands.DeleteIngredient;

public class DeleteIngredientCommandHandler(IIngredientRepository ingredientRepository,IUnitOfWork unitOfWork, ITranslator translator) : IRequestHandler<DeleteIngredientCommand, BaseResult>
{
    public async Task<BaseResult> Handle(DeleteIngredientCommand request, CancellationToken cancellationToken)
    {
        var ingredient = await ingredientRepository.GetIngredientByIdAsync(request.Id);

        if (ingredient is null)
        {
            return new BaseResult(new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.IngredientMessages.Không_tìm_thấy_nguyên_liệu(request.Id)), nameof(request.Id)));
        }
        if (ingredient.IngredientSessions.Count !=0 && ingredient.IngredientProducts.Count != 0)
        {
            return new BaseResult(new Error(ErrorCode.ConstraintViolation, translator.GetString(TranslatorMessages.IngredientMessages.Nguyên_liệu_tồn_tại_trong_sản_phẩm_và_ca_làm_việc(request.Id)), nameof(request.Id)));
        }
        if(ingredient.IngredientNutritions.Count != 0)
        {
            foreach(var i in ingredient.IngredientNutritions)
            {
                ingredient.IngredientNutritions.Remove(i);
            }
        }
        ingredientRepository.Delete(ingredient);
        await unitOfWork.SaveChangesAsync();

        return new BaseResult();
    }
}