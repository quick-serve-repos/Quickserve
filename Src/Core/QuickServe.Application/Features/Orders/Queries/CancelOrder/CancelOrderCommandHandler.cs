using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using QuickServe.Application.DTOs.Orders.Response;
using QuickServe.Application.Interfaces;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Wrappers;

namespace QuickServe.Application.Features.Orders.Queries.CancelOrder;

public class CancelOrderCommandHandler(
    ITranslator translator,
    IUnitOfWork unitOfWork,
    IOrderRepository orderRepository,
    IIngredientSessionRepository ingredientSessionRepository,
    ISessionRepository sessionRepository) : IRequestHandler<CancelOrderCommand, BaseResult<OrderResponse>>
{
    public async Task<BaseResult<OrderResponse>> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
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

        // Kiểm tra nếu đơn hàng đã bị hủy trước đó
        if (order.Status == 6)
        {
            return new BaseResult<OrderResponse>(new Error(ErrorCode.FieldDataInvalid, "Order is already canceled"));
        }

        // Nếu trạng thái là 1 (Pending) hoặc 2 (Paid), cần giảm soldQuantity trong ingredientSession khi hủy đơn hàng
        if (order.Status == 1 || order.Status == 2)
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

        // Cập nhật trạng thái đơn hàng về 6 (Canceled)
        order.Status = 6;
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