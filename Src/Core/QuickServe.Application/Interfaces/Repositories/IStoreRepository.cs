using System.Threading.Tasks;
using QuickServe.Application.DTOs;
using QuickServe.Domain.Stores.Dtos;
using QuickServe.Domain.Stores.Entities;

namespace QuickServe.Application.Interfaces.Repositories;

public interface IStoreRepository : IGenericRepository<Store>
{
    Task<PagenationResponseDto<StoreDto>> GetPagedListAsync(int pageNumber, int pageSize, string name);
    Task<Store> FindByIdAsync(long id);
}