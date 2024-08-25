using QuickServe.Application.DTOs.ProductTemplates.Request;
using QuickServe.Application.Interfaces;
using QuickServe.Application.Interfaces.IngredientTypeTemplateSteps;
using QuickServe.Application.Wrappers;
using QuickServe.Infrastructure.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuickServe.Application.Helpers;
using QuickServe.Domain.IngredientTypeTemplateSteps.Entities;
using QuickServe.Application.DTOs.ProductTemplates.Response;
using QuickServe.Application.DTOs.IngredientTypeTemplateSteps.Response;
using QuickServe.Application.DTOs.IngredientTypeTemplateSteps.Request;
using QuickServe.Application.Utils.Enums;
using System.ComponentModel.Design;
using QuickServe.Application.DTOs.Ingredients.Responses;
using QuickServe.Application.Features.TemplateSteps.Commands.CreateTemplateStep;
using QuickServe.Domain.TemplateSteps.Entities;
using System.Net;
using QuickServe.Domain.ProductTemplates.Dtos;
using Azure.Core;
using MediatR;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Domain.IngredientTypes.Entities;

namespace QuickServe.Infrastructure.Persistence.Services
{
    public class IngredientTypeTemplateStepService : IIngredientTypeTemplateStepService
    {
        private readonly ApplicationDbContext _context;
        private readonly ITranslator _translator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISessionRepository _sessionRepository;
        private readonly IIngredientSessionRepository _ingredientSessionRepository;
        public IngredientTypeTemplateStepService(ApplicationDbContext context, ITranslator translator, IUnitOfWork unitOfWork,
            ISessionRepository sessionRepository, IIngredientSessionRepository ingredientSessionRepository)
        {
            _context = context;
            _translator = translator;
            _unitOfWork = unitOfWork;
            _sessionRepository = sessionRepository;
            _ingredientSessionRepository = ingredientSessionRepository;
        }
        public async Task<BaseResult> CreateTempalte(CreateTemplateStepCommand request)
        {
            try
            {
                var productTemplate = await _context.ProductTemplates
                    .Include(c => c.TemplateSteps).ThenInclude(c => c.IngredientTypeTemplateSteps)
                    .ThenInclude(c => c.IngredientType)
                    .FirstOrDefaultAsync(c=> c.Id == request.ProductTemplateId);
                if(productTemplate == null)
                {
                    return new BaseResult(new Error(ErrorCode.NotFound, 
                        _translator.GetString(TranslatorMessages.ProductTemplateMessages
                        .Không_tìm_thấy_mẫu_sản_phẩm(request.ProductTemplateId)), 
                        nameof(request.ProductTemplateId)));
                }


                if (await _context.TemplateSteps
                    .AnyAsync(c => c.Name.ToLower() == request.Name.ToLower().Trim() && c.ProductTemplateId == request.ProductTemplateId))
                {
                    return new BaseResult(new Error(ErrorCode.NotFound,
                        _translator.GetString(TranslatorMessages.TemplateStepMessages.Tên_bước_mẫu_đã_tồn_tại(request.Name)),
                        nameof(request.Name)));

                }
                foreach (var st in productTemplate.TemplateSteps) {
                    foreach (var t in request.IngredientTypes)
                    {
                        if(st.IngredientTypeTemplateSteps.Any(c => c.IngredientTypeId == t.IngredientTypeId))
                        {
                            return new BaseResult(new Error(ErrorCode.FieldDataInvalid, _translator.GetString("Loại nguyên liệu đã tồn tại trong bước khác.")));
                        }
                    }
                }
                var result = new TemplateStep
                {
                    Name = request.Name.Trim(),
                    ProductTemplateId = request.ProductTemplateId,
                };
               
                foreach (var newIngredientType in request.IngredientTypes)
                {
                    var count = 0;
                    var ingredientType = await _context.IngredientTypes.Include(i=> i.Ingredients)
                        .FirstOrDefaultAsync(i => i.Id == newIngredientType.IngredientTypeId);
                    if (ingredientType == null)
                    {
                        return new BaseResult(new Error(ErrorCode.NotFound, _translator.GetString(TranslatorMessages.IngredientTypeMessages.Không_tìm_thấy_loại_nguyên_liệu(newIngredientType.IngredientTypeId)), nameof(newIngredientType.IngredientTypeId)));
                    }
                    if (!ingredientType.Ingredients.Any())
                    {
                        return new BaseResult(new Error(ErrorCode.NotFound, _translator.GetString(ingredientType.Name +" chưa có nguyên liệu. Hãy thêm nguyên liệu.")));

                    }
                    if(ingredientType.Ingredients.Count()< newIngredientType.QuantityMax)
                    {
                        return new BaseResult(new Error(ErrorCode.FieldDataInvalid, _translator.GetString(ingredientType.Name + " không đủ nguyên liệu. Chọn lại số lượng lớn nhất.")));
                    }
                    foreach (var ingre in ingredientType.Ingredients)
                    {
                        if(ingre.DefaultQuantity > 0)
                        {
                            count++;
                        }
                    }
                    if(count > newIngredientType.QuantityMax)
                    {
                        return new BaseResult(new Error(ErrorCode.FieldDataInvalid, 
                            _translator.GetString(ingredientType.Name + " chứa "+count+" nguyên liệu mặc định. Chọn lại số lượng lớn nhất.")));
                    }
                    await _context.TemplateSteps.AddAsync(result);
                    await _unitOfWork.SaveChangesAsync();
                    var ingredientStep = new IngredientTypeTemplateStep
                    {
                        TemplateStepId = result.Id,
                        IngredientTypeId = newIngredientType.IngredientTypeId,
                        QuantityMax = newIngredientType.QuantityMax,
                        QuantityMin = newIngredientType.QuantityMin,
                    };
                    await _context.IngredientTypeTemplateSteps.AddAsync(ingredientStep);
                }
                result.Update((int)TemplateStepStatus.Active);


                await _unitOfWork.SaveChangesAsync();
                return new BaseResult();
            }
            catch (Exception ex)
            {
                return new BaseResult($"Đã xảy ra lỗi khi tạo mẫu: {ex.Message}");
            }
    
        }

       

        public async Task<BaseResult> DeleteTemplate(DeleteTemplateRequest request)
        {
            try
            {
                var templateStep = await _context.TemplateSteps.Include(c=> c.ProductTemplate).FirstOrDefaultAsync(c => c.Id == request.TemplateStepId);
                if (templateStep == null)
                {
                    return new BaseResult(new Error(ErrorCode.NotFound, _translator.GetString(TranslatorMessages.TemplateStepMessages.Không_tìm_thấy_bước_mẫu(request.TemplateStepId)), nameof(request.TemplateStepId)));
                }
                var existsTemplate = await _context.IngredientTypeTemplateSteps
                    .Where(c => c.TemplateStepId == request.TemplateStepId).ToListAsync();
                if (existsTemplate.Any())
                {
                    _context.IngredientTypeTemplateSteps.RemoveRange(existsTemplate);
                }
                _context.TemplateSteps.Remove(templateStep);
                templateStep.ProductTemplate.Status = (int)ProductTemplateStatus.Inactive;
                await _unitOfWork.SaveChangesAsync();
                return new BaseResult();
            }
            catch (Exception ex) {
                return new BaseResult($"Đã xảy ra lỗi khi xóa mẫu: {ex.Message}");
            }
        }

        public async Task<BaseResult> GetAll(GetAllTemplateRequest request)
        {
            try
            {
                var productTemplate = await _context.ProductTemplates
                    .Include(c=> c.TemplateSteps).ThenInclude(c=> c.IngredientTypeTemplateSteps)
                    .ThenInclude(c=> c.IngredientType)
                    .ThenInclude(c=> c.Ingredients)
                    .FirstOrDefaultAsync(c => c.Id == request.ProductTemplateId);
                
                if (productTemplate == null)
                {
                    return new BaseResult(new Error(ErrorCode.NotFound, _translator.GetString(TranslatorMessages.ProductTemplateMessages.Không_tìm_thấy_mẫu_sản_phẩm(request.ProductTemplateId)), nameof(request.ProductTemplateId)));
                }
                var templates = new List<TemplateResponse>();
               
                
                foreach (var ts in productTemplate.TemplateSteps)
                {
                    var templateStep = new TemplateResponse(ts);
                    var its = new List<IngredientTypeResponse>();
                    var ingredients = new List<IngredientInfoResponse>();
                    foreach (var it in ts.IngredientTypeTemplateSteps)
                    {
                        var ingreStep = new IngredientTypeResponse(it);
                       
                        foreach(var ingredient in it.IngredientType.Ingredients.
                            Where(c=>c.Status == (int)IngredientStatus.Active)) {
                            var ingredientRes = new IngredientInfoResponse(ingredient);
                            var remainQuantity = 0;
                            var isSold = true;
                            var sessions = await _sessionRepository.GetAllAsync();
                            var currentSession = sessions.FirstOrDefault(x => x.StartTime <= DateTime.Now.AddHours(7).TimeOfDay && x.EndTime >= DateTime.Now.AddHours(7).TimeOfDay);

                            if (currentSession != null)
                            {
                                var ingredientSession = await _ingredientSessionRepository.GetByIdAsync(ingredient.Id, currentSession.Id);

                                if (ingredientSession != null)
                                {
                                    remainQuantity = ingredientSession.Quantity - ingredientSession.SoldQuantity;
                                    if (remainQuantity > 0)
                                    {
                                        isSold = true;
                                    }
                                    else
                                    {
                                        isSold = false;
                                    }
                                }
                            }
                            ingredientRes.RemainingQuantity = remainQuantity;
                            ingredientRes.IsSold = isSold;
                            ingredients.Add(ingredientRes);
                        }
                        ingreStep.Ingredients = ingredients;
                        its.Add(ingreStep);
                    }
                    templateStep.IngredientTypes = its;
                    templates.Add(templateStep);
                }
                var result = new ProductTemplateResponse
                {
                    Id = productTemplate.Id,
                    Name = productTemplate.Name,
                    Price = productTemplate.Price,
                    Templates = templates,
                };

                return new BaseResult<ProductTemplateResponse>(result);
            }
            catch (Exception ex)
            {
                return new BaseResult($"Đã xảy ra lỗi khi lấy tất cả mẫu: {ex.Message}");
            }
        }

        public async Task<BaseResult> GetById(GetTemplateByIdRequest request)
        {
            try
            { 
                var templateStep = await _context.TemplateSteps.Include(c => c.IngredientTypeTemplateSteps)
                    .ThenInclude(c => c.IngredientType)
                    .FirstOrDefaultAsync(c=> c.Id == request.TemplateStepId);
                if (templateStep == null)
                {
                    return new BaseResult(new Error(ErrorCode.NotFound, _translator.GetString(TranslatorMessages.TemplateStepMessages.Không_tìm_thấy_bước_mẫu(request.TemplateStepId)), nameof(request.TemplateStepId)));
                }

                var its = new List<IngredientTypeResponse>();
                foreach (var it in templateStep.IngredientTypeTemplateSteps)
                {
                    var ingreStep = new IngredientTypeResponse(it);
                    var ingredients = new List<IngredientInfoResponse>();
                    foreach (var ingredient in it.IngredientType.Ingredients.
                        Where(c => c.Status == (int)IngredientStatus.Active))
                    {
                        var ingredientRes = new IngredientInfoResponse(ingredient);
                        ingredients.Add(ingredientRes);
                    }
                    ingreStep.Ingredients = ingredients;
                    its.Add(ingreStep);
                }
                var resutl = new TemplateResponse(templateStep);
                resutl.IngredientTypes = its;
                return new BaseResult<TemplateResponse>(resutl);
            }
            catch (Exception ex)
            {
                return new BaseResult($"Đã xảy ra lỗi khi lấy mẫu: {ex.Message}");
            }
        }

        public async Task<BaseResult> GetIngredients(long ingredientTypeId)
        {
            try
            {
                var ingredientType = await _context.IngredientTypes
                    .Include(c => c.Ingredients)
                    .FirstOrDefaultAsync(c=> c.Id == ingredientTypeId);
                if (ingredientType == null)
                {
                    return new BaseResult(new Error(ErrorCode.NotFound, _translator.GetString("Không tìm thấy loại nguyên liệu")));
                }
                var ingredients = new List<GetIngredientsResponse>();
                foreach (var ingredient in ingredientType.Ingredients.
                    Where(c => c.Status == (int)IngredientStatus.Active))
                {
                    var remainQuantity = 0;
                    var isSold = true;
                    var sessions = await _sessionRepository.GetAllAsync();
                    var currentSession = sessions.FirstOrDefault(x => x.StartTime <= DateTime.Now.TimeOfDay && x.EndTime >= DateTime.Now.TimeOfDay);

                    if (currentSession != null)
                    {
                        var ingredientSession = await _ingredientSessionRepository.GetByIdAsync(ingredient.Id, currentSession.Id);
                       
                        if (ingredientSession != null)
                        {
                           remainQuantity = ingredientSession.Quantity - ingredientSession.SoldQuantity;
                            if (remainQuantity > 0)
                            {
                                isSold = true;
                            }
                            else { 
                                isSold=false;
                            }
                        }
                    }
                    var ingredientRes = new GetIngredientsResponse
                    {
                        Id = ingredient.Id,
                        Name = ingredient.Name,
                        ImageUrl = ingredient.ImageUrl,
                        DefaultQuantity = ingredient.DefaultQuantity,
                        QuantityMax = ingredient.QuantityMax,
                        Price = ingredient.Price,
                        RemainingQuantity = remainQuantity,
                        IsSold = isSold
                    };
                    ingredients.Add(ingredientRes);
                }
                return new BaseResult<List<GetIngredientsResponse>>(ingredients);
            }
            catch (Exception ex)
            {
                return new BaseResult($"Đã xảy ra lỗi khi lấy các nguyên liệu: {ex.Message}");
            }
        }

        public async Task<BaseResult> GetProductTemplate(GetAllTemplateRequest request)
        {
            try
            {
                var productTemplate = await _context.ProductTemplates
                    .Include(c => c.TemplateSteps).ThenInclude(c => c.IngredientTypeTemplateSteps)
                    .ThenInclude(c => c.IngredientType)
                    .FirstOrDefaultAsync(c => c.Id == request.ProductTemplateId);

                if (productTemplate == null)
                {
                    return new BaseResult(new Error(ErrorCode.NotFound, _translator.GetString(TranslatorMessages.ProductTemplateMessages.Không_tìm_thấy_mẫu_sản_phẩm(request.ProductTemplateId)), nameof(request.ProductTemplateId)));
                }
                var templates = new List<TemplateStepResponse>();
                foreach (var ts in productTemplate.TemplateSteps)
                {
                    var templateStep = new TemplateStepResponse
                    {
                        Id = ts.Id,
                        Name = ts.Name
                    };
                    var its = new List<IngredientTypeDto>();
                    foreach (var it in ts.IngredientTypeTemplateSteps)
                    {
                        var ingreStep = new IngredientTypeDto
                        {
                            Id = it.IngredientTypeId,
                            Name = it.IngredientType.Name,
                            QuantityMin = it.QuantityMin,
                            QuantityMax = it.QuantityMax
                        };
                        its.Add(ingreStep);
                    }
                    templateStep.IngredientTypes = its;
                    templates.Add(templateStep);
                }
                var result = new GetProductTemplateResponse
                {
                    Id = productTemplate.Id,
                    Name = productTemplate.Name,
                    Price = productTemplate.Price,
                    Steps = templates,
                };

                return new BaseResult<GetProductTemplateResponse>(result);
            }
            catch (Exception ex)
            {
                return new BaseResult($"Đã xảy ra lỗi khi lấy tất cả mẫu: {ex.Message}");
            }
        }

        public async Task<BaseResult> UpdateTempalte(CreateTemplateRequest request)
        {
            try
            {
                if (request.TemplateStepId <= 0)
                {
                    return new BaseResult(new Error(ErrorCode.FieldDataInvalid, _translator.GetString(TranslatorMessages.RequestMessage.Trường_id_không_hợp_lệ(request.TemplateStepId)), nameof(request.TemplateStepId)));
                }
                var templateStep = await _context.TemplateSteps.FirstOrDefaultAsync(c => c.Id == request.TemplateStepId);

                if (templateStep == null)
                {
                    return new BaseResult(new Error(ErrorCode.NotFound, _translator.GetString(TranslatorMessages.TemplateStepMessages.Không_tìm_thấy_bước_mẫu(request.TemplateStepId)), nameof(request.TemplateStepId)));
                }
                foreach (var ingreType in request.IngredientType)
                {
                    var ingredientType = await _context.IngredientTypes.FirstOrDefaultAsync(i => i.Id == ingreType.IngredientTypeId);
                    if (ingredientType == null)
                    {
                        return new BaseResult(new Error(ErrorCode.NotFound, _translator.GetString(TranslatorMessages.IngredientTypeMessages.Không_tìm_thấy_loại_nguyên_liệu(ingreType.IngredientTypeId)), nameof(ingreType.IngredientTypeId)));
                    }
                    if (_context.IngredientTypeTemplateSteps.Any(c => c.IngredientTypeId == ingredientType.Id &&
                        c.TemplateStepId != templateStep.Id
                    ))
                    {
                        return new BaseResult(new Error(ErrorCode.FieldDataInvalid, _translator.GetString("Loại nguyên liệu đã tồn tại trong bước khác.")));
                    }
                }

                var existsTemplate = await _context.IngredientTypeTemplateSteps
                    .Where(c=> c.TemplateStepId == request.TemplateStepId).ToListAsync();
                if (existsTemplate.Any())
                {
                    _context.IngredientTypeTemplateSteps.RemoveRange(existsTemplate);
                }

                foreach (var newIngredientType in request.IngredientType)
                {
                    var count = 0;
                    var ingredientType = await _context.IngredientTypes.Include(i => i.Ingredients)
                        .FirstOrDefaultAsync(i => i.Id == newIngredientType.IngredientTypeId);
                    if (!ingredientType.Ingredients.Any())
                    {
                        return new BaseResult(new Error(ErrorCode.NotFound, _translator.GetString(ingredientType.Name + " chưa có nguyên liệu. Hãy thêm nguyên liệu.")));

                    }
                    if (ingredientType.Ingredients.Count() < newIngredientType.QuantityMax)
                    {
                        return new BaseResult(new Error(ErrorCode.FieldDataInvalid, _translator.GetString(ingredientType.Name + " không đủ nguyên liệu. Chọn lại số lượng lớn nhất.")));
                    }
                    foreach (var ingre in ingredientType.Ingredients)
                    {
                        if (ingre.DefaultQuantity > 0)
                        {
                            count++;
                        }
                    }
                    if (count > newIngredientType.QuantityMax)
                    {
                        return new BaseResult(new Error(ErrorCode.FieldDataInvalid,
                            _translator.GetString(ingredientType.Name + " chứa " + count + " nguyên liệu mặc định. Chọn lại số lượng lớn nhất.")));
                    }
                    var ingredientProduct = new IngredientTypeTemplateStep
                    {
                        TemplateStepId = request.TemplateStepId,
                        IngredientTypeId = newIngredientType.IngredientTypeId,
                        QuantityMax = newIngredientType.QuantityMax,
                        QuantityMin = newIngredientType.QuantityMin,
                    };
                    await _context.IngredientTypeTemplateSteps.AddAsync(ingredientProduct);
                }
                templateStep.Update((int)TemplateStepStatus.Active);
                await _unitOfWork.SaveChangesAsync();
                return new BaseResult();
            }
            catch (Exception ex)
            {
                return new BaseResult($"Đã xảy ra lỗi khi cập nhật mẫu: {ex.Message}");
            }
        }

        public async Task<BaseResult> UpdateTemplateStatus(UpdateTemplateStatusRequest request)
        {
            try
            {
                var productTemplate = await _context.ProductTemplates
                                    .Include(c => c.TemplateSteps)
                                    .ThenInclude(c=> c.IngredientTypeTemplateSteps)
                                    .ThenInclude(c=> c.IngredientType)
                                    .ThenInclude(c=> c.Ingredients)
                                    .FirstOrDefaultAsync(c => c.Id == request.ProductTemplateId);

                if (productTemplate == null)
                {
                    return new BaseResult(new Error(ErrorCode.NotFound, _translator.GetString(TranslatorMessages.ProductTemplateMessages.Không_tìm_thấy_mẫu_sản_phẩm(request.ProductTemplateId)), nameof(request.ProductTemplateId)));
                }
                if (productTemplate.TemplateSteps.Count == 0)
                {
                    return new BaseResult(new Error(ErrorCode.NotFound, _translator.GetString("Mẫu chưa có bước!!!")));
                }
                if (productTemplate.TemplateSteps.Any(c => c.Status == (int)TemplateStepStatus.Inactive))
                {
                    return new BaseResult(new Error(ErrorCode.Exception, _translator.GetString(TranslatorMessages.ProductTemplateMessages.Mẫu_sản_phẩm_tồn_tại_bước_không_hoạt_động(request.ProductTemplateId)), nameof(request.ProductTemplateId)));
                }
                decimal price = 0;
                foreach(var step in productTemplate.TemplateSteps)
                {
                    foreach(var ingreStep in step.IngredientTypeTemplateSteps) {
                        foreach(var ingredient in ingreStep.IngredientType.Ingredients)
                        {
                            price += ingredient.Price * ingredient.DefaultQuantity;
                        }
                    }
                }
                productTemplate.Price = price;
                productTemplate.Status = (int)ProductTemplateStatus.Active;
                await _unitOfWork.SaveChangesAsync();
                return new BaseResult();
            }
            catch(Exception ex)
            {
                return new BaseResult($"Đã xảy ra lỗi khi cập nhật trạng thái mẫu: {ex.Message}");
            }
        }
    }
}
