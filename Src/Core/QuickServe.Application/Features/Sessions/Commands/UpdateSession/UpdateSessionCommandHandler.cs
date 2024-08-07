using MediatR;
using QuickServe.Application.Features.ProductTemplates.Commands.UpdateProductTemplate;
using QuickServe.Application.Helpers;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Interfaces;
using QuickServe.Application.Wrappers;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace QuickServe.Application.Features.Sessions.Commands.UpdateSession;

public class UpdateSessionCommandHandler(ISessionRepository sessionRepository, IUnitOfWork unitOfWork, ITranslator translator) : IRequestHandler<UpdateSessionCommand, BaseResult>
{
    public async Task<BaseResult> Handle(UpdateSessionCommand request, CancellationToken cancellationToken)
    {
        if (request.Id <= 0)
        {
            return new BaseResult(new Error(ErrorCode.FieldDataInvalid, translator.GetString(TranslatorMessages.RequestMessage.Trường_id_không_hợp_lệ(request.Id)), nameof(request.Id)));
        }
        var session = await sessionRepository.GetByIdAsync(request.Id);

        if (session is null)
        {
            return new BaseResult(new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.SessionMessage.Không_tìm_thấy_ca_làm_việc(request.Id)), nameof(request.Id)));
        }
        TimeSpan startTime = TimeSpan.Parse(request.StartTime);
        TimeSpan endTime = TimeSpan.Parse(request.EndTime);
        if (startTime >= endTime)
        {
            return new BaseResult(new Error(ErrorCode.FieldDataInvalid, translator.GetString(TranslatorMessages.SessionMessage.Thời_gian_bắt_đầu_phải_trước_thời_gian_kết_thúc(request.Id)), nameof(request.Id)));
        }

        if (await sessionRepository.ExistsByNameAsync(session.StoreId, request.Name.Trim()) &&
            session.Name.ToLower() != request.Name.ToLower().Trim())
        {
            return new BaseResult(new Error(ErrorCode.Duplicate, translator.GetString(TranslatorMessages.SessionMessage.Tên_ca_làm_việc_đã_tồn_tại(request.Name)), nameof(request.Name)));
        }
        if (startTime != session.StartTime && endTime != session.EndTime)
        {
            if (await sessionRepository.ExistsByTimeAsync(session.StoreId, startTime, endTime))
            {
                return new BaseResult(new Error(ErrorCode.Duplicate, translator.GetString(TranslatorMessages.SessionMessage.Thời_gian_làm_việc_đã_có_trong_ca_khác(request.Id)), nameof(request.Id)));
            }
        }
        session.Update(request.Name.Trim(), startTime, endTime);
        await unitOfWork.SaveChangesAsync();
        return new BaseResult();
    }
}