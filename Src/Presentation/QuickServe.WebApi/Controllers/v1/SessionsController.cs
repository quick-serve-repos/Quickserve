using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickServe.Application.Features.Sessions.Commands.CreateSession;
using QuickServe.Application.Features.Sessions.Commands.DeleteSession;
using QuickServe.Application.Features.Sessions.Commands.UpdateSession;
using QuickServe.Application.Features.Sessions.Queries.GetPagedListSession;
using QuickServe.Application.Features.Sessions.Queries.GetSessionById;
using QuickServe.Application.Wrappers;
using QuickServe.Domain.Sessions.Dtos;
using System.Threading.Tasks;

namespace QuickServe.WebApi.Controllers.v1
{
    public class SessionsController : BaseApiController
    {
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Store_Manager, Employee")]
        [HttpGet("paged")]
        public async Task<PagedResponse<SessionDto>> GetPagedListSessionByStore([FromQuery] GetPagedListSessionQuery model)
            => await Mediator.Send(model);

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Store_Manager, Employee")]
        [HttpGet("{id}")]
        public async Task<BaseResult<SessionDto>> GetSessionById(long id)
            => await Mediator.Send(new GetSessionByIdQuery { Id = id });

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Store_Manager")]
        public async Task<BaseResult> CreateSession(CreateSessionCommand model)
            => await Mediator.Send(model);


        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Store_Manager")]
        public async Task<BaseResult> UpdateSession(long id, UpdateSessionCommand model)
        {
            model.Id = id;
            return await Mediator.Send(model);
        }


        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Store_Manager")]
        public async Task<BaseResult> DeleteSession(long id)
            => await Mediator.Send(new DeleteSessionCommand { Id = id });
    }
}
