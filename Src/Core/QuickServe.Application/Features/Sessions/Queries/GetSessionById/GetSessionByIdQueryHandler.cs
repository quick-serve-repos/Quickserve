using MediatR;
using QuickServe.Application.Helpers;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Interfaces;
using QuickServe.Application.Utils;
using QuickServe.Application.Wrappers;
using System.Threading.Tasks;
using System.Threading;
using System;
using QuickServe.Domain.Sessions.Dtos;

namespace QuickServe.Application.Features.Sessions.Queries.GetSessionById;

public class GetSessionByIdQueryHandler(ISessionRepository sessionRepository, ITranslator translator) : IRequestHandler<GetSessionByIdQuery, BaseResult<SessionDto>>
{

    public async Task<BaseResult<SessionDto>> Handle(GetSessionByIdQuery request, CancellationToken cancellationToken)
    {
        var session = await sessionRepository.GetByIdAsync(request.Id);
        if (session is null)
        {
            return new BaseResult<SessionDto>(new Error(ErrorCode.NotFound,
                translator.GetString(TranslatorMessages.SessionMessage.Không_tìm_thấy_ca_làm_việc(request.Id)),
                nameof(request.Id)));
        }

        var result = new SessionDto(session);
        result.Created = TimeZoneConverter.ConvertToUserTimeZone(session.Created);
        result.LastModified = session.LastModified.HasValue
                ? TimeZoneConverter.ConvertToUserTimeZone(session.LastModified.Value)
                : (DateTime?)null;
        return new BaseResult<SessionDto>(result);
    }
}