using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickServe.Application.Features.Store.Commands.AddEmployee;
using QuickServe.Application.Features.Store.Commands.CreateStore;
using QuickServe.Application.Features.Store.Commands.DeleteStore;
using QuickServe.Application.Features.Store.Commands.UpdateStore;
using QuickServe.Application.Features.Store.Queries.GetPagedListStore;
using QuickServe.Application.Features.Store.Queries.GetStoreById;
using QuickServe.Application.Features.Store.Queries.GetStoreEmployees;
using QuickServe.Application.Wrappers;
using QuickServe.Domain.Stores.Dtos;

namespace QuickServe.WebApi.Controllers.v1
{
    public class StoresController : BaseApiController
    {
        [HttpGet("paged")]
        public async Task<PagedResponse<StoreDto>> GetPagedListStore([FromQuery] GetPagedListStoreQuery model)
            => await Mediator.Send(model);

        [HttpGet("{id}")]
        public async Task<BaseResult<StoreDto>> GetStoreById(long id)
            => await Mediator.Send(new GetStoreByIdQuery { Id = id });

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<BaseResult<long>> CreateStore(CreateStoreCommand model)
            => await Mediator.Send(model);

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<BaseResult> UpdateStore(long id, UpdateStoreCommand model)
        {
            model.Id = id;
            return await Mediator.Send(model);
        }
          

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<BaseResult> DeleteStore(long id)
            => await Mediator.Send(new DeleteStoreCommand { Id = id });

        [Authorize(Roles = "Admin, Store_Manager")]
        [HttpGet("employees/paged")]
        public async Task<PagedResponse<EmployeeDto>> GetEmployees([FromQuery] GetPagedListStoreEmployeesQuery model)
            => await Mediator.Send(new GetPagedListStoreEmployeesQuery { PageNumber = model.PageNumber, PageSize = model.PageSize });

        [Authorize(Roles = "Admin, Store_Manager")]
        [HttpPost("employees")]
        public async Task<BaseResult<Guid>> AddEmployee([FromBody] AddEmployeeCommand model)
        {
            return await Mediator.Send(model);
        }
           
    }
}
