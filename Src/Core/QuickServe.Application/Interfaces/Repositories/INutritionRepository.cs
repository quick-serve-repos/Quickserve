using QuickServe.Application.DTOs;
using QuickServe.Domain.Nutritions.Dtos;
using QuickServe.Domain.Nutritions.Entities;
using System.Threading.Tasks;

namespace QuickServe.Application.Interfaces.Repositories;

public interface INutritionRepository : IGenericRepository<Nutrition>
{
    Task<Nutrition> FindByIdAsync(long id);
    Task<PagenationResponseDto<NutritionDTO>> GetPagedListAsync(int pageNumber, int pageSize, string name);
    Task<bool> ExistsByNameAsync(string name);
}