using System.Threading;
using System.Threading.Tasks;
using MediatR;
using QuickServe.Application.Helpers;
using QuickServe.Application.Interfaces;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Wrappers;
using QuickServe.Domain.ProductTemplates.Dtos;

namespace QuickServe.Application.Features.ProductTemplates.Queries.GetProductTemplateById;

public class GetProductTemplateByIdQueryHandler(IProductTemplateRepository productTemplateRepository, ITranslator translator)  : IRequestHandler<GetProductTemplateByIdQuery, BaseResult<ProductTemplateDto>>
{
    public async Task<BaseResult<ProductTemplateDto>> Handle(GetProductTemplateByIdQuery request, CancellationToken cancellationToken)
    {
        var productTemplate = await productTemplateRepository.GetProductTemplateByIdAsync(request.Id);
        if (productTemplate is null)
        {
            return new BaseResult<ProductTemplateDto>(new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.ProductTemplateMessages.Không_tìm_thấy_mẫu_sản_phẩm(request.Id)), nameof(request.Id)));
        }

        var result = new ProductTemplateDto(productTemplate);
        return new BaseResult<ProductTemplateDto>(result);
    }
}