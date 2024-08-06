using MediatR;
using QuickServe.Application.Features.Categories.Commands.UpdateCategory;
using QuickServe.Application.Helpers;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Interfaces;
using QuickServe.Application.Wrappers;
using System.Threading.Tasks;
using System.Threading;

namespace QuickServe.Application.Features.Ingredients.Commands.UpdateIngredient;

public class UpdateIngredientCommandHandler(IIngredientRepository ingredientRepositiry, IUnitOfWork unitOfWork, ITranslator translator) : IRequestHandler<UpdateIngredientCommand, BaseResult>
{
    public async Task<BaseResult> Handle(UpdateIngredientCommand request, CancellationToken cancellationToken)
    {
        if (request.Id <= 0)
        {
            return new BaseResult(new Error(ErrorCode.FieldDataInvalid, translator.GetString(TranslatorMessages.RequestMessage.Trường_id_không_hợp_lệ(request.Id)), nameof(request.Id)));
        }
        var ingredient = await ingredientRepositiry.GetIngredientByIdAsync(request.Id);

        if (ingredient is null)
        {
            return new BaseResult(new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.IngredientMessages.Không_tìm_thấy_nguyên_liệu(request.Id)), nameof(request.Id)));
        }
        if (await ingredientRepositiry.ExistByNameAsync(request.Name.Trim()) && ingredient.Name.ToLower() != request.Name.ToLower().Trim())
        {
            return new BaseResult(new Error(ErrorCode.Duplicate, translator.GetString(TranslatorMessages.IngredientMessages.Tên_nguyên_liệu_đã_tồn_tại(request.Name)), nameof(request.Name)));
        }

        ingredient.Update(request.Name.Trim(), request.Price, request.Calo, request.DefaultQuantity, request.Description
            , request.IngredientTypeId, request.QuantityMax);
        if(ingredient.Price != request.Price)
        {
            foreach (var ingreStep in ingredient.IngredientType.IngredientTypeTemplateSteps)
            {
                ingreStep.TemplateStep.ProductTemplate.Price += (request.Price - ingredient.Price)* ingredient.DefaultQuantity;
            }
        }
        await unitOfWork.SaveChangesAsync();
        return new BaseResult();
    }
}