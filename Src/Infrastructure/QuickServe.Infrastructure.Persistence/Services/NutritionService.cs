using QuickServe.Application.DTOs.Nutritions.Request;
using QuickServe.Application.Interfaces.ImageInterfaces;
using QuickServe.Application.Interfaces;
using QuickServe.Application.Interfaces.Nutritions;
using QuickServe.Application.Wrappers;
using QuickServe.Infrastructure.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuickServe.Application.Helpers;
using QuickServe.Domain.Nutritions.Entities;
using QuickServe.Utils.Enums;
using QuickServe.Infrastructure.Resources.Services;

namespace QuickServe.Infrastructure.Persistence.Services
{
    public class NutritionService : INutritionService
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageService _imageService;
        private readonly ITranslator _translator;
        private readonly IUnitOfWork _unitOfWork;
        public NutritionService(ApplicationDbContext context, IImageService imageService, ITranslator translator, IUnitOfWork unitOfWork)
        {
            _context = context;
            _imageService = imageService;
            _translator = translator;
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResult> CreateNutritionAsync(CreateNutritionRequest request)
        {
            try
            {
                if (await _context.Nutritions.AnyAsync(i => i.Name.ToLower() == request.Name.Trim().ToLower()))
                {
                    return new BaseResult(new Error(ErrorCode.NotFound, _translator.GetString(TranslatorMessages.NutritionMessages.Tên_dinh_dưỡng_đã_tồn_tại(request.Name)), nameof(request.Name)));
                }
                var nutrition = new Nutrition
                {
                    Name = request.Name,
                    Description = request.Description,
                    Vitamin = request.Vitamin,
                    HealthValue = request.HealthValue,
                    Status = (int) NutritionStatus.Active,

                };

                if (request.Image != null)
                {
                    var imageUrl = await _imageService.UploadImageAsync(request.Image);
                    nutrition.ImageUrl = imageUrl;
                }

                await _context.Nutritions.AddAsync(nutrition);
                await _unitOfWork.SaveChangesAsync();

                return new BaseResult<long>(nutrition.Id);
            }
            catch (Exception ex)
            {
                return new BaseResult($"Đã xảy ra lỗi khi tạo dinh dưỡng: {ex.Message}");
            }
        }

        public async Task<BaseResult> UpdateNutritionImageAsync(long id, UpdateNutritionImageRequest request)
        {
            try
            {
                if (id <= 0)
                {
                    return new BaseResult(new Error(ErrorCode.FieldDataInvalid, _translator.GetString(TranslatorMessages.RequestMessage.Trường_id_không_hợp_lệ(id)), nameof(id)));
                }
                var nutrition = await _context.Nutritions.FirstOrDefaultAsync(i => i.Id == id);
                if (nutrition == null)
                {
                    return new BaseResult(new Error(ErrorCode.NotFound, _translator.GetString(TranslatorMessages.NutritionMessages.Không_tìm_thấy_dinh_dưỡng(id)), nameof(id)));
                }
                if (request.Image != null)
                {
                    var imageUrl = await _imageService.UpdateImageAsync(nutrition.ImageUrl, request.Image);
                    nutrition.ImageUrl = imageUrl;
                }
                _context.Nutritions.Update(nutrition);
                await _unitOfWork.SaveChangesAsync();
                return new BaseResult();
            }
            catch (Exception ex)
            {
                return new BaseResult($"Đã xảy ra lỗi khi cập nhật ảnh dinh dưỡng: {ex.Message}");
            }
        }
    }
}
