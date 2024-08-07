using QuickServe.Application.DTOs.Account.Requests;
using QuickServe.Application.DTOs.Account.Responses;
using QuickServe.Application.DTOs;
using QuickServe.Application.DTOs.Ingredients.Request;
using QuickServe.Application.Interfaces.IngredientInterfaces;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Wrappers;
using QuickServe.Infrastructure.Identity.Contexts;
using QuickServe.Infrastructure.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickServe.Application.Interfaces.ImageInterfaces;
using Microsoft.AspNetCore.DataProtection.Repositories;
using QuickServe.Domain.Ingredients.Entities;
using Microsoft.EntityFrameworkCore;
using QuickServe.Application.Helpers;
using QuickServe.Infrastructure.Resources.Services;
using QuickServe.Application.Interfaces;
using QuickServe.Application.Utils.Enums;
using Azure.Core;

namespace QuickServe.Infrastructure.Persistence.Services
{
    public class IngredientService : IIngredientService
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageService _imageService;
        private readonly ITranslator _translator;
        private readonly IUnitOfWork _unitOfWork;

        public IngredientService(ApplicationDbContext context, IImageService imageService, ITranslator translator, IUnitOfWork unitOfWork)
        {
            _context = context;
            _imageService = imageService;
            _translator = translator;
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResult> CreateIngredientAsync(CreateIngredientRequest request)
        {
            try
            {
                if(await _context.Ingredients.AnyAsync(i=> i.Name.ToLower() == request.Name.Trim().ToLower())) {
                    return new BaseResult(new Error(ErrorCode.NotFound, _translator.GetString(TranslatorMessages.IngredientMessages.Tên_nguyên_liệu_đã_tồn_tại(request.Name)), nameof(request.Name)));
                }
                var ingredientType = await _context.IngredientTypes.FirstOrDefaultAsync(i => i.Id == request.IngredientTypeId);
                if (ingredientType == null)
                {
                    return new BaseResult(new Error(ErrorCode.NotFound, _translator.GetString(TranslatorMessages.IngredientTypeMessages.Không_tìm_thấy_loại_nguyên_liệu(request.IngredientTypeId)), nameof(request.IngredientTypeId)));
                }
                var ingredient = new Ingredient
                {
                    Name = request.Name,
                    Price = request.Price,
                    Calo = request.Calo,
                    DefaultQuantity = request.DefaultQuantity,
                    Description = request.Description,
                    IngredientTypeId = request.IngredientTypeId,
                    Status = (int)IngredientStatus.Active,
                    IngredientType = ingredientType
                    
                };

                if (request.Image != null)
                {
                    var imageUrl = await _imageService.UploadImageAsync(request.Image);
                    ingredient.ImageUrl = imageUrl;
                }

                await _context.Ingredients.AddAsync(ingredient);
                await _unitOfWork.SaveChangesAsync();

                return new BaseResult<long>(ingredient.Id);
            }
            catch (Exception ex)
            {
                return new BaseResult($"An error occurred while creating the ingredient: {ex.Message}");
            }
        }

        public async Task<BaseResult> UpdateIngredientImageAsync(long id, UpdateIngredientImageRequest request)
        {
            try
            {
                var ingredient = await _context.Ingredients.FirstOrDefaultAsync(i => i.Id == id);
                if (ingredient == null)
                {
                    return new BaseResult(new Error(ErrorCode.NotFound, _translator.GetString(TranslatorMessages.IngredientMessages.Không_tìm_thấy_nguyên_liệu(id)), nameof(id)));
                }
                if (request.Image != null)
                {
                    var imageUrl = await _imageService.UpdateImageAsync(ingredient.ImageUrl, request.Image);
                    ingredient.ImageUrl = imageUrl;
                }
                _context.Ingredients.Update(ingredient);
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
