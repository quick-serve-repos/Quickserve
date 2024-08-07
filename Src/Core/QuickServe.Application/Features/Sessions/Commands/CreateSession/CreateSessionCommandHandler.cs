using MediatR;
using QuickServe.Application.Helpers;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Interfaces;
using QuickServe.Application.Utils.Enums;
using QuickServe.Application.Wrappers;
using System.Threading.Tasks;
using System.Threading;
using QuickServe.Domain.Sessions.Entities;
using System;

namespace QuickServe.Application.Features.Sessions.Commands.CreateSession;

public class CreateSessionCommandHandler(IAccountRepository accountRepository, IAuthenticatedUserService authenticatedUserService, ISessionRepository sessionRepository, IUnitOfWork unitOfWork, ITranslator translator) : IRequestHandler<CreateSessionCommand, BaseResult>
{
    public async Task<BaseResult> Handle(CreateSessionCommand request, CancellationToken cancellationToken)
    {
        var currentUser = await accountRepository.FindByIdAsync(Guid.Parse(authenticatedUserService.UserId));
        if (currentUser == null)
        {
            return new BaseResult<Guid>(new Error(ErrorCode.NotFound, translator.GetString("Không tim thấy tài khoản"), nameof(authenticatedUserService.UserId)));
        }

        TimeSpan startTime = TimeSpan.Parse(request.StartTime);
        TimeSpan endTime = TimeSpan.Parse(request.EndTime);
        if (startTime >= endTime)
        {
            return new BaseResult(new Error(ErrorCode.FieldDataInvalid, translator.GetString(TranslatorMessages.SessionMessage.Thời_gian_bắt_đầu_phải_trước_thời_gian_kết_thúc(currentUser.Staff.StoreId)), nameof(currentUser.Staff.StoreId)));
        }
        
        if (await sessionRepository.ExistsByTimeAsync(currentUser.Staff.StoreId, startTime, endTime))
        {
            return new BaseResult(new Error(ErrorCode.Duplicate, translator.GetString(TranslatorMessages.SessionMessage.Thời_gian_làm_việc_đã_có_trong_ca_khác(currentUser.Staff.StoreId)), nameof(currentUser.Staff.StoreId)));
        }
        if (await sessionRepository.ExistsByNameAsync(currentUser.Staff.StoreId, request.Name.Trim()))
        {
            return new BaseResult(new Error(ErrorCode.Duplicate, translator.GetString(TranslatorMessages.SessionMessage.Tên_ca_làm_việc_đã_tồn_tại(request.Name)), nameof(request.Name)));
        }
        var session = new Session
        {
            Name = request.Name,
            StartTime = startTime,
            EndTime = endTime,
            StoreId = currentUser.Staff.StoreId,
            Status = (int) SessionStatus.Active
        };
        await sessionRepository.AddAsync(session);
        await unitOfWork.SaveChangesAsync();
        return new BaseResult<long>(session.Id);
    }
}