using MediatR;
using QuickServe.Application.Features.Ingredients.Commands.UpdateIngredient;
using QuickServe.Application.Helpers;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Interfaces;
using QuickServe.Application.Wrappers;
using System.Threading.Tasks;
using System.Threading;

namespace QuickServe.Application.Features.ProductTemplates.Commands.UpdateProductTemplate;

public class UpdateProductTemplateCommandHandler(IProductTemplateRepository productTemplateRepository, IUnitOfWork unitOfWork, ITranslator translator) : IRequestHandler<UpdateProductTemplateCommand, BaseResult>
{
    public async Task<BaseResult> Handle(UpdateProductTemplateCommand request, CancellationToken cancellationToken)
    {
        if (request.Id <= 0)
        {
            return new BaseResult(new Error(ErrorCode.FieldDataInvalid, translator.GetString(TranslatorMessages.RequestMessage.Trường_id_không_hợp_lệ(request.Id)), nameof(request.Id)));
        }
        var productTemplate = await productTemplateRepository.GetByIdAsync(request.Id);

        if (productTemplate is null)
        {
            return new BaseResult(new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.ProductTemplateMessages.Không_tìm_thấy_mẫu_sản_phẩm(request.Id)), nameof(request.Id)));
        }
        if (await productTemplateRepository.ExistByNameAsync(request.Name.Trim()) &&
            productTemplate.Name.ToLower() != request.Name.ToLower().Trim())
        {
            return new BaseResult(new Error(ErrorCode.Duplicate, translator.GetString(TranslatorMessages.ProductTemplateMessages.Tên_mẫu_sản_phẩm_đã_tồn_tại(request.Name)), nameof(request.Name)));
        }
        productTemplate.Update(request.Name.Trim(), request.Size, request.Description
            , request.CategoryId);
        await unitOfWork.SaveChangesAsync();
        return new BaseResult();
    }
}