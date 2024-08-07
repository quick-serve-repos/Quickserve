using MediatR;
using QuickServe.Application.Parameters;
using QuickServe.Application.Wrappers;
using QuickServe.Domain.Sessions.Dtos;

namespace QuickServe.Application.Features.Sessions.Queries.GetPagedListSession;

public class GetPagedListSessionQuery : PagenationRequestParameter, IRequest<PagedResponse<SessionDto>>
{
    public string Name { get; set; }
}