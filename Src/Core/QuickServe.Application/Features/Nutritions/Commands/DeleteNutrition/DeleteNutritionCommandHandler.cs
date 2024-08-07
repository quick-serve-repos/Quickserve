using QuickServe.Application.Helpers;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Interfaces;
using QuickServe.Application.Wrappers;
using System.Threading.Tasks;
using System.Threading;
using MediatR;

namespace QuickServe.Application.Features.Nutritions.Commands.DeleteNutrition;

public class DeleteNutritionCommandHandler(INutritionRepository nutritionRepository, IUnitOfWork unitOfWork, ITranslator translator) : IRequestHandler<DeleteNutritionCommand, BaseResult>
{
    public async Task<BaseResult> Handle(DeleteNutritionCommand request, CancellationToken cancellationToken)
    {
        var nutrition = await nutritionRepository.FindByIdAsync(request.Id);

        if (nutrition is null)
        {
            return new BaseResult(new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.NutritionMessages.Không_tìm_thấy_dinh_dưỡng(request.Id)), nameof(request.Id)));
        }
        if (nutrition.IngredientNutritions.Count != 0)
        {
            return new BaseResult(new Error(ErrorCode.ConstraintViolation, translator.GetString(TranslatorMessages.NutritionMessages.Dinh_dưỡng_tồn_tại_trong_danh_sách_dinh_dưỡng_của_nguyên_liệu(request.Id)), nameof(request.Id)));
        }
        nutritionRepository.Delete(nutrition);
        await unitOfWork.SaveChangesAsync();

        return new BaseResult();
    }
}