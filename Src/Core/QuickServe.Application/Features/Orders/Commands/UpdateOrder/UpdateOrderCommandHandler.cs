using System;
using System.Linq;
using MediatR;
using QuickServe.Application.DTOs.Orders.Response;
using QuickServe.Application.Helpers;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Interfaces;
using QuickServe.Application.Wrappers;
using System.Threading;
using System.Threading.Tasks;
using static QuickServe.Application.Helpers.TranslatorMessages;

namespace QuickServe.Application.Features.Orders.Commands.UpdateOrder;

public class UpdateOrderCommandHandler(
    ITranslator translator,
    IUnitOfWork unitOfWork,
    IOrderRepository orderRepository,
    IIngredientSessionRepository ingredientSessionRepository,
    ISessionRepository sessionRepository) : IRequestHandler<UpdateOrderCommand, BaseResult<OrderResponse>>
{
    /* public async Task<BaseResult<OrderResponse>> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
     {
         if (request.OrderId <= 0)
         {
             return new BaseResult<OrderResponse>(new Error(ErrorCode.FieldDataInvalid, "OrderId not valid"));
         }
         var order = await orderRepository.GetByIdAsync(request.OrderId);

         if (order is null)
         {
             return new BaseResult<OrderResponse>(new Error(ErrorCode.NotFound, "Order not found"));
         }

         if(order.Status != request.Status)
         {
             order.Status = request.Status;
             orderRepository.Update(order);
             await unitOfWork.SaveChangesAsync();
         }

         var result = new OrderResponse()
         {
             OrderId = order.Id.ToString(),
             Status = order.Status
         };

         return new BaseResult<OrderResponse>(result);
     }
 */
    //update ok ngon , nhung khong co check rang buoc cac trang thia
    /* public async Task<BaseResult<OrderResponse>> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
     {
         if (request.OrderId <= 0)
         {
             return new BaseResult<OrderResponse>(new Error(ErrorCode.FieldDataInvalid, "OrderId not valid"));
         }

         var order = await orderRepository.GetByIdAsync(request.OrderId);

         if (order == null)
         {
             return new BaseResult<OrderResponse>(new Error(ErrorCode.NotFound, "Order not found"));
         }

         // Kiểm tra trạng thái có thể được cập nhật hay không (chỉ cho phép tăng trạng thái theo tuần tự)
         if (request.Status <= order.Status)
         {
             return new BaseResult<OrderResponse>(new Error(ErrorCode.FieldDataInvalid, "Cannot move to a lower or same status"));
         }


         // Kiểm tra nếu trạng thái mới không phải là trạng thái tiếp theo
         if (request.Status - order.Status > 1)
         {
             return new BaseResult<OrderResponse>(new Error(ErrorCode.FieldDataInvalid, "You must follow the status sequence step by step"));
         }

         // Chỉ cập nhật nếu trạng thái thay đổi
         if (order.Status != request.Status)
         {
             // Nếu trạng thái chuyển từ 2 (Paid) sang 6 (Canceled), giảm soldQuantity trong ingredientSession
             if (order.Status == 2 && request.Status == 6)
             {
                 // Lấy giờ hiện tại theo UTC+7
                 var utcNow = DateTime.UtcNow.AddHours(7);
                 var currentTimeOfDay = utcNow.TimeOfDay;

                 // Lấy phiên hiện tại dựa trên thời gian UTC+7
                 var sessions = await sessionRepository.GetAllAsync();
                 var currentSession = sessions.FirstOrDefault(x =>
                     x.StartTime <= currentTimeOfDay && x.EndTime >= currentTimeOfDay);

                 if (currentSession == null)
                 {
                     return new BaseResult<OrderResponse>(new Error(ErrorCode.NotFound, "Current session not found"));
                 }

                 foreach (var orderProduct in order.OrderProducts)
                 {
                     foreach (var ingredientProduct in orderProduct.Product.IngredientProducts)
                     {
                         // Sử dụng ingredientId và sessionId để tìm ingredientSession
                         var ingredientSession =
                             await ingredientSessionRepository.GetByIdAsync(ingredientProduct.IngredientId,
                                 currentSession.Id);
                         if (ingredientSession != null)
                         {
                             // Giảm soldQuantity khi đơn hàng bị hủy
                             ingredientSession.SoldQuantity -= ingredientProduct.Quantity;
                             ingredientSessionRepository
                                 .Update(ingredientSession); // Cập nhật lại soldQuantity trong IngredientSession
                         }
                     }
                 }
             }

             // Nếu trạng thái là 2 (Paid), tăng soldQuantity trong ingredientSession
             else if (request.Status == 2)
             {
                 // Lấy giờ hiện tại theo UTC+7
                 var utcNow = DateTime.UtcNow.AddHours(7);
                 var currentTimeOfDay = utcNow.TimeOfDay;

                 // Lấy phiên hiện tại dựa trên thời gian UTC+7
                 var sessions = await sessionRepository.GetAllAsync();
                 var currentSession = sessions.FirstOrDefault(x =>
                     x.StartTime <= currentTimeOfDay && x.EndTime >= currentTimeOfDay);

                 if (currentSession == null)
                 {
                     return new BaseResult<OrderResponse>(new Error(ErrorCode.NotFound, "Current session not found"));
                 }

                 foreach (var orderProduct in order.OrderProducts)
                 {
                     foreach (var ingredientProduct in orderProduct.Product.IngredientProducts)
                     {
                         // Sử dụng ingredientId và sessionId để tìm ingredientSession
                         var ingredientSession =
                             await ingredientSessionRepository.GetByIdAsync(ingredientProduct.IngredientId,
                                 currentSession.Id);
                         if (ingredientSession != null)
                         {
                             // Kiểm tra số lượng tồn kho còn lại trước khi tăng soldQuantity
                             if (ingredientProduct.Quantity <=
                                 (ingredientSession.Quantity - ingredientSession.SoldQuantity))
                             {
                                 ingredientSession.SoldQuantity += ingredientProduct.Quantity;
                                 ingredientSessionRepository
                                     .Update(ingredientSession); // Cập nhật lại soldQuantity trong IngredientSession
                             }
                             else
                             {
                                 return new BaseResult<OrderResponse>(new Error(ErrorCode.FieldDataInvalid,
                                     "Not enough ingredient stock available"));
                             }
                         }
                     }
                 }
             }

             // Cập nhật trạng thái đơn hàng
             order.Status = request.Status;
             orderRepository.Update(order);
             await unitOfWork.SaveChangesAsync();
         }

         var result = new OrderResponse()
         {
             OrderId = order.Id.ToString(),
             BillCode = order.BillCode,
             Status = order.Status
         };

         return new BaseResult<OrderResponse>(result);
     }*/
    public async Task<BaseResult<OrderResponse>> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
{
    if (request.OrderId <= 0)
    {
        return new BaseResult<OrderResponse>(new Error(ErrorCode.FieldDataInvalid, "OrderId not valid"));
    }

    var order = await orderRepository.GetByIdAsync(request.OrderId);

    if (order == null)
    {
        return new BaseResult<OrderResponse>(new Error(ErrorCode.NotFound, "Order not found"));
    }

    // Không cho phép cập nhật trạng thái nếu trạng thái mới <= trạng thái hiện tại
    if (request.Status <= order.Status)
    {
        return new BaseResult<OrderResponse>(new Error(ErrorCode.FieldDataInvalid, "Cannot move to a lower or same status"));
    }

    // Kiểm tra nếu trạng thái mới không phải là trạng thái tiếp theo
    if (request.Status - order.Status > 1 && !(order.Status == 1 || order.Status == 2) && request.Status == 6)
    {
        return new BaseResult<OrderResponse>(new Error(ErrorCode.FieldDataInvalid, "You must follow the status sequence step by step"));
    }

    // Nếu trạng thái chuyển từ 2 (Paid) sang 6 (Canceled), giảm soldQuantity trong ingredientSession
    if ( (order.Status == 1 ||order.Status == 2) && request.Status == 6)
    {
        // Lấy giờ hiện tại theo UTC+7
        var utcNow = DateTime.UtcNow.AddHours(7);
        var currentTimeOfDay = utcNow.TimeOfDay;

        // Lấy phiên hiện tại dựa trên thời gian UTC+7
        var sessions = await sessionRepository.GetAllAsync();
        var currentSession = sessions.FirstOrDefault(x =>
            x.StartTime <= currentTimeOfDay && x.EndTime >= currentTimeOfDay);

        if (currentSession == null)
        {
            return new BaseResult<OrderResponse>(new Error(ErrorCode.NotFound, "Current session not found"));
        }

        foreach (var orderProduct in order.OrderProducts)
        {
            foreach (var ingredientProduct in orderProduct.Product.IngredientProducts)
            {
                // Sử dụng ingredientId và sessionId để tìm ingredientSession
                var ingredientSession =
                    await ingredientSessionRepository.GetByIdAsync(ingredientProduct.IngredientId,
                        currentSession.Id);
                if (ingredientSession != null)
                {
                    // Giảm soldQuantity khi đơn hàng bị hủy
                    ingredientSession.SoldQuantity -= ingredientProduct.Quantity;
                    ingredientSessionRepository
                        .Update(ingredientSession); // Cập nhật lại soldQuantity trong IngredientSession
                }
            }
        }
    }

    // Nếu trạng thái là 2 (Paid), tăng soldQuantity trong ingredientSession
    else if (request.Status == 2)
    {
        // Lấy giờ hiện tại theo UTC+7
        var utcNow = DateTime.UtcNow.AddHours(7);
        var currentTimeOfDay = utcNow.TimeOfDay;

        // Lấy phiên hiện tại dựa trên thời gian UTC+7
        var sessions = await sessionRepository.GetAllAsync();
        var currentSession = sessions.FirstOrDefault(x =>
            x.StartTime <= currentTimeOfDay && x.EndTime >= currentTimeOfDay);

        if (currentSession == null)
        {
            return new BaseResult<OrderResponse>(new Error(ErrorCode.NotFound, "Current session not found"));
        }

        foreach (var orderProduct in order.OrderProducts)
        {
            foreach (var ingredientProduct in orderProduct.Product.IngredientProducts)
            {
                // Sử dụng ingredientId và sessionId để tìm ingredientSession
                var ingredientSession =
                    await ingredientSessionRepository.GetByIdAsync(ingredientProduct.IngredientId,
                        currentSession.Id);
                if (ingredientSession != null)
                {
                    // Kiểm tra số lượng tồn kho còn lại trước khi tăng soldQuantity
                    if (ingredientProduct.Quantity <=
                        (ingredientSession.Quantity - ingredientSession.SoldQuantity))
                    {
                        ingredientSession.SoldQuantity += ingredientProduct.Quantity;
                        ingredientSessionRepository
                            .Update(ingredientSession); // Cập nhật lại soldQuantity trong IngredientSession
                    }
                    else
                    {
                        return new BaseResult<OrderResponse>(new Error(ErrorCode.FieldDataInvalid,
                            "Not enough ingredient stock available"));
                    }
                }
            }
        }
    }

    // Cập nhật trạng thái đơn hàng
    order.Status = request.Status;
    orderRepository.Update(order);
    await unitOfWork.SaveChangesAsync();

    // Trả về kết quả cuối cùng
    var orderResponse = new OrderResponse()
    {
        OrderId = order.Id.ToString(),
        BillCode = order.BillCode,
        Status = order.Status
    };

    return new BaseResult<OrderResponse>(orderResponse);
}

}