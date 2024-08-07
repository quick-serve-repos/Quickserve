using System.Threading;
using System.Threading.Tasks;
using MediatR;
using QuickServe.Application.Helpers;
using QuickServe.Application.Interfaces;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Utils.Enums;
using QuickServe.Application.Wrappers;
using QuickServe.Domain.Categories.Entities;

namespace QuickServe.Application.Features.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork, ITranslator translator) : IRequestHandler<CreateCategoryCommand, BaseResult>
{
    public async Task<BaseResult> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        if (await categoryRepository.ExistsCategoryByNameAsync(request.Name.Trim()))
        {
            return new BaseResult(new Error(ErrorCode.Duplicate, translator.GetString(TranslatorMessages.CategoryMessages.Tên_danh_mục_đã_tồn_tại(request.Name)), nameof(request.Name)));
        }
        var category = new Category(request.Name.Trim());
        category.Status = (int) CategoryStatus.Active;
         await categoryRepository.AddAsync(category);
        await unitOfWork.SaveChangesAsync();
        return new BaseResult<long>(category.Id);
    }
}