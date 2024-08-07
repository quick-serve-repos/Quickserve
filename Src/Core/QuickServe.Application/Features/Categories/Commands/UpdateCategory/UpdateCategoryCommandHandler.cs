using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using QuickServe.Application.Helpers;
using QuickServe.Application.Interfaces;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Wrappers;

namespace QuickServe.Application.Features.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork, ITranslator translator) : IRequestHandler<UpdateCategoryCommand, BaseResult>
{
    public async Task<BaseResult> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        if(request.Id <= 0)
        {
            return new BaseResult(new Error(ErrorCode.FieldDataInvalid, translator.GetString(TranslatorMessages.RequestMessage.Trường_id_không_hợp_lệ(request.Id)), nameof(request.Id)));
        }
        var category = await categoryRepository.GetByIdAsync(request.Id);

        if (category is null)
        {
            return new BaseResult(new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.CategoryMessages.Không_tìm_thấy_danh_mục(request.Id)), nameof(request.Id)));
        }
        if (await categoryRepository.ExistsCategoryByNameAsync(request.Name.Trim()) && category.Name.ToLower() != request.Name.Trim().ToLower())
        {
            return new BaseResult(new Error(ErrorCode.Duplicate, translator.GetString(TranslatorMessages.CategoryMessages.Tên_danh_mục_đã_tồn_tại(request.Name)), nameof(request.Name)));
        }
        category.Update(request.Name.Trim());
        await unitOfWork.SaveChangesAsync();
        return new  BaseResult();
    }
}