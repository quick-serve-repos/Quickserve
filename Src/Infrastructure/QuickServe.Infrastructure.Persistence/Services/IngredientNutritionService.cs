using Azure.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QuickServe.Application.DTOs.IngredientNutrions.Request;
using QuickServe.Application.DTOs.Ingredients.Responses;
using QuickServe.Application.DTOs.Nutritions.Response;
using QuickServe.Application.Helpers;
using QuickServe.Application.Interfaces;
using QuickServe.Application.Interfaces.IngredientNutritions;
using QuickServe.Application.Wrappers;
using QuickServe.Domain.IngredientNutritions.Entities;
using QuickServe.Infrastructure.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickServe.Infrastructure.Persistence.Services
{
    public class IngredientNutritionService : IIngredientNutritionService
    {
        private readonly ApplicationDbContext _context;
        private readonly ITranslator _translator;
        private readonly IUnitOfWork _unitOfWork;

        public IngredientNutritionService(ApplicationDbContext context, ITranslator translator, IUnitOfWork unitOfWork)
        {
            _context = context;
            _translator = translator;
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResult> CreateIngredientNutritionAsync(CreateIngredientNutritionRequest request)
        {
            try
            {
                var ingredient = await _context.Ingredients.Include(c=> c.IngredientNutritions).FirstOrDefaultAsync(c => c.Id == request.IngredientId);

                if (ingredient == null)
                {
                    return new BaseResult(new Error(ErrorCode.NotFound, _translator.GetString(TranslatorMessages.IngredientMessages.Không_tìm_thấy_nguyên_liệu(request.IngredientId)), nameof(request.IngredientId)));
                }
                if(ingredient.IngredientNutritions.Count != 0)
                {
                    return new BaseResult(new Error(ErrorCode.ConstraintViolation, _translator.GetString(TranslatorMessages.IngredientMessages.Nguyên_liệu_tồn_tại_các_dinh_dưỡng(request.IngredientId)), nameof(request.IngredientId)));
                }
                foreach (var nutritionId in request.NutritionIds)
                {
                    if(await _context.Nutritions.AnyAsync(c=> c.Id ==  nutritionId) == false)
                    {
                        return new BaseResult(new Error(ErrorCode.NotFound, _translator.GetString(TranslatorMessages.NutritionMessages.Không_tìm_thấy_dinh_dưỡng(nutritionId)), nameof(nutritionId)));
                    }
                    var ingreNutri = new IngredientNutrition
                    {
                        IngredientId = request.IngredientId,
                        NutritionId = nutritionId
                    };
                    await _context.IngredientNutritions.AddAsync(ingreNutri);
                }
                await _unitOfWork.SaveChangesAsync();
                return new BaseResult();
            }
            catch (Exception ex)
            {
                return new BaseResult($"Đã xảy ra lỗi khi tạo thành phần dinh dưỡng: {ex.Message}");
            }
        }

        public async Task<BaseResult> DeleteAllNutritionAsync(long ingredientId)
        {
            try
            {
                var exists = await _context.IngredientNutritions
                    .Where(c => c.IngredientId == ingredientId).ToListAsync();
                if (exists.Any())
                {
                    _context.IngredientNutritions.RemoveRange(exists);
                }
                await _unitOfWork.SaveChangesAsync();
                return new BaseResult();
            }
            catch (Exception ex)
            {
                return new BaseResult($"Đã xảy ra lỗi khi xóa thành phần dinh dưỡng: {ex.Message}");
            }
        }

        public async Task<BaseResult> DeleteIngredientNutritionAsync(long ingredientId, DeleteNutritionInIngredientRequest request)
        {
            try
            {
                var exists = await _context.IngredientNutritions
                    .Where(c => c.IngredientId == ingredientId).ToListAsync();
                var nutrition = exists.FirstOrDefault(c=> c.NutritionId == request.NutritionId);
                if (nutrition == null)
                {
                    return new BaseResult(new Error(ErrorCode.NotFound, _translator.GetString("Không tìm thấy thành phần dinh dưỡng trong nguyên liệu"), nameof(request.NutritionId)));
                }
                _context.IngredientNutritions.Remove(nutrition);
                await _unitOfWork.SaveChangesAsync();
                return new BaseResult();
            }
            catch (Exception ex)
            {
                return new BaseResult($"Đã xảy ra lỗi khi xóa tất cả thành phần dinh dưỡng: {ex.Message}");
            }
        }

        public async Task<BaseResult> GetByIngredientIdAsync(long ingredientId)
        {
            try
            {
                var ingredient = await _context.Ingredients.Include(c=> c.IngredientNutritions)
                    .ThenInclude(c=> c.Nutrition)
                    .FirstOrDefaultAsync(c=> c.Id == ingredientId);
                if (ingredient == null)
                {
                    return new BaseResult(new Error(ErrorCode.NotFound, _translator.GetString(TranslatorMessages.IngredientMessages.Không_tìm_thấy_nguyên_liệu(ingredientId)), nameof(ingredientId)));
                }
                if(ingredient.IngredientNutritions.Count == 0)
                {
                    return new BaseResult(new Error(ErrorCode.EmptyData, _translator.GetString(TranslatorMessages.IngredientMessages.Nguyên_liệu_không_có_các_dinh_dưỡng(ingredientId)), nameof(ingredientId)));
                }
                var nutritions = new List<NutritionResponse>();
                foreach(var ingreNu in ingredient.IngredientNutritions)
                {
                    var nutrition = new NutritionResponse
                    {
                        Id = ingreNu.NutritionId,
                        Description = ingreNu.Nutrition.Description,
                        Name = ingreNu.Nutrition.Name,
                        ImageUrl = ingreNu.Nutrition.ImageUrl,
                        Vitamin = ingreNu.Nutrition.Vitamin,
                        HealthValue = ingreNu.Nutrition.HealthValue
                    };
                    nutritions.Add(nutrition);
                }
                var result = new IngredientResponse
                {
                    Id = ingredientId,
                    Name = ingredient.Name,
                    Nutritions = nutritions
                };
                return new BaseResult<IngredientResponse>(result);
            }
            catch (Exception ex)
            {
                return new BaseResult($"Đã xảy ra lỗi khi lấy thành phần dinh dưỡng bằng ingredient id: {ex.Message}");
            }
        }

        public async Task<BaseResult> UpdateIngredientNutritionAsync(CreateIngredientNutritionRequest request)
        {
            try
            {
                var ingredient = await _context.Ingredients.Include(c => c.IngredientNutritions).FirstOrDefaultAsync(c => c.Id == request.IngredientId);

                if (ingredient == null)
                {
                    return new BaseResult(new Error(ErrorCode.NotFound, _translator.GetString(TranslatorMessages.IngredientMessages.Không_tìm_thấy_nguyên_liệu(request.IngredientId)), nameof(request.IngredientId)));
                }
                var exists = await _context.IngredientNutritions
                    .Where(c => c.IngredientId == request.IngredientId).ToListAsync();
                if (exists.Any())
                {
                    _context.IngredientNutritions.RemoveRange(exists);
                }
                foreach (var nutritionId in request.NutritionIds)
                {
                    if (await _context.Nutritions.AnyAsync(c => c.Id == nutritionId) == false)
                    {
                        return new BaseResult(new Error(ErrorCode.NotFound, _translator.GetString(TranslatorMessages.NutritionMessages.Không_tìm_thấy_dinh_dưỡng(nutritionId)), nameof(nutritionId)));
                    }
                    var ingreNutri = new IngredientNutrition
                    {
                        IngredientId = request.IngredientId,
                        NutritionId = nutritionId
                    };
                    await _context.IngredientNutritions.AddAsync(ingreNutri);
                }
                await _unitOfWork.SaveChangesAsync();
                return new BaseResult();
            }
            catch (Exception ex)
            {
                return new BaseResult($"Đã xảy ra lỗi khi cập nhật thành phần dinh dưỡng: {ex.Message}");
            }
        }
    }
}
