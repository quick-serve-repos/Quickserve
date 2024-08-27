using MediatR;
using QuickServe.Application.Parameters;
using QuickServe.Application.Wrappers;
using QuickServe.Domain.Stores.Dtos;

namespace QuickServe.Application.Features.Store.Queries.GetStoreEmployees
{
    public class GetPagedListStoreEmployeesQuery : PagenationRequestParameter, IRequest<PagedResponse<EmployeeDto>>
    {
        public string Name { get; set; }
        public string[] Roles { get; set; } = [];

    }
}