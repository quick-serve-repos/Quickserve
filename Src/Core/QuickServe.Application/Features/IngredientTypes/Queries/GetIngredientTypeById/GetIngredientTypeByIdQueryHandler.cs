using MediatR;
using QuickServe.Application.Helpers;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Interfaces;
using QuickServe.Application.Utils;
using QuickServe.Application.Wrappers;
using System.Threading.Tasks;
using System.Threading;
using System;
using QuickServe.Domain.IngredientTypes.Dtos;

namespace QuickServe.Application.Features.IngredientTypes.Queries.GetIngredientTypeById;

public class GetIngredientTypeByIdQueryHandler(IIngredientTypeRepository ingredientTypeRepository, ITranslator translator) : IRequestHandler<GetIngredientTypeByIdQuery, BaseResult<IngredientTypeDTO>>
{

    public async Task<BaseResult<IngredientTypeDTO>> Handle(GetIngredientTypeByIdQuery request, CancellationToken cancellationToken)
    {
        var ingredientType = await ingredientTypeRepository.GetByIdAsync(request.Id);
        if (ingredientType is null)
        {
            return new BaseResult<IngredientTypeDTO>(new Error(ErrorCode.NotFound,
                translator.GetString(TranslatorMessages.IngredientTypeMessages.Không_tìm_thấy_loại_nguyên_liệu(request.Id)),
                nameof(request.Id)));
        }

        var result = new IngredientTypeDTO(ingredientType);
        result.Created = TimeZoneConverter.ConvertToUserTimeZone(ingredientType.Created);
        result.LastModified = ingredientType.LastModified.HasValue
                ? TimeZoneConverter.ConvertToUserTimeZone(ingredientType.LastModified.Value)
                : (DateTime?)null;
        return new BaseResult<IngredientTypeDTO>(result);
    }
}