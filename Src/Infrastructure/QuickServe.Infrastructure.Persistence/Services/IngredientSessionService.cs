using Azure.Core;
using Microsoft.EntityFrameworkCore;
using QuickServe.Application.DTOs.Ingredients.Responses;
using QuickServe.Application.DTOs.IngredientSessions;
using QuickServe.Application.DTOs.Sessions.Response;
using QuickServe.Application.Helpers;
using QuickServe.Application.Interfaces;
using QuickServe.Application.Interfaces.IngredientSessions;
using QuickServe.Application.Wrappers;
using QuickServe.Domain.IngredientSessions.Entities;
using QuickServe.Infrastructure.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickServe.Infrastructure.Persistence.Services
{
    public class IngredientSessionService : IIngredientSessionService
    {
        private readonly ApplicationDbContext _context;
        private readonly ITranslator _translator;
        private readonly IUnitOfWork _unitOfWork;
        public IngredientSessionService(ApplicationDbContext context, ITranslator translator, IUnitOfWork unitOfWork)
        {
            _context = context;
            _translator = translator;
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResult> CreateIngredientSessionAsync(CreateIngredientSessionRequest request)
        {
            try
            {
                var session = await _context.Sessions.Include(c=> c.IngredientSessions)
                    .FirstOrDefaultAsync(c => c.Id == request.SessionId);

                if (session == null)
                {
                    return new BaseResult(new Error(ErrorCode.NotFound, _translator.GetString(TranslatorMessages.SessionMessage.Không_tìm_thấy_ca_làm_việc(request.SessionId)), nameof(request.SessionId)));
                }
                if (session.IngredientSessions.Count != 0)
                {
                    return new BaseResult(new Error(ErrorCode.ConstraintViolation, _translator.GetString(TranslatorMessages.SessionMessage.Ca_làm_việc_tồn_tại_các_nguyên_liệu(request.SessionId)), nameof(request.SessionId)));
                }
                foreach (var ingredient in request.Ingredients)
                {
                    if (await _context.Ingredients.AnyAsync(c => c.Id == ingredient.Id) == false)
                    {
                        return new BaseResult(new Error(ErrorCode.NotFound, _translator.GetString(TranslatorMessages.IngredientMessages.Không_tìm_thấy_nguyên_liệu(ingredient.Id)), nameof(ingredient.Id)));
                    }
                    var sessionIngre = new IngredientSession
                    {
                        IngredientId = ingredient.Id,
                        SessionId = request.SessionId,
                        Quantity = ingredient.Quantity,
                        SoldQuantity = 0
                    };
                    await _context.IngredientSessions.AddAsync(sessionIngre);
                }
                await _unitOfWork.SaveChangesAsync();
                return new BaseResult();
            }
            catch (Exception ex)
            {
                return new BaseResult($"Đã xảy ra lỗi khi tạo nguyên liệu trong ca làm việc: {ex.Message}");
            }
        }

        public async Task<BaseResult> DeleteAllIngredientAsync(long sessionId)
        {
            try
            {
                var exists = await _context.IngredientSessions
                    .Where(c => c.SessionId == sessionId).ToListAsync();
                if (exists.Any())
                {
                    _context.IngredientSessions.RemoveRange(exists);
                }
                await _unitOfWork.SaveChangesAsync();
                return new BaseResult();
            }
            catch (Exception ex)
            {
                return new BaseResult($"Đã xảy ra lỗi khi xóa hết nguyên liệu trong ca làm việc: {ex.Message}");
            }
        }

        public async Task<BaseResult> DeleteIngredientSessionAsync(long sessionId, DeleteIngredientInSessionRequest request)
        {
            try
            {
                var exists = await _context.IngredientSessions
                    .Where(c => c.SessionId == sessionId).ToListAsync();
                var ingre = exists.FirstOrDefault(c=> c.IngredientId == request.IngredientId);
                if( ingre == null ) {
                    return new BaseResult(new Error(ErrorCode.NotFound,
                        _translator.GetString("Không tìm thấy nguyên liệu trong ca"), nameof(request.IngredientId)));
                }
                exists.Remove(ingre);
                await _unitOfWork.SaveChangesAsync();
                return new BaseResult();
            }
            catch (Exception ex)
            {
                return new BaseResult($"Đã xảy ra lỗi khi xóa  nguyên liệu trong ca làm việc: {ex.Message}");
            }
        }

        public async Task<BaseResult> GetBySessionIdAsync(long sessionId)
        {
            try
            {
                var session = await _context.Sessions.Include(c => c.IngredientSessions)
                    .ThenInclude(c => c.Ingredient)
                    .FirstOrDefaultAsync(c => c.Id == sessionId);
                if (session == null)
                {
                    return new BaseResult(new Error(ErrorCode.NotFound, _translator.GetString(TranslatorMessages.SessionMessage.Không_tìm_thấy_ca_làm_việc(sessionId)), nameof(sessionId)));
                }
                if (session.IngredientSessions.Count == 0)
                {
                    return new BaseResult(new Error(ErrorCode.EmptyData, _translator.GetString(TranslatorMessages.SessionMessage.Ca_làm_việc_không_tồn_tại_các_nguyên_liệu(sessionId)), nameof(sessionId)));
                }
                var ingredients = new List<IngredientInSessionResponse>();
                foreach (var ingreNu in session.IngredientSessions)
                {
                    var ingredient  = new IngredientInSessionResponse
                    {
                        Id = ingreNu.IngredientId,
                        Name = ingreNu.Ingredient.Name,
                        ImageUrl = ingreNu.Ingredient.ImageUrl,
                        Quantity = ingreNu.Quantity,
                        SoldQuantity = ingreNu.SoldQuantity
                    };
                    ingredients.Add(ingredient);
                }
                var result = new SessionResponse
                {
                    Id = sessionId,
                    Name = session.Name,
                    StartTime = session.StartTime,
                    EndTime = session.EndTime,

                    Ingredients = ingredients
                };
                return new BaseResult<SessionResponse>(result);
            }
            catch (Exception ex)
            {
                return new BaseResult($"Đã xảy ra lỗi khi lấy nguyên liệu trong ca làm việc: {ex.Message}");
            }
        }

        public async Task<BaseResult> UpdateIngredientSessionAsync(CreateIngredientSessionRequest request)
        {
            try
            {
                var session = await _context.Sessions.Include(c => c.IngredientSessions).FirstOrDefaultAsync(c => c.Id == request.SessionId);

                if (session == null)
                {
                    return new BaseResult(new Error(ErrorCode.NotFound, _translator.GetString(TranslatorMessages.SessionMessage.Không_tìm_thấy_ca_làm_việc(request.SessionId)), nameof(request.SessionId)));
                }
                var exists = await _context.IngredientSessions
                    .Where(c => c.SessionId == request.SessionId).ToListAsync();
                if (exists.Any())
                {
                    _context.IngredientSessions.RemoveRange(exists);
                }
                foreach (var ingredient in request.Ingredients)
                {
                    if (await _context.Ingredients.AnyAsync(c => c.Id == ingredient.Id) == false)
                    {
                        return new BaseResult(new Error(ErrorCode.NotFound, _translator.GetString(TranslatorMessages.IngredientMessages.Không_tìm_thấy_nguyên_liệu(ingredient.Id)), nameof(ingredient.Id)));
                    }
                    var ingredientSession = new IngredientSession
                    {
                        IngredientId = ingredient.Id,
                        SessionId = request.SessionId,
                        Quantity = ingredient.Quantity,
                        SoldQuantity = 0
                    };
                    await _context.IngredientSessions.AddAsync(ingredientSession);
                }
                await _unitOfWork.SaveChangesAsync();
                return new BaseResult();
            }
            catch (Exception ex)
            {
                return new BaseResult($"Đã xảy ra lỗi khi cập nhật nguyên liệu trong ca làm việc: {ex.Message}");
            }
        }
    }
}
