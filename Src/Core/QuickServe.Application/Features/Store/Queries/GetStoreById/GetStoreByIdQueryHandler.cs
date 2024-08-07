using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using QuickServe.Application.DTOs;
using QuickServe.Application.Helpers;
using QuickServe.Application.Interfaces;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Wrappers;
using QuickServe.Domain.Stores.Dtos;

namespace QuickServe.Application.Features.Store.Queries.GetStoreById;

public class GetStoreByIdQueryHandler(IStoreRepository storeRepository, ITranslator translator) : IRequestHandler<GetStoreByIdQuery, BaseResult<StoreDto>>
{
    public async Task<BaseResult<StoreDto>> Handle(GetStoreByIdQuery request, CancellationToken cancellationToken)
    {
        var store = await storeRepository.GetByIdAsync(request.Id);
        if (store is null)
        {
            return new BaseResult<StoreDto>(new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.StoreMessages.Không_tìm_thấy_cửa_hàng(request.Id)), nameof(request.Id)));

        }

        var result = new StoreDto(store);
        return new BaseResult<StoreDto>(result);
    }
}