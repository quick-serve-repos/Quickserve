using MediatR;
using QuickServe.Application.Helpers;
using QuickServe.Application.Interfaces;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Utils;
using QuickServe.Application.Wrappers;
using QuickServe.Domain.Nutritions.Dtos;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace QuickServe.Application.Features.Nutritions.Queries.GetNutritionById;

public class GetNutritionByIdQueryHandler(INutritionRepository nutritionRepository, ITranslator translator) : IRequestHandler<GetNutritionByIdQuery, BaseResult<NutritionDTO>>
{

    public async Task<BaseResult<NutritionDTO>> Handle(GetNutritionByIdQuery request, CancellationToken cancellationToken)
    {
        var nutrition = await nutritionRepository.GetByIdAsync(request.Id);
        if (nutrition is null)
        {
            return new BaseResult<NutritionDTO>(new Error(ErrorCode.NotFound,
                translator.GetString(TranslatorMessages.NutritionMessages.Không_tìm_thấy_dinh_dưỡng(request.Id)),
                nameof(request.Id)));
        }

        var result = new NutritionDTO(nutrition);
        result.Created = TimeZoneConverter.ConvertToUserTimeZone(nutrition.Created);
        result.LastModified = nutrition.LastModified.HasValue
                ? TimeZoneConverter.ConvertToUserTimeZone(nutrition.LastModified.Value)
                : (DateTime?)null;
        return new BaseResult<NutritionDTO>(result);
    }
}