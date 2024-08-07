using MediatR;
using QuickServe.Application.Features.Categories.Commands.CreateCategory;
using QuickServe.Application.Helpers;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Interfaces;
using QuickServe.Application.Utils.Enums;
using QuickServe.Application.Wrappers;
using QuickServe.Domain.Categories.Entities;
using System.Threading.Tasks;
using System.Threading;
using QuickServe.Domain.IngredientTypes.Entities;

namespace QuickServe.Application.Features.IngredientTypes.Commands.CreateIngredientType;

public class CreateIngredientTypeCommandHandler(IIngredientTypeRepository ingredientTypeRepository, IUnitOfWork unitOfWork, ITranslator translator) : IRequestHandler<CreateIngredientTypeCommand, BaseResult>
{
    public async Task<BaseResult> Handle(CreateIngredientTypeCommand request, CancellationToken cancellationToken)
    {
        if (await ingredientTypeRepository.ExistByNameAsync(request.Name.Trim()))
        {
            return new BaseResult(new Error(ErrorCode.Duplicate, translator.GetString(TranslatorMessages.CategoryMessages.Tên_danh_mục_đã_tồn_tại(request.Name)), nameof(request.Name)));
        }
        var ingredientType = new IngredientType(request.Name.Trim());
        ingredientType.Status = (int)IngredientTypeStatus.Active;
        await ingredientTypeRepository.AddAsync(ingredientType);
        await unitOfWork.SaveChangesAsync();
        return new BaseResult<long>(ingredientType.Id);
    }
}