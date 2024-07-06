using System;
using QuickServe.Application.Interfaces;
using QuickServe.Application.Wrappers;
using QuickServe.Infrastructure.Persistence.Contexts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuickServe.Application.Utils.Enums;
using QuickServe.Application.Features.Orders.Commands.CreateOrder;
using QuickServe.Domain.Products.Entities;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Interfaces.IOrderServices;
using QuickServe.Utils.Extensions;
using QuickServe.Domain.IngredientProducts.Entities;
using QuickServe.Domain.Orders.Entities;
using QuickServe.Domain.OrderProducts.Entities;
using QuickServe.Application.DTOs.Orders.Response;
using QuickServe.Domain.Customers.Entities;

namespace QuickServe.Infrastructure.Persistence.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductTemplateRepository _productTemplateRepository;
        private readonly ICustomerRepository _customerRepository;

        public OrderService(
            ApplicationDbContext context, 
            IUnitOfWork unitOfWork, 
            IProductTemplateRepository productTemplateRepository,
            ICustomerRepository customerRepository)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _productTemplateRepository = productTemplateRepository;
            _customerRepository = customerRepository;
        }
        public async Task<BaseResult<OrderResponse>> CreateOrderAsync(CreateOrderCommand command)
        {
            if (command.Products == null || !command.Products.Any())
                return new BaseResult<OrderResponse>(new Error(ErrorCode.NotFound));

            Customer account = null;
            if (!string.IsNullOrEmpty(command.PhoneNumber))
            {
                account = await _customerRepository.GetByPhoneAsync(command.PhoneNumber);
                if (account == null)
                {
                    string userName = "user-" + EnumExtension.GenerateUniqueId();
                    account = new Customer()
                    {
                        Id = Guid.NewGuid(),
                        Name = command.Name,
                        UserName = userName,
                        PhoneNumber = command.PhoneNumber
                    };
                    await _context.Customers.AddRangeAsync(account);
                }
            }

            List<Product> products = new List<Product>();
            List<IngredientProduct> ingredientProducts = new List<IngredientProduct>();
            List<OrderProduct> orderProducts = new List<OrderProduct>(); 
            var order = new Order()
            {
                Id = EnumExtension.GenerateUniqueId(),
                CustomerId = account != null ? account.Id : null,
                StoreId = 1 //hardcode storeId => 1
            };

            foreach (var obj in command.Products)
            {
                if (obj == null || obj.ProductTemplateId <= 0) continue;

                var productTemplate = await _productTemplateRepository.GetProductTemplateByIdAsync(obj.ProductTemplateId);
                var product = new Product()
                {
                    Id = EnumExtension.GenerateUniqueId(),
                    Name = productTemplate.Name,
                    Quantity = productTemplate.Quantity,
                    ProductTemplateId = productTemplate.Id,
                    //Price = productTemplate.Price hiện tại ko cộng giá của productTemplate
                };

                //Tính toán nếu có nguyên liệu được thêm vào
                if (obj.Ingredients != null && obj.Ingredients.Any())
                {
                    foreach (var ingre in obj.Ingredients)
                    {
                        var ingredientProduct = new IngredientProduct()
                        {
                            ProductId = product.Id,
                            IngredientId = ingre.Id,
                            Quantity = ingre.Quantity
                        };
                        ingredientProducts.Add(ingredientProduct);

                        product.Price += ingre.Price * ingre.Quantity;
                    }
                }

                products.Add(product);

                //Lưu thông tin orderProduct
                var orderProuct = new OrderProduct()
                {
                    OrderId = order.Id,
                    ProductId = product.Id,
                    Quantity = obj.Quantity
                };
                orderProducts.Add(orderProuct);

                //Tính cộng dồn thông tin order (giá sp sau khi thêm thành phần * số lượng)
                order.Amount += (double)product.Price * obj.Quantity;
                order.Status = (int)OrderStatus.Pending;
            }

            await _context.ProDucts.AddRangeAsync(products);
            if (ingredientProducts.Any())
                await _context.IngredientProducts.AddRangeAsync(ingredientProducts);
            
            await _context.Orders.AddRangeAsync(order);
            await _context.OrderProducts.AddRangeAsync(orderProducts);

            var result = await _unitOfWork.SaveChangesAsync();

            OrderResponse response = new OrderResponse()
            {
                OrderId = result ? order.Id : 0,
                Status = result ? (int)OrderStatus.Pending : (int)OrderStatus.Failed,
            };

            return new BaseResult<OrderResponse>(response);
        }
    }
}
