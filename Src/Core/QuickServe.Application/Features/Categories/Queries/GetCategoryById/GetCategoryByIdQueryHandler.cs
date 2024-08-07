using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using QuickServe.Application.Helpers;
using QuickServe.Application.Interfaces;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Utils;
using QuickServe.Application.Wrappers;
using QuickServe.Domain.Categories.Dtos;

namespace QuickServe.Application.Features.Categories.Queries.GetCategoryById;

public class GetCategoryByIdQueryHandler(ICategoryRepository categoryRepository, ITranslator translator) : IRequestHandler<GetCategoryByIdQuery ,BaseResult<CategoryDto>>
{
    
    public async Task<BaseResult<CategoryDto>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(request.Id);
        if (category is null)
        {
            return new BaseResult<CategoryDto>(new Error(ErrorCode.NotFound,
                translator.GetString(TranslatorMessages.CategoryMessages.Không_tìm_thấy_danh_mục(request.Id)),
                nameof(request.Id)));
        }

        var result = new CategoryDto(category);
        result.Created = TimeZoneConverter.ConvertToUserTimeZone(category.Created);
        result.LastModified = category.LastModified.HasValue
                ? TimeZoneConverter.ConvertToUserTimeZone(category.LastModified.Value)
                : (DateTime?)null;
        return new BaseResult<CategoryDto>(result);
    }
}