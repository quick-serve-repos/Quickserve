using MediatR;
using QuickServe.Application.Features.Categories.Queries.GetCategoryById;
using QuickServe.Application.Helpers;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Interfaces;
using QuickServe.Application.Utils;
using QuickServe.Application.Wrappers;
using QuickServe.Domain.Categories.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using QuickServe.Domain.TemplateSteps.Dtos;

namespace QuickServe.Application.Features.TemplateSteps.Queries.GetTemplateStepById
{
    public class GetTemplateStepByIdQueryHandler(ITemplateStepRepository templateStepRepository, ITranslator translator) : IRequestHandler<GetTemplateStepByIdQuery, BaseResult<TemplateStepDTO>>
    {

        public async Task<BaseResult<TemplateStepDTO>> Handle(GetTemplateStepByIdQuery request, CancellationToken cancellationToken)
        {
            var templateStep = await templateStepRepository.GetByIdAsync(request.Id);
            if (templateStep is null)
            {
                return new BaseResult<TemplateStepDTO>(new Error(ErrorCode.NotFound,
                    translator.GetString(TranslatorMessages.TemplateStepMessages.Không_tìm_thấy_bước_mẫu(request.Id)),
                    nameof(request.Id)));
            }

            var result = new TemplateStepDTO(templateStep);
            result.Created = TimeZoneConverter.ConvertToUserTimeZone(templateStep.Created);
            result.LastModified = templateStep.LastModified.HasValue
                    ? TimeZoneConverter.ConvertToUserTimeZone(templateStep.LastModified.Value)
                    : (DateTime?)null;
            return new BaseResult<TemplateStepDTO>(result);
        }
    }
}