using System.Threading;
using System.Threading.Tasks;
using MediatR;
using QuickServe.Application.Helpers;
using QuickServe.Application.Interfaces;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Wrappers;

namespace QuickServe.Application.Features.Store.Commands.UpdateStore;

public class UpdateStoreCommandHandler(IStoreRepository storeRepository, IUnitOfWork unitOfWork, ITranslator translator) : IRequestHandler<UpdateStoreCommand, BaseResult>
{
    public async Task<BaseResult> Handle(UpdateStoreCommand request, CancellationToken cancellationToken)
    {
        if (request.Id <= 0)
        {
            return new BaseResult(new Error(ErrorCode.FieldDataInvalid, translator.GetString(TranslatorMessages.RequestMessage.Trường_id_không_hợp_lệ(request.Id)), nameof(request.Id)));
        }
        var store = await storeRepository.GetByIdAsync(request.Id);
        if (store is null)
        {
            return new BaseResult(new Error(ErrorCode.NotFound,
                translator.GetString(TranslatorMessages.StoreMessages.Không_tìm_thấy_cửa_hàng(request.Id)),
                nameof(request.Id)));
        }

        store.Update(request.Name, request.Address);
        await unitOfWork.SaveChangesAsync();
        return new BaseResult();
    }
}