using MediatR;
using QuickServe.Application.Wrappers;
using System;

namespace QuickServe.Application.Features.Sessions.Commands.CreateSession;

public class CreateSessionCommand : IRequest<BaseResult>
{
    public string Name { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get; set; }

}