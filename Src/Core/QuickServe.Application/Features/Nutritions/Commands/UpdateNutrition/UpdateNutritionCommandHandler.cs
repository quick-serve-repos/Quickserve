using MediatR;
using QuickServe.Application.Helpers;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Interfaces;
using QuickServe.Application.Wrappers;
using System.Threading.Tasks;
using System.Threading;

namespace QuickServe.Application.Features.Nutritions.Commands.UpdateNutrition;

public class UpdateNutritionCommandHandler(INutritionRepository nutritionRepository, IUnitOfWork unitOfWork, ITranslator translator) : IRequestHandler<UpdateNutritionCommand, BaseResult>
{
    public async Task<BaseResult> Handle(UpdateNutritionCommand request, CancellationToken cancellationToken)
    {
        if (request.Id <= 0)
        {
            return new BaseResult(new Error(ErrorCode.FieldDataInvalid, translator.GetString(TranslatorMessages.RequestMessage.Trường_id_không_hợp_lệ(request.Id)), nameof(request.Id)));
        }
        var nutrition = await nutritionRepository.GetByIdAsync(request.Id);
        if (nutrition is null)
        {
            return new BaseResult(new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.NutritionMessages.Không_tìm_thấy_dinh_dưỡng(request.Id)), nameof(request.Id)));
        }
        if (await nutritionRepository.ExistsByNameAsync(request.Name.Trim()) &&
            nutrition.Name.ToLower() != request.Name.ToLower().Trim())
        {
            return new BaseResult(new Error(ErrorCode.Duplicate, translator.GetString(TranslatorMessages.NutritionMessages.Tên_dinh_dưỡng_đã_tồn_tại(request.Name)), nameof(request.Name)));
        }
        nutrition.Update(request.Name.Trim(), request.Description, request.Vitamin, request.HealthValue);
        await unitOfWork.SaveChangesAsync();
        return new BaseResult();
    }
}