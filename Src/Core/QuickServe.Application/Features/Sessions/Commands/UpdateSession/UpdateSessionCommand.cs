using MediatR;
using QuickServe.Application.Wrappers;
using System;

namespace QuickServe.Application.Features.Sessions.Commands.UpdateSession;

public class UpdateSessionCommand : IRequest<BaseResult>
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get; set; }
}