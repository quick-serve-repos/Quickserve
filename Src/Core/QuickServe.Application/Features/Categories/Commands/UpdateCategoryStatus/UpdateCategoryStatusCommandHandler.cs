using MediatR;
using QuickServe.Application.Features.Categories.Commands.UpdateCategory;
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
using QuickServe.Application.Utils.Enums;

namespace QuickServe.Application.Features.Categories.Commands.UpdateCategoryStatus
{
    public class UpdateCategoryStatusCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork, ITranslator translator) : IRequestHandler<UpdateCategoryStatusCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(UpdateCategoryStatusCommand request, CancellationToken cancellationToken)
        {
            var category = await categoryRepository.FindByIdAsync(request.Id);

            if (category is null)
            {
                return new BaseResult(new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.CategoryMessages.Không_tìm_thấy_danh_mục(request.Id)), nameof(request.Id)));
            }
            if(category.Status ==(int) CategoryStatus.Active)
            {
                category.Status = (int)CategoryStatus.Inactive;
            }
            else
            {
                category.Status = (int)CategoryStatus.Active;
            }
            if(category.ProductTemplates.Count != 0)
            {
                foreach (var t in category.ProductTemplates)
                {
                    t.Status = category.Status;
                }
            }
           
            category.Update(category.Status);
            await unitOfWork.SaveChangesAsync();
            return new BaseResult();
        }
    }
}