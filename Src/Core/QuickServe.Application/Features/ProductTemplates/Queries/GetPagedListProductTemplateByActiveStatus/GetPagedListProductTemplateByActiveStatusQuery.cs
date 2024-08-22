using MediatR;
using QuickServe.Application.Parameters;
using QuickServe.Application.Wrappers;
using QuickServe.Domain.ProductTemplates.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickServe.Application.Features.ProductTemplates.Queries.GetPagedListProductTemplateByActiveStatus
{
    public class GetPagedListProductTemplateByActiveStatusQuery : PagenationRequestParameter, IRequest<PagedResponse<ProductTemplateDto>>
    {
        public string Name { get; set; }
        public long StoreId { get; set; }
    }
}