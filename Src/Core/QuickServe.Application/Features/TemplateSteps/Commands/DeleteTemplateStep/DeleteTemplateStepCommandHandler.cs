using MediatR;
using QuickServe.Application.Features.Categories.Commands.DeleteCategory;
using QuickServe.Application.Helpers;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Interfaces;
using QuickServe.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QuickServe.Application.Features.TemplateSteps.Commands.DeleteTemplateStep
{
    public class DeleteTemplateStepCommandHandler(ITemplateStepRepository templateStepRepository, IUnitOfWork unitOfWork, ITranslator translator) : IRequestHandler<DeleteTemplateStepCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(DeleteTemplateStepCommand request, CancellationToken cancellationToken)
        {
            var templateStep = await templateStepRepository.FindByIdAsync(request.Id);

            if (templateStep is null)
            {
                return new BaseResult(new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.TemplateStepMessages.Không_tìm_thấy_bước_mẫu(request.Id)), nameof(request.Id)));
            }
            if (templateStep.IngredientTypeTemplateSteps.Count != 0)
            {
                return new BaseResult(new Error(ErrorCode.ConstraintViolation, translator.GetString(TranslatorMessages.TemplateStepMessages.Bước_mẫu_tồn_tại_loại_nguyên_liệu(request.Id)), nameof(request.Id)));
            }
            templateStepRepository.Delete(templateStep);
            await unitOfWork.SaveChangesAsync();

            return new BaseResult();
        }
    }
}
