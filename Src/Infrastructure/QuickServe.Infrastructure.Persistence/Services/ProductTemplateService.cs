using QuickServe.Application.DTOs.Ingredients.Request;
using QuickServe.Application.DTOs.ProductTemplates;
using QuickServe.Application.Interfaces.ImageInterfaces;
using QuickServe.Application.Interfaces;
using QuickServe.Application.Interfaces.IProductTemplateServices;
using QuickServe.Application.Wrappers;
using QuickServe.Infrastructure.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuickServe.Application.Helpers;
using QuickServe.Application.Utils.Enums;
using QuickServe.Domain.Ingredients.Entities;
using QuickServe.Domain.ProductTemplates.Entities;

namespace QuickServe.Infrastructure.Persistence.Services
{
    public class ProductTemplateService : IProductTemplateService
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageService _imageService;
        private readonly ITranslator _translator;
        private readonly IUnitOfWork _unitOfWork;

        public ProductTemplateService(ApplicationDbContext context, IImageService imageService, ITranslator translator, IUnitOfWork unitOfWork)
        {
            _context = context;
            _imageService = imageService;
            _translator = translator;
            _unitOfWork = unitOfWork;
        }
        public async Task<BaseResult> CreateProductTemplateAsync(CreateProductTemplateRequest request)
        {
            try
            {
                if (await _context.ProductTemplates.AnyAsync(i => i.Name.ToLower() == request.Name.Trim().ToLower()))
                {
                    return new BaseResult(new Error(ErrorCode.Duplicate, _translator.GetString(TranslatorMessages.ProductTemplateMessages.Tên_mẫu_sản_phẩm_đã_tồn_tại(request.Name)), nameof(request.Name)));
                }
                var category = await _context.Categories.FirstOrDefaultAsync(i => i.Id == request.CategoryId);
                if (category == null)
                {
                    return new BaseResult(new Error(ErrorCode.NotFound, _translator.GetString(TranslatorMessages.CategoryMessages.Không_tìm_thấy_danh_mục(request.CategoryId)), nameof(request.CategoryId)));
                }
                var productTemplate = new ProductTemplate
                {
                    Name = request.Name,
                    Price = 0,
                    Size = request.Size,
                    Description = request.Description,
                    CategoryId = request.CategoryId,
                    Status = (int)ProductTemplateStatus.Inactive,
                    Category = category
                };

                if (request.Image != null)
                {
                    var imageUrl = await _imageService.UploadImageAsync(request.Image);
                    productTemplate.ImageUrl = imageUrl;
                }

                await _context.ProductTemplates.AddAsync(productTemplate);
                await _unitOfWork.SaveChangesAsync();

                return new BaseResult<long>(productTemplate.Id);
            }
            catch (Exception ex)
            {
                return new BaseResult($"An error occurred while creating the productTemplate: {ex.Message}");
            }
        }

        public async Task<BaseResult> UpdateProductTemplateImageAsync(long id, UpdateProductTemplateImageRequest request)
        {
            try
            {
                var productTemplate = await _context.ProductTemplates.FirstOrDefaultAsync(i => i.Id == id);
                if (productTemplate == null)
                {
                    return new BaseResult(new Error(ErrorCode.NotFound, _translator.GetString(TranslatorMessages.ProductTemplateMessages.Không_tìm_thấy_mẫu_sản_phẩm(id)), nameof(id)));
                }
                if (request.Image != null)
                {
                    var imageUrl = await _imageService.UpdateImageAsync(productTemplate.ImageUrl, request.Image);
                    productTemplate.ImageUrl = imageUrl;
                }
                _context.ProductTemplates.Update(productTemplate);
                await _unitOfWork.SaveChangesAsync();
                return new BaseResult();
            }
            catch (Exception ex)
            {
                return new BaseResult($"An error occurred while creating the ingredient: {ex.Message}");
            }
        }
    }
}
