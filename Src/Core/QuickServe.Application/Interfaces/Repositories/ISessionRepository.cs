using QuickServe.Application.DTOs;
using QuickServe.Domain.Sessions.Dtos;
using QuickServe.Domain.Sessions.Entities;
using System;
using System.Threading.Tasks;

namespace QuickServe.Application.Interfaces.Repositories;

public interface ISessionRepository : IGenericRepository<Session>
{
    Task<Session> FindByIdAsync(long id);
    Task<PagenationResponseDto<SessionDto>> GetPagedListAsyncByStore(long storeId, int pageNumber, int pageSize, string name);
    Task<bool> ExistsByNameAsync(long storeId, string name);
    Task<bool> ExistsByTimeAsync(long storeId, TimeSpan startTime, TimeSpan endTime);
}