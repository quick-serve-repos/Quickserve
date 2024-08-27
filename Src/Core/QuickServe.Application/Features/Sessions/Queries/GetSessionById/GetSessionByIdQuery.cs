using MediatR;
using QuickServe.Application.Wrappers;
using QuickServe.Domain.Sessions.Dtos;

namespace QuickServe.Application.Features.Sessions.Queries.GetSessionById;

public class GetSessionByIdQuery : IRequest<BaseResult<SessionDto>>
{
    public long Id { get; set; }
}