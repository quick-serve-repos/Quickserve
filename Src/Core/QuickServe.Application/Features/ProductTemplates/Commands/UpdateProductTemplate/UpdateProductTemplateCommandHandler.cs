﻿using MediatR;
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
        var ingredient = await productTemplateRepository.GetByIdAsync(request.Id);

        if (ingredient is null)
        {
            return new BaseResult(new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.ProductTemplateMessages.Mẫu_sản_phẩm_không_tìm_thấy_với_id(request.Id)), nameof(request.Id)));
        }
        if (await productTemplateRepository.ExistByNameAsync(request.Name.Trim()))
        {
            return new BaseResult(new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.ProductTemplateMessages.Tên_mẫu_sản_phẩm_đã_tồn_tại_với_tên(request.Name)), nameof(request.Name)));
        }
        ingredient.Update(request.Name.Trim(), request.Price, request.Size, request.Description
            , request.CategoryId);
        await unitOfWork.SaveChangesAsync();
        return new BaseResult();
    }
}