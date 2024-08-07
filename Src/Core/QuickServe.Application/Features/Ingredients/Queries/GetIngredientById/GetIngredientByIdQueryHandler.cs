using QuickServe.Application.Helpers;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Interfaces;
using QuickServe.Application.Utils;
using QuickServe.Application.Wrappers;
using System.Threading.Tasks;
using System.Threading;
using System;
using MediatR;
using QuickServe.Domain.Ingredients.Dtos;
using QuickServe.Domain.Categories.Dtos;

namespace QuickServe.Application.Features.Ingredients.Queries.GetIngredientById;

public class GetIngredientByIdQueryHandler(IIngredientRepository ingredientRepository, ITranslator translator) : IRequestHandler<GetIngredientByIdQuery, BaseResult<IngredientDTO>>
{

    public async Task<BaseResult<IngredientDTO>> Handle(GetIngredientByIdQuery request, CancellationToken cancellationToken)
    {

        var result = await ingredientRepository.GetIngredientByIdAsync(request.Id);
        if(result == null)
        {
            return new BaseResult<IngredientDTO>(new Error(ErrorCode.NotFound,
                translator.GetString(TranslatorMessages.IngredientMessages.Không_tìm_thấy_nguyên_liệu(request.Id)),
                nameof(request.Id)));
        }
        result.Created = TimeZoneConverter.ConvertToUserTimeZone(result.Created);
        result.LastModified = result.LastModified.HasValue
                ? TimeZoneConverter.ConvertToUserTimeZone(result.LastModified.Value)
                : (DateTime?)null;
        var response = new IngredientDTO(result);
        return new BaseResult<IngredientDTO>(response);
    }
}