using MediatR;
using QuickServe.Application.Features.IngredientTypes.Queries.GetPagedListIngredientTypeByActiveStatus;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Wrappers;
using QuickServe.Domain.IngredientTypes.Dtos;
using QuickServe.Domain.ProductTemplates.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QuickServe.Application.Features.ProductTemplates.Queries.GetPagedListProductTemplateByActiveStatus
{
    public class GetPagedListProductTemplateByActiveStatusQueryHandler(IProductTemplateRepository productTemplateRepository) : IRequestHandler<GetPagedListProductTemplateByActiveStatusQuery, PagedResponse<ProductTemplateDto>>
    {
        public async Task<PagedResponse<ProductTemplateDto>> Handle(GetPagedListProductTemplateByActiveStatusQuery request, CancellationToken cancellationToken)
        {
            var result = await productTemplateRepository.GetPagedListByAcitveStatusAsync(request.PageNumber, request.PageSize, request.Name, request.StoreId);
            return new PagedResponse<ProductTemplateDto>(result, request);
        }
    }
}