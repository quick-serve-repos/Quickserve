using QuickServe.Application.DTOs.Nutritions.Request;
using QuickServe.Application.DTOs.ProductTemplates;
using QuickServe.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickServe.Application.Interfaces.Nutritions
{
    public interface INutritionService
    {
        Task<BaseResult> CreateNutritionAsync(CreateNutritionRequest request);
        Task<BaseResult> UpdateNutritionImageAsync(long id, UpdateNutritionImageRequest request);
    }
}
