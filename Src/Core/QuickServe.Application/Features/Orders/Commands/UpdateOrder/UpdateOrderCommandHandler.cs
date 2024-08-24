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

public class UpdateOrderCommandHandler (ITranslator translator, IUnitOfWork unitOfWork, IOrderRepository orderRepository, IIngredientSessionRepository ingredientSessionRepository, ISessionRepository sessionRepository) : IRequestHandler<UpdateOrderCommand, BaseResult<OrderResponse>>
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

        // Chỉ cập nhật nếu trạng thái thay đổi
        if (order.Status != request.Status)
        {
            // Nếu trạng thái là 4 (Thành công), tăng soldQuantity trong ingredientSession
            if (request.Status == 2)
            {
                // Lấy phiên hiện tại
                var sessions = await sessionRepository.GetAllAsync();
                var currentSession = sessions.FirstOrDefault(x =>
                    x.StartTime <= DateTime.Now.TimeOfDay && x.EndTime >= DateTime.Now.TimeOfDay);

                if (currentSession == null)
                {
                    return new BaseResult<OrderResponse>(new Error(ErrorCode.NotFound, "Current session not found"));
                }

                foreach (var orderProduct in order.OrderProducts)
                {
                    foreach (var ingredientProduct in orderProduct.Product.IngredientProducts)
                    {
                        // Sử dụng ingredientId và sessionId để tìm ingredientSession
                        var ingredientSession = await ingredientSessionRepository.GetByIdAsync(ingredientProduct.IngredientId, currentSession.Id);
                        if (ingredientSession != null)
                        {
                            // Kiểm tra số lượng tồn kho còn lại trước khi tăng soldQuantity
                            if (ingredientProduct.Quantity <= (ingredientSession.Quantity - ingredientSession.SoldQuantity))
                            {
                                ingredientSession.SoldQuantity += ingredientProduct.Quantity;
                                ingredientSessionRepository.Update(ingredientSession); // Cập nhật lại soldQuantity trong IngredientSession
                            }
                            else
                            {
                                return new BaseResult<OrderResponse>(new Error(ErrorCode.FieldDataInvalid, "Not enough ingredient stock available"));
                            }
                        }
                    }
                }
            }

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
    }

}