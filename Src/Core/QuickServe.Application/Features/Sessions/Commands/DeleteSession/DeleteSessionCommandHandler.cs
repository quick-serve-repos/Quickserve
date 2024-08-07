using MediatR;
using QuickServe.Application.Helpers;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Interfaces;
using QuickServe.Application.Wrappers;
using System.Threading.Tasks;
using System.Threading;

namespace QuickServe.Application.Features.Sessions.Commands.DeleteSession;

public class DeleteSessionCommandHandler(ISessionRepository sessionRepository, IUnitOfWork unitOfWork, ITranslator translator) : IRequestHandler<DeleteSessionCommand, BaseResult>
{
    public async Task<BaseResult> Handle(DeleteSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await sessionRepository.FindByIdAsync(request.Id);

        if (session is null)
        {
            return new BaseResult(new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.SessionMessage.Không_tìm_thấy_ca_làm_việc(request.Id)), nameof(request.Id)));
        }
        if (session.IngredientSessions.Count != 0)
        {
            return new BaseResult(new Error(ErrorCode.ConstraintViolation, translator.GetString(TranslatorMessages.SessionMessage.Ca_làm_việc_tồn_tại_các_nguyên_liệu(request.Id)), nameof(request.Id)));
        }
        sessionRepository.Delete(session);
        await unitOfWork.SaveChangesAsync();
        return new BaseResult();
    }
}