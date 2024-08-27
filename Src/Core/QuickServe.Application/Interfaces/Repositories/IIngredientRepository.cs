using QuickServe.Application.DTOs;
using QuickServe.Domain.Categories.Dtos;
using QuickServe.Domain.Categories.Entities;
using QuickServe.Domain.Ingredients.Dtos;
using QuickServe.Domain.Ingredients.Entities;
using System.Threading.Tasks;

namespace QuickServe.Application.Interfaces.Repositories;

public interface IIngredientRepository : IGenericRepository<Ingredient>
{
    Task<Ingredient> GetIngredientByIdAsync(long id);
    Task<PagenationResponseDto<IngredientDTO>> GetPagedListAsync(int pageNumber, int pageSize, string name);
    Task<PagenationResponseDto<IngredientDTO>> GetPagedListByAcitveStatusAsync(int pageNumber, int pageSize, string name);
    Task<bool> ExistByNameAsync(string name, long id);
}