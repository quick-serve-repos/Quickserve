using MediatR;
using QuickServe.Application.Wrappers;

namespace QuickServe.Application.Features.Sessions.Commands.DeleteSession;

public class DeleteSessionCommand : IRequest<BaseResult>
{
    public long Id { get; set; }
}