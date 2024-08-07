using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickServe.Application.DTOs.ProductTemplates.Request
{
    public class GetAllTemplateRequest
    {
        public long ProductTemplateId { get; set; }

    }
    public class GetAllTemplateRequestValidator : AbstractValidator<GetAllTemplateRequest>
    {
        public GetAllTemplateRequestValidator()
        {
            RuleFor(x => x.ProductTemplateId)
                 .NotEmpty().WithMessage("Mẫu sản phẩm là bắt buộc.")
                 .GreaterThan(0).WithMessage("Id mẫu sản phẩm phải lớn hơn 0.");
        }
    }
}
