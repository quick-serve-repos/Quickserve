using MediatR;
using QuickServe.Application.Features.Categories.Commands.UpdateCategory;
using QuickServe.Application.Helpers;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Interfaces;
using QuickServe.Application.Wrappers;
using System.Threading.Tasks;
using System.Threading;

namespace QuickServe.Application.Features.IngredientTypes.Commands.UpdateIngredientType;

public class UpdateIngredientTypeCommandHandler(IIngredientTypeRepository ingredientTypeRepository, IUnitOfWork unitOfWork, ITranslator translator) : IRequestHandler<UpdateIngredientTypeCommand, BaseResult>
{
    public async Task<BaseResult> Handle(UpdateIngredientTypeCommand request, CancellationToken cancellationToken)
    {
        if (request.Id <= 0)
        {
            return new BaseResult(new Error(ErrorCode.FieldDataInvalid, translator.GetString(TranslatorMessages.RequestMessage.Trường_id_không_hợp_lệ(request.Id)), nameof(request.Id)));
        }
        var ingredientType = await ingredientTypeRepository.GetByIdAsync(request.Id);

        if (ingredientType is null)
        {
            return new BaseResult(new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.IngredientTypeMessages.Không_tìm_thấy_loại_nguyên_liệu(request.Id)), nameof(request.Id)));
        }
        if (await ingredientTypeRepository.ExistByNameAsync(request.Name.Trim()) && 
            ingredientType.Name.ToLower() != request.Name.ToLower().Trim())
        {
            return new BaseResult(new Error(ErrorCode.Duplicate, translator.GetString(TranslatorMessages.IngredientTypeMessages.Tên_loại_nguyên_liệu_đã_tồn_tại(request.Name)), nameof(request.Name)));
        }
        ingredientType.Update(request.Name.Trim());
        await unitOfWork.SaveChangesAsync();
        return new BaseResult();
    }
}