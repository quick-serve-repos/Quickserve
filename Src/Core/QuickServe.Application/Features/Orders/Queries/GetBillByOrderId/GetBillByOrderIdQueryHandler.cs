using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using QuickServe.Application.DTOs.Bill;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Wrappers;

namespace QuickServe.Application.Features.Orders.Queries.GetBillByOrderId;

public class GetBillByOrderIdQueryHandler(IOrderRepository orderRepository, IPaymentRepository paymentRepository) : IRequestHandler<GetBillByOrderIdQuery, BaseResult<BillDto>>
{
    public async Task<BaseResult<BillDto>> Handle(GetBillByOrderIdQuery request, CancellationToken cancellationToken)
    {
        // Lấy thông tin đơn hàng
        var order = await orderRepository.GetByIdAsync(request.OrderId);
        if (order == null || order.Status != 2) // Kiểm tra trạng thái đơn hàng đã thanh toán
        {
            return new BaseResult<BillDto>(new Error(ErrorCode.NotFound, "Order not found or not paid"));
        }

        // Lấy thông tin phương thức thanh toán
        var payment = await paymentRepository.GetByOrderIdAsync(request.OrderId);
        // Xử lý logic paymentMethod dựa trên PaymentType
        var paymentMethod = "Unknown";
        if (payment != null)
        {
            paymentMethod = payment.PaymentType switch
            {
                1 => "Tiền mặt",           // PaymentType = 1 => Tiền mặt
                2 => "Thanh toán online",   // PaymentType = 2 => Thanh toán online
                _ => "Unknown"              // Giá trị mặc định nếu không khớp
            };
        }

        // Lấy thông tin cửa hàng
        var store = order.Store;

        // Tạo DTO cho hóa đơn
        var billDto = new BillDto
        {
            StoreName = store.Name,
            StoreAddress = store.Address,
            CurrentDate = DateTime.UtcNow.AddHours(7), // Giờ hiện tại theo UTC+7
            BillNumber = order.BillCode,
            OrderId = order.Id,
            TotalPrice = order.Amount,
            PaymentMethod = paymentMethod,
            Platform = order.Platform,
            Products = new List<BillProductDto>()
        };

        // Duyệt qua sản phẩm trong đơn hàng
        foreach (var orderProduct in order.OrderProducts)
        {
            var productDto = new BillProductDto
            {
                ProductName = orderProduct.Product.Name,
                Quantity = orderProduct.Quantity ?? 0,
                Price = orderProduct.Price,
                Ingredients = new List<BillIngredientDto>()
            };

            // Duyệt qua nguyên liệu của sản phẩm
            foreach (var ingredientProduct in orderProduct.Product.IngredientProducts)
            {
                var ingredientDto = new BillIngredientDto
                {
                    IngredientName = ingredientProduct.Ingredient.Name,
                    Quantity = ingredientProduct.Quantity,
                    Price = ingredientProduct.Ingredient.Price
                };

                productDto.Ingredients.Add(ingredientDto);
            }

            billDto.Products.Add(productDto);
        }

        return new BaseResult<BillDto>(billDto);
    }
}
