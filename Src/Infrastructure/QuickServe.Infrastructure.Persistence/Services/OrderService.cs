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
using QuickServe.Infrastructure.Persistence.Repositories;
using QuickServe.Infrastructure.Resources.Services;
using Microsoft.AspNetCore.SignalR;
using QuickServe.Infrastructure.Persistence.Services.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace QuickServe.Infrastructure.Persistence.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductTemplateRepository _productTemplateRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ISessionRepository _sessionRepository;
        private readonly IIngredientSessionRepository _ingredientSessionRepository;
        private readonly ITemplateStepRepository _templateStepRepository;
        private readonly IIngredientTypeRepository _ingredientTypeRepository;
        private readonly IAuthenticatedUserService _authenticatedUserService;
        private readonly IAccountRepository _accountRepository;
        private readonly IHubContext<NotificationHub> _hubContext;


        public OrderService(
            ApplicationDbContext context,
            IUnitOfWork unitOfWork,
            IProductTemplateRepository productTemplateRepository,
            ICustomerRepository customerRepository,
            ISessionRepository sessionRepository,
            IIngredientSessionRepository ingredientSessionRepository,
            ITemplateStepRepository templateStepRepository,
            IIngredientTypeRepository ingredientTypeRepository,
            IAuthenticatedUserService authenticatedUserService,
            IAccountRepository accountRepository,
            IHubContext<NotificationHub> hubContext
        )
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _productTemplateRepository = productTemplateRepository;
            _customerRepository = customerRepository;
            _sessionRepository = sessionRepository;
            _ingredientSessionRepository = ingredientSessionRepository;
            _templateStepRepository = templateStepRepository;
            _ingredientTypeRepository = ingredientTypeRepository;
            _authenticatedUserService = authenticatedUserService;
            _accountRepository = accountRepository;
        }
        /*public async Task<BaseResult<OrderResponse>> CreateOrderAsync(CreateOrderCommand command)
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
                Id = int.Parse(DateTimeOffset.Now.ToString("ffffff")),
                CustomerId = account != null ? account.Id : null,
                //StoreId = 1, //hardcode storeId => 1
                StoreId = command.StoreId,
                BillCode = "Bill-" + EnumExtension.GenerateUniqueId(),
                Platform = 2

            };

            foreach (var obj in command.Products)
            {
                if (obj == null || obj.ProductTemplateId <= 0) continue;
                var productTemplate = await _productTemplateRepository.GetProductTemplateByIdAsync(obj.ProductTemplateId);
                var orderProduct = new OrderProduct();

                var product = new Product()
                {
                    Id = EnumExtension.GenerateUniqueId(),
                    Name = productTemplate.Name,
                    Quantity = obj.Quantity,
                    ProductTemplateId = productTemplate.Id,
                    Price = productTemplate.Price// hiện tại ko cộng giá của productTemplate
                };


                if(!obj.Ingredients.Any())
                {

                    foreach (var step in productTemplate.TemplateSteps)
                    {
                        var temStep = await _templateStepRepository.FindByIdAsync(step.Id);
                        foreach(var ingreStep in temStep.IngredientTypeTemplateSteps)
                        {
                            var ingreType = await _ingredientTypeRepository.GetIngredientTypeByIdAsync(ingreStep.IngredientTypeId);
                            foreach(var igre in ingreType.Ingredients)
                            {
                                if(igre.DefaultQuantity == 0) continue;

                                var sessions = await _sessionRepository.GetAllAsync();
                                var currentSession = sessions.FirstOrDefault(x => x.StartTime <= DateTime.Now.TimeOfDay && x.EndTime >= DateTime.Now.TimeOfDay);

                                if (currentSession != null)
                                {
                                    var ingredientSession = await _ingredientSessionRepository.GetByIdAsync(igre.Id, currentSession.Id);
                                    //Nếu không có ingredientSession nào được khai báo => cho đặt thoải mái
                                    //Trường hợp nếu có => cộng dồn ở SoldQuantity => Để check còn tồn có hợp lệ không
                                    if (ingredientSession != null)
                                    {
                                        //check tồn có đủ đk không
                                        var quantityExist = ingredientSession.SoldQuantity + igre.DefaultQuantity;
                                        if (quantityExist > ingredientSession.Quantity)
                                        {
                                            throw new Exception("Nguyên liệu không đủ số lượng tồn");
                                        }
                                        else
                                        {
                                            ingredientSession.SoldQuantity += igre.DefaultQuantity;
                                        }
                                    }
                                }

                                var ingredientProduct = new IngredientProduct()
                                {
                                    ProductId = product.Id,
                                    IngredientId = igre.Id,
                                    Quantity = igre.DefaultQuantity
                                };
                                ingredientProducts.Add(ingredientProduct);
                                product.Price += igre.Price;
                            }
                        }
                    }
                    products.Add(product);
                    //Lưu thông tin orderProduct
                    orderProduct = new OrderProduct()
                    {
                        OrderId = order.Id,
                        ProductId = product.Id,
                        Quantity = obj.Quantity,
                        Price = product.Price

                    };
                    orderProducts.Add(orderProduct);

                    //Tính cộng dồn thông tin order (giá sp sau khi thêm thành phần * số lượng)
                    order.Amount += (double)product.Price * obj.Quantity;
                    order.Status = (int)OrderStatus.Pending;
                }
                else
                {
                    foreach (var ingre in obj.Ingredients)
                    {
                        var sessions = await _sessionRepository.GetAllAsync();
                        var currentSession = sessions.FirstOrDefault(x => x.StartTime <= DateTime.Now.TimeOfDay && x.EndTime >= DateTime.Now.TimeOfDay);
                        //sau truyền lại sl thì int ingreQuantityDefault = ingre.Quantity;
                        int ingreQuantityDefault = 1;

                        if (currentSession != null)
                        {
                            var ingredientSession = await _ingredientSessionRepository.GetByIdAsync(ingre.Id, currentSession.Id);
                            //Nếu không có ingredientSession nào được khai báo => cho đặt thoải mái
                            //Trường hợp nếu có => cộng dồn ở SoldQuantity => Để check còn tồn có hợp lệ không
                            if (ingredientSession != null)
                            {
                                //check tồn có đủ đk không
                                var quantityExist = ingredientSession.SoldQuantity + ingreQuantityDefault;
                                if (quantityExist > ingredientSession.Quantity)
                                {
                                    throw new Exception("Nguyên liệu không đủ số lượng tồn");
                                }
                                else
                                {
                                    ingredientSession.SoldQuantity += ingreQuantityDefault;
                                }
                            }
                        }

                        var ingredientProduct = new IngredientProduct()
                        {
                            ProductId = product.Id,
                            IngredientId = ingre.Id,
                           // Quantity = ingreQuantityDefault
                           Quantity = ingre.Quantity
                        };
                        ingredientProducts.Add(ingredientProduct);

                        //product.Price += ingre.Price * ingreQuantityDefault;
                        product.Price += ingre.Price * ingre.Quantity;

                    }
                    products.Add(product);

                    //Lưu thông tin orderProduct
                    orderProduct = new OrderProduct()
                    {
                        OrderId = order.Id,
                        ProductId = product.Id,
                        Quantity = obj.Quantity,
                        Price = product.Price
                    };
                    orderProducts.Add(orderProduct);

                    //Tính cộng dồn thông tin order (giá sp sau khi thêm thành phần * số lượng)
                    order.Amount += (double)product.Price * obj.Quantity;
                    order.Status = (int)OrderStatus.Pending;

                }
            }


            await _context.ProDucts.AddRangeAsync(products);
            if (ingredientProducts.Any())
                await _context.IngredientProducts.AddRangeAsync(ingredientProducts);

            await _context.Orders.AddRangeAsync(order);
            await _context.OrderProducts.AddRangeAsync(orderProducts);

            var result = await _unitOfWork.SaveChangesAsync();

            OrderResponse response = new OrderResponse()
            {
                OrderId = result ? order.Id.ToString() : null,
                Status = result ? (int)OrderStatus.Pending : (int)OrderStatus.Failed,
                BillCode = order.BillCode
            };

            return new BaseResult<OrderResponse>(response);
        }*/

        /* public async Task<BaseResult<OrderResponse>> CreateOrderAsync(CreateOrderCommand command)
         {
             if (command.Products == null || !command.Products.Any())
                 return new BaseResult<OrderResponse>(new Error(ErrorCode.NotFound));

             List<Product> products = new List<Product>();
             List<IngredientProduct> ingredientProducts = new List<IngredientProduct>();
             List<OrderProduct> orderProducts = new List<OrderProduct>();

             // Customer information is no longer required
             var order = new Order()
             {
                 Id = int.Parse(DateTimeOffset.Now.ToString("ffffff")),
                 StoreId = command.StoreId, // StoreId is now passed from the command
                 BillCode = "Bill-" + EnumExtension.GenerateUniqueId(),
                 Platform = 2
             };

             foreach (var obj in command.Products)
             {
                 if (obj == null || obj.ProductTemplateId <= 0) continue;
                 var productTemplate =
                     await _productTemplateRepository.GetProductTemplateByIdAsync(obj.ProductTemplateId);
                 var orderProduct = new OrderProduct();

                 var product = new Product()
                 {
                     Id = EnumExtension.GenerateUniqueId(),
                     Name = productTemplate.Name,
                     Quantity = obj.Quantity,
                     ProductTemplateId = productTemplate.Id,
                     //Price = productTemplate.Price
                 };

                 if (!obj.Ingredients.Any())
                 {
                     foreach (var step in productTemplate.TemplateSteps)
                     {
                         var temStep = await _templateStepRepository.FindByIdAsync(step.Id);
                         foreach (var ingreStep in temStep.IngredientTypeTemplateSteps)
                         {
                             var ingreType =
                                 await _ingredientTypeRepository.GetIngredientTypeByIdAsync(ingreStep.IngredientTypeId);
                             foreach (var igre in ingreType.Ingredients)
                             {
                                 if (igre.DefaultQuantity == 0) continue;

                                 var sessions = await _sessionRepository.GetAllAsync();
                                 var currentSession = sessions.FirstOrDefault(x =>
                                     x.StartTime <= DateTime.Now.TimeOfDay && x.EndTime >= DateTime.Now.TimeOfDay);

                                 if (currentSession != null)
                                 {
                                     var ingredientSession =
                                         await _ingredientSessionRepository.GetByIdAsync(igre.Id, currentSession.Id);
                                     if (ingredientSession != null)
                                     {
                                         var quantityExist = ingredientSession.SoldQuantity + igre.DefaultQuantity;
                                         if (quantityExist > ingredientSession.Quantity)
                                         {
                                             throw new Exception("Nguyên liệu không đủ số lượng tồn");
                                         }
                                         else
                                         {
                                             ingredientSession.SoldQuantity += igre.DefaultQuantity;
                                         }
                                     }
                                 }

                                 var ingredientProduct = new IngredientProduct()
                                 {
                                     ProductId = product.Id,
                                     IngredientId = igre.Id,
                                     Quantity = igre.DefaultQuantity
                                 };
                                 ingredientProducts.Add(ingredientProduct);
                                 product.Price += igre.Price;
                             }
                         }
                     }

                     products.Add(product);
                     orderProduct = new OrderProduct()
                     {
                         OrderId = order.Id,
                         ProductId = product.Id,
                         Quantity = obj.Quantity,
                         Price = product.Price
                     };
                     orderProducts.Add(orderProduct);
                     order.Amount += (double)product.Price * obj.Quantity;
                     order.Status = (int)OrderStatus.Pending;
                 }
                 else
                 {
                     foreach (var ingre in obj.Ingredients)
                     {
                         var sessions = await _sessionRepository.GetAllAsync();
                         var currentSession = sessions.FirstOrDefault(x =>
                             x.StartTime <= DateTime.Now.TimeOfDay && x.EndTime >= DateTime.Now.TimeOfDay);

                         if (currentSession != null)
                         {
                             var ingredientSession =
                                 await _ingredientSessionRepository.GetByIdAsync(ingre.Id, currentSession.Id);
                             if (ingredientSession != null)
                             {
                                 var quantityExist = ingredientSession.SoldQuantity + ingre.Quantity;
                                 if (quantityExist > ingredientSession.Quantity)
                                 {
                                     throw new Exception("Nguyên liệu không đủ số lượng tồn");
                                 }
                                 else
                                 {
                                     ingredientSession.SoldQuantity += ingre.Quantity;
                                 }
                             }
                         }

                         var ingredientProduct = new IngredientProduct()
                         {
                             ProductId = product.Id,
                             IngredientId = ingre.Id,
                             Quantity = ingre.Quantity
                         };
                         ingredientProducts.Add(ingredientProduct);
                         product.Price += ingre.Price * ingre.Quantity;
                     }

                     products.Add(product);
                     orderProduct = new OrderProduct()
                     {
                         OrderId = order.Id,
                         ProductId = product.Id,
                         Quantity = obj.Quantity,
                         Price = product.Price
                     };
                     orderProducts.Add(orderProduct);
                     order.Amount += (double)product.Price * obj.Quantity;
                     order.Status = (int)OrderStatus.Pending;
                 }
             }

             await _context.ProDucts.AddRangeAsync(products);
             if (ingredientProducts.Any())
                 await _context.IngredientProducts.AddRangeAsync(ingredientProducts);

             await _context.Orders.AddRangeAsync(order);
             await _context.OrderProducts.AddRangeAsync(orderProducts);

             var result = await _unitOfWork.SaveChangesAsync();

             OrderResponse response = new OrderResponse()
             {
                 OrderId = result ? order.Id.ToString() : null,
                 Status = result ? (int)OrderStatus.Pending : (int)OrderStatus.Failed,
                 BillCode = order.BillCode
             };

             return new BaseResult<OrderResponse>(response);
         }*/


        //đặt hàng được nhưng chưa check nếu đặt hàng không nằm trong session thì có đặt được không?
        /*public async Task<BaseResult<OrderResponse>> CreateOrderAsync(CreateOrderCommand command)
        {
            if (command.Products == null || !command.Products.Any())
                return new BaseResult<OrderResponse>(new Error(ErrorCode.NotFound));

            List<Product> products = new List<Product>();
            List<IngredientProduct> ingredientProducts = new List<IngredientProduct>();
            List<OrderProduct> orderProducts = new List<OrderProduct>();

            var order = new Order()
            {
                Id = int.Parse(DateTimeOffset.Now.ToString("ffffff")),
                StoreId = command.StoreId, // StoreId is now passed from the command
                BillCode = "Bill-" + EnumExtension.GenerateUniqueId(),
                Platform = 2
            };

            foreach (var obj in command.Products)
            {
                if (obj == null || obj.ProductTemplateId <= 0) continue;
                var productTemplate =
                    await _productTemplateRepository.GetProductTemplateByIdAsync(obj.ProductTemplateId);
                var orderProduct = new OrderProduct();

                var product = new Product()
                {
                    Id = EnumExtension.GenerateUniqueId(),
                    Name = productTemplate.Name,
                    Quantity = obj.Quantity,
                    ProductTemplateId = productTemplate.Id,
                };

                if (!obj.Ingredients.Any())
                {
                    foreach (var step in productTemplate.TemplateSteps)
                    {
                        var temStep = await _templateStepRepository.FindByIdAsync(step.Id);
                        foreach (var ingreStep in temStep.IngredientTypeTemplateSteps)
                        {
                            var ingreType =
                                await _ingredientTypeRepository.GetIngredientTypeByIdAsync(ingreStep.IngredientTypeId);
                            foreach (var igre in ingreType.Ingredients)
                            {
                                if (igre.DefaultQuantity == 0) continue;

                                var sessions = await _sessionRepository.GetAllAsync();
                                var currentSession = sessions.FirstOrDefault(x =>
                                    x.StartTime <= DateTime.Now.TimeOfDay && x.EndTime >= DateTime.Now.TimeOfDay);

                                if (currentSession != null)
                                {
                                    var ingredientSession =
                                        await _ingredientSessionRepository.GetByIdAsync(igre.Id, currentSession.Id);
                                    if (ingredientSession != null)
                                    {
                                        // Kiểm tra số lượng yêu cầu có thỏa mãn số lượng tồn kho không
                                        if (igre.DefaultQuantity >
                                            (ingredientSession.Quantity - ingredientSession.SoldQuantity))
                                        {
                                            throw new Exception("Nguyên liệu không đủ số lượng tồn.");
                                        }
                                    }
                                }

                                var ingredientProduct = new IngredientProduct()
                                {
                                    ProductId = product.Id,
                                    IngredientId = igre.Id,
                                    Quantity = igre.DefaultQuantity
                                };
                                ingredientProducts.Add(ingredientProduct);
                                product.Price += igre.Price;
                            }
                        }
                    }

                    products.Add(product);
                    orderProduct = new OrderProduct()
                    {
                        OrderId = order.Id,
                        ProductId = product.Id,
                        Quantity = obj.Quantity,
                        Price = product.Price
                    };
                    orderProducts.Add(orderProduct);
                    order.Amount += (double)product.Price * obj.Quantity;
                    order.Status = (int)OrderStatus.Pending;
                }
                else
                {
                    foreach (var ingre in obj.Ingredients)
                    {
                        // Lấy giờ hiện tại theo UTC+7
                        var utcNow = DateTime.UtcNow.AddHours(7);
                        var currentTimeOfDay = utcNow.TimeOfDay;

                        // Lấy phiên hiện tại dựa trên thời gian UTC+7
                        var sessions = await _sessionRepository.GetAllAsync();
                        var currentSession = sessions.FirstOrDefault(x =>
                            x.StartTime <= currentTimeOfDay && x.EndTime >= currentTimeOfDay);

                        if (currentSession != null)
                        {
                            var ingredientSession =
                                await _ingredientSessionRepository.GetByIdAsync(ingre.Id, currentSession.Id);
                            if (ingredientSession != null)
                            {
                                // Kiểm tra số lượng yêu cầu có thỏa mãn số lượng tồn kho không
                                if (ingre.Quantity > (ingredientSession.Quantity - ingredientSession.SoldQuantity))
                                {
                                    throw new Exception("Nguyên liệu không đủ số lượng tồn.");
                                }
                            }
                        }

                        var ingredientProduct = new IngredientProduct()
                        {
                            ProductId = product.Id,
                            IngredientId = ingre.Id,
                            Quantity = ingre.Quantity
                        };
                        ingredientProducts.Add(ingredientProduct);
                        product.Price += ingre.Price * ingre.Quantity;
                    }

                    products.Add(product);
                    orderProduct = new OrderProduct()
                    {
                        OrderId = order.Id,
                        ProductId = product.Id,
                        Quantity = obj.Quantity,
                        Price = product.Price
                    };
                    orderProducts.Add(orderProduct);
                    order.Amount += (double)product.Price * obj.Quantity;
                    order.Status = (int)OrderStatus.Pending;
                }
            }

            await _context.ProDucts.AddRangeAsync(products);
            if (ingredientProducts.Any())
                await _context.IngredientProducts.AddRangeAsync(ingredientProducts);

            await _context.Orders.AddRangeAsync(order);
            await _context.OrderProducts.AddRangeAsync(orderProducts);

            var result = await _unitOfWork.SaveChangesAsync();

            OrderResponse response = new OrderResponse()
            {
                OrderId = result ? order.Id.ToString() : null,
                Status = result ? (int)OrderStatus.Pending : (int)OrderStatus.Failed,
                BillCode = order.BillCode
            };

            return new BaseResult<OrderResponse>(response);
        }*/
        // 26/08/204 : dat duoc.
        /* public async Task<BaseResult<OrderResponse>> CreateOrderAsync(CreateOrderCommand command)
         {
             if (command.Products == null || !command.Products.Any())
                 return new BaseResult<OrderResponse>(new Error(ErrorCode.NotFound));

             // Lấy giờ hiện tại theo UTC+7
             var utcNow = DateTime.UtcNow.AddHours(7);
             var currentTimeOfDay = utcNow.TimeOfDay;

             // Lấy phiên hiện tại dựa trên thời gian UTC+7
             var sessions = await _sessionRepository.GetAllAsync();
             var currentSession = sessions.FirstOrDefault(x =>
                 x.StartTime <= currentTimeOfDay && x.EndTime >= currentTimeOfDay);

             // Kiểm tra xem có phiên hiện tại không, nếu không có thì không cho phép đặt hàng
             if (currentSession == null || currentSession.Status != 2)
             {
                 return new BaseResult<OrderResponse>(new Error(ErrorCode.SessionNotFound,
                     "Không có phiên làm việc nào đang hoạt động, không thể đặt hàng"));
             }

             List<Product> products = new List<Product>();
             List<IngredientProduct> ingredientProducts = new List<IngredientProduct>();
             List<OrderProduct> orderProducts = new List<OrderProduct>();

             var order = new Order()
             {
                 Id = int.Parse(DateTimeOffset.Now.ToString("ffffff")),
                 StoreId = command.StoreId, // StoreId is now passed from the command
                 BillCode = "Bill-" + EnumExtension.GenerateUniqueId(),
                 Platform = 2
             };

             foreach (var obj in command.Products)
             {
                 if (obj == null || obj.ProductTemplateId <= 0) continue;
                 var productTemplate =
                     await _productTemplateRepository.GetProductTemplateByIdAsync(obj.ProductTemplateId);
                 var orderProduct = new OrderProduct();

                 var product = new Product()
                 {
                     Id = EnumExtension.GenerateUniqueId(),
                     Name = productTemplate.Name,
                     Quantity = obj.Quantity,
                     ProductTemplateId = productTemplate.Id,
                 };

                 if (!obj.Ingredients.Any())
                 {
                     foreach (var step in productTemplate.TemplateSteps)
                     {
                         var temStep = await _templateStepRepository.FindByIdAsync(step.Id);
                         foreach (var ingreStep in temStep.IngredientTypeTemplateSteps)
                         {
                             var ingreType =
                                 await _ingredientTypeRepository.GetIngredientTypeByIdAsync(ingreStep.IngredientTypeId);
                             foreach (var igre in ingreType.Ingredients)
                             {
                                 if (igre.DefaultQuantity == 0) continue;

                                 var ingredientSession =
                                     await _ingredientSessionRepository.GetByIdAsync(igre.Id, currentSession.Id);
                                 if (ingredientSession != null)
                                 {
                                     // Kiểm tra số lượng yêu cầu có thỏa mãn số lượng tồn kho không
                                     if (igre.DefaultQuantity >
                                         (ingredientSession.Quantity - ingredientSession.SoldQuantity))
                                     {
                                         //throw new Exception("Nguyên liệu không đủ số lượng tồn.");
                                         return new BaseResult<OrderResponse>(new Error(
                                             ErrorCode.NotEnoughQuantityIngredient,
                                             "Nguyên liệu không đủ số lượng tồn."));
                                     }
                                     // Dự trữ trước (Pre-allocate)
                                     ingredientSession.SoldQuantity += ingre.Quantity;
                                     await _ingredientSessionRepository.Update(ingredientSession); // Cập nhật ngay lập tức vào kho
                                 }

                                 var ingredientProduct = new IngredientProduct()
                                 {
                                     ProductId = product.Id,
                                     IngredientId = igre.Id,
                                     Quantity = igre.DefaultQuantity
                                 };
                                 ingredientProducts.Add(ingredientProduct);
                                 product.Price += igre.Price;
                             }
                         }
                     }

                     products.Add(product);
                     orderProduct = new OrderProduct()
                     {
                         OrderId = order.Id,
                         ProductId = product.Id,
                         Quantity = obj.Quantity,
                         Price = product.Price
                     };
                     orderProducts.Add(orderProduct);
                     order.Amount += (double)product.Price * obj.Quantity;
                     order.Status = (int)OrderStatus.Pending;
                 }
                 else
                 {
                     foreach (var ingre in obj.Ingredients)
                     {
                         var ingredientSession =
                             await _ingredientSessionRepository.GetByIdAsync(ingre.Id, currentSession.Id);
                         if (ingredientSession != null)
                         {
                             // Kiểm tra số lượng yêu cầu có thỏa mãn số lượng tồn kho không
                             if (ingre.Quantity > (ingredientSession.Quantity - ingredientSession.SoldQuantity))
                             {
                                 //throw new Exception("Nguyên liệu không đủ số lượng tồn.");
                                 return new BaseResult<OrderResponse>(new Error(
                                     ErrorCode.NotEnoughQuantityIngredient,
                                     "Nguyên liệu không đủ số lượng tồn."));
                             }
                         }

                         var ingredientProduct = new IngredientProduct()
                         {
                             ProductId = product.Id,
                             IngredientId = ingre.Id,
                             Quantity = ingre.Quantity
                         };
                         ingredientProducts.Add(ingredientProduct);
                         product.Price += ingre.Price * ingre.Quantity;
                     }

                     products.Add(product);
                     orderProduct = new OrderProduct()
                     {
                         OrderId = order.Id,
                         ProductId = product.Id,
                         Quantity = obj.Quantity,
                         Price = product.Price
                     };
                     orderProducts.Add(orderProduct);
                     order.Amount += (double)product.Price * obj.Quantity;
                     order.Status = (int)OrderStatus.Pending;
                 }
             }

             await _context.ProDucts.AddRangeAsync(products);
             if (ingredientProducts.Any())
                 await _context.IngredientProducts.AddRangeAsync(ingredientProducts);

             await _context.Orders.AddRangeAsync(order);
             await _context.OrderProducts.AddRangeAsync(orderProducts);

             var result = await _unitOfWork.SaveChangesAsync();

             OrderResponse response = new OrderResponse()
             {
                 OrderId = result ? order.Id.ToString() : null,
                 Status = result ? (int)OrderStatus.Pending : (int)OrderStatus.Failed,
                 BillCode = order.BillCode
             };

             return new BaseResult<OrderResponse>(response);
         }*/

        // 22:00:00 -27-08-2024
        /*    public async Task<BaseResult<OrderResponse>> CreateOrderAsync(CreateOrderCommand command)
    {
        if (command.Products == null || !command.Products.Any())
            return new BaseResult<OrderResponse>(new Error(ErrorCode.NotFound));

        // Lấy giờ hiện tại theo UTC+7
        var utcNow = DateTime.UtcNow.AddHours(7);
        var currentTimeOfDay = utcNow.TimeOfDay;

        // Lấy phiên hiện tại dựa trên thời gian UTC+7
        var sessions = await _sessionRepository.GetAllAsync();
        var currentSession = sessions.FirstOrDefault(x =>
            x.StartTime <= currentTimeOfDay && x.EndTime >= currentTimeOfDay);

        // Kiểm tra xem có phiên hiện tại không và session này phải có status = 2
        if (currentSession == null || currentSession.Status != 1)
        {
            return new BaseResult<OrderResponse>(new Error(ErrorCode.SessionNotFound,
                "Không có phiên làm việc nào đang hoạt động hoặc phiên này không hợp lệ, không thể đặt hàng"));
        }

        List<Product> products = new List<Product>();
        List<IngredientProduct> ingredientProducts = new List<IngredientProduct>();
        List<OrderProduct> orderProducts = new List<OrderProduct>();

        var order = new Order()
        {
            Id = int.Parse(DateTimeOffset.Now.ToString("ffffff")),
            StoreId = command.StoreId, // StoreId is now passed from the command
            BillCode = "Bill-" + EnumExtension.GenerateUniqueId(),
            Platform = 2,
            Status = (int)OrderStatus.Pending // Tình trạng chờ thanh toán
        };

        foreach (var obj in command.Products)
        {
            if (obj == null || obj.ProductTemplateId <= 0) continue;
            var productTemplate =
                await _productTemplateRepository.GetProductTemplateByIdAsync(obj.ProductTemplateId);
            var orderProduct = new OrderProduct();

            var product = new Product()
            {
                Id = EnumExtension.GenerateUniqueId(),
                Name = productTemplate.Name,
                Quantity = obj.Quantity,
                ProductTemplateId = productTemplate.Id,
            };

            if (!obj.Ingredients.Any())
            {
                foreach (var step in productTemplate.TemplateSteps)
                {
                    var temStep = await _templateStepRepository.FindByIdAsync(step.Id);
                    foreach (var ingreStep in temStep.IngredientTypeTemplateSteps)
                    {
                        var ingreType =
                            await _ingredientTypeRepository.GetIngredientTypeByIdAsync(ingreStep.IngredientTypeId);
                        foreach (var igre in ingreType.Ingredients)
                        {
                            if (igre.DefaultQuantity == 0) continue;

                            var ingredientSession =
                                await _ingredientSessionRepository.GetByIdAsync(igre.Id, currentSession.Id);
                            if (ingredientSession != null)
                            {
                                // Kiểm tra số lượng tồn kho và trừ ngay số lượng yêu cầu (Pre-allocation)
                                if (igre.DefaultQuantity > (ingredientSession.Quantity - ingredientSession.SoldQuantity))
                                {
                                    return new BaseResult<OrderResponse>(new Error(
                                        ErrorCode.NotEnoughQuantityIngredient,
                                        "Nguyên liệu không đủ số lượng tồn."));
                                }

                                // Dự trữ trước (Pre-allocate)
                                ingredientSession.SoldQuantity += igre.DefaultQuantity;
                                 _ingredientSessionRepository.Update(ingredientSession); // Cập nhật ngay lập tức vào kho
                            }

                            var ingredientProduct = new IngredientProduct()
                            {
                                ProductId = product.Id,
                                IngredientId = igre.Id,
                                Quantity = igre.DefaultQuantity
                            };
                            ingredientProducts.Add(ingredientProduct);
                            product.Price += igre.Price;
                        }
                    }
                }

                products.Add(product);
                orderProduct = new OrderProduct()
                {
                    OrderId = order.Id,
                    ProductId = product.Id,
                    Quantity = obj.Quantity,
                    Price = product.Price
                };
                orderProducts.Add(orderProduct);
                order.Amount += (double)product.Price * obj.Quantity;
            }
            else
            {
                foreach (var ingre in obj.Ingredients)
                {
                    var ingredientSession =
                        await _ingredientSessionRepository.GetByIdAsync(ingre.Id, currentSession.Id);
                    if (ingredientSession != null)
                    {
                        // Kiểm tra số lượng tồn kho và trừ ngay số lượng yêu cầu (Pre-allocation)
                        if (ingre.Quantity > (ingredientSession.Quantity - ingredientSession.SoldQuantity))
                        {
                            return new BaseResult<OrderResponse>(new Error(
                                ErrorCode.NotEnoughQuantityIngredient,
                                "Nguyên liệu không đủ số lượng tồn."));
                        }

                        // Dự trữ trước (Pre-allocate)
                        ingredientSession.SoldQuantity += ingre.Quantity;
                        _ingredientSessionRepository.Update(ingredientSession); // Cập nhật ngay lập tức vào kho
                    }

                    var ingredientProduct = new IngredientProduct()
                    {
                        ProductId = product.Id,
                        IngredientId = ingre.Id,
                        Quantity = ingre.Quantity
                    };
                    ingredientProducts.Add(ingredientProduct);
                    product.Price += ingre.Price * ingre.Quantity;
                }

                products.Add(product);
                orderProduct = new OrderProduct()
                {
                    OrderId = order.Id,
                    ProductId = product.Id,
                    Quantity = obj.Quantity,
                    Price = product.Price
                };
                orderProducts.Add(orderProduct);
                order.Amount += (double)product.Price * obj.Quantity;
            }
        }

        await _context.ProDucts.AddRangeAsync(products);
        if (ingredientProducts.Any())
            await _context.IngredientProducts.AddRangeAsync(ingredientProducts);

        await _context.Orders.AddRangeAsync(order);
        await _context.OrderProducts.AddRangeAsync(orderProducts);

        var result = await _unitOfWork.SaveChangesAsync();

        OrderResponse response = new OrderResponse()
        {
            OrderId = result ? order.Id.ToString() : null,
            Status = result ? (int)OrderStatus.Pending : (int)OrderStatus.Failed,
            BillCode = order.BillCode
        };

        return new BaseResult<OrderResponse>(response);
    }   */

        public async Task<BaseResult<OrderResponse>> CreateOrderAsync(CreateOrderCommand command)
        {
            if (command.Products == null || !command.Products.Any())
                return new BaseResult<OrderResponse>(new Error(ErrorCode.NotFound));

            List<Product> products = new List<Product>();
            List<IngredientProduct> ingredientProducts = new List<IngredientProduct>();
            List<OrderProduct> orderProducts = new List<OrderProduct>();

            // Lấy giờ hiện tại theo UTC+7
            var utcNow = DateTime.UtcNow.AddHours(7);
            var currentTimeOfDay = utcNow.TimeOfDay;

            // Lấy phiên hiện tại dựa trên thời gian UTC+7
            var sessions = await _sessionRepository.GetAllAsync();
            var currentSession = sessions.FirstOrDefault(x =>
                x.StartTime <= currentTimeOfDay && x.EndTime >= currentTimeOfDay);

            // Kiểm tra xem có phiên hiện tại không và session này phải có status = 2
            if (currentSession == null || currentSession.Status != 1)
            {
                return new BaseResult<OrderResponse>(new Error(ErrorCode.SessionNotFound,
                    "Không có phiên làm việc nào đang hoạt động hoặc phiên không hợp lệ, không thể đặt hàng"));
            }

            var order = new Order()
            {
                Id = int.Parse(DateTimeOffset.Now.ToString("ffffff")),
                StoreId = command.StoreId,
                BillCode = "Bill-" + EnumExtension.GenerateUniqueId(),
                Platform = 2
            };

            foreach (var obj in command.Products)
            {
                if (obj == null || obj.ProductTemplateId <= 0) continue;
                var productTemplate =
                    await _productTemplateRepository.GetProductTemplateByIdAsync(obj.ProductTemplateId);
                var orderProduct = new OrderProduct();

                var product = new Product()
                {
                    Id = EnumExtension.GenerateUniqueId(),
                    Name = productTemplate.Name,
                    Quantity = obj.Quantity,
                    ProductTemplateId = productTemplate.Id,
                };

                if (!obj.Ingredients.Any())
                {
                    foreach (var step in productTemplate.TemplateSteps)
                    {
                        var temStep = await _templateStepRepository.FindByIdAsync(step.Id);
                        foreach (var ingreStep in temStep.IngredientTypeTemplateSteps)
                        {
                            var ingreType =
                                await _ingredientTypeRepository.GetIngredientTypeByIdAsync(ingreStep.IngredientTypeId);
                            foreach (var igre in ingreType.Ingredients)
                            {
                                if (igre.DefaultQuantity == 0) continue;

                                var ingredientSession =
                                    await _ingredientSessionRepository.GetByIdAsync(igre.Id, currentSession.Id);
                                if (ingredientSession != null)
                                {
                                    // Kiểm tra số lượng tồn kho dựa trên `ingredientSession.Quantity`
                                    if (ingredientSession.Quantity - ingredientSession.SoldQuantity <
                                        obj.Quantity)
                                    {
                                        return new BaseResult<OrderResponse>(new Error(
                                            ErrorCode.NotEnoughQuantityIngredient,
                                            "Nguyên liệu không đủ số lượng tồn."));
                                    }

                                    // Dự trữ trước (Pre-allocate)
                                    ingredientSession.SoldQuantity += obj.Quantity;
                                    _ingredientSessionRepository.Update(ingredientSession);
                                }

                                var ingredientProduct = new IngredientProduct()
                                {
                                    ProductId = product.Id,
                                    IngredientId = igre.Id,
                                    Quantity = obj.Quantity
                                };
                                ingredientProducts.Add(ingredientProduct);
                                product.Price += igre.Price;
                            }
                        }
                    }

                    products.Add(product);
                    orderProduct = new OrderProduct()
                    {
                        OrderId = order.Id,
                        ProductId = product.Id,
                        Quantity = obj.Quantity,
                        Price = product.Price
                    };
                    orderProducts.Add(orderProduct);
                    order.Amount += (double)product.Price * obj.Quantity;
                    order.Status = (int)OrderStatus.Pending;
                }
                else
                {
                    foreach (var ingre in obj.Ingredients)
                    {
                        var ingredientSession =
                            await _ingredientSessionRepository.GetByIdAsync(ingre.Id, currentSession.Id);
                        if (ingredientSession != null)
                        {
                            // Kiểm tra số lượng tồn kho dựa trên `ingredientSession.Quantity`
                            if (ingredientSession.Quantity - ingredientSession.SoldQuantity <
                                obj.Quantity)
                            {
                                return new BaseResult<OrderResponse>(new Error(
                                    ErrorCode.NotEnoughQuantityIngredient,
                                    "Nguyên liệu không đủ số lượng tồn."));
                            }
                            else
                            {
                                // Sử dụng `ingre.Quantity` để cộng chính xác số lượng được bán
                                ingredientSession.SoldQuantity += ingre.Quantity;
                            }
                        }

                        var ingredientProduct = new IngredientProduct()
                        {
                            ProductId = product.Id,
                            IngredientId = ingre.Id,
                            Quantity = ingre.Quantity
                        };
                        ingredientProducts.Add(ingredientProduct);
                        product.Price += ingre.Price * ingre.Quantity;
                    }

                    products.Add(product);
                    orderProduct = new OrderProduct()
                    {
                        OrderId = order.Id,
                        ProductId = product.Id,
                        Quantity = obj.Quantity,
                        Price = product.Price
                    };
                    orderProducts.Add(orderProduct);
                    order.Amount += (double)product.Price * obj.Quantity;
                    order.Status = (int)OrderStatus.Pending;
                }
            }

            await _context.ProDucts.AddRangeAsync(products);
            if (ingredientProducts.Any())
                await _context.IngredientProducts.AddRangeAsync(ingredientProducts);

            await _context.Orders.AddRangeAsync(order);
            await _context.OrderProducts.AddRangeAsync(orderProducts);

            var result = await _unitOfWork.SaveChangesAsync();

            OrderResponse response = new OrderResponse()
            {
                OrderId = result ? order.Id.ToString() : null,
                Status = result ? (int)OrderStatus.Pending : (int)OrderStatus.Failed,
                BillCode = order.BillCode
            };

            return new BaseResult<OrderResponse>(response);
        }


        /*public async Task<BaseResult<OrderResponse>> CreateOrderForCustomerAsync(CreateOrderCommand command)
        {
            if (command.Products == null || !command.Products.Any())
                return new BaseResult<OrderResponse>(new Error(ErrorCode.NotFound));

            List<Product> products = new List<Product>();
            List<IngredientProduct> ingredientProducts = new List<IngredientProduct>();
            List<OrderProduct> orderProducts = new List<OrderProduct>();


            var userId = _authenticatedUserService.UserId;

            var currentUser = await _accountRepository.FindByIdAsync(Guid.Parse(userId));
            Guid? customerId = null;
            if (currentUser is Customer customer)
            {
                customerId = customer.Id; // Assuming the 'Id' in the Customer class is the customerId
            }

            if (customerId == null)
            {
                throw new Exception("User is not a customer");
            }

            // Customer information is no longer required
            var order = new Order()
            {
                Id = int.Parse(DateTimeOffset.Now.ToString("ffffff")),
                StoreId = command.StoreId, // StoreId is now passed from the command
                BillCode = "Bill-" + EnumExtension.GenerateUniqueId(),
                CustomerId = customerId,
                Platform = 1
            };

            foreach (var obj in command.Products)
            {
                if (obj == null || obj.ProductTemplateId <= 0) continue;
                var productTemplate =
                    await _productTemplateRepository.GetProductTemplateByIdAsync(obj.ProductTemplateId);
                var orderProduct = new OrderProduct();

                var product = new Product()
                {
                    Id = EnumExtension.GenerateUniqueId(),
                    Name = productTemplate.Name,
                    Quantity = obj.Quantity,
                    ProductTemplateId = productTemplate.Id,
                    //Price = productTemplate.Price
                };

                if (!obj.Ingredients.Any())
                {
                    foreach (var step in productTemplate.TemplateSteps)
                    {
                        var temStep = await _templateStepRepository.FindByIdAsync(step.Id);
                        foreach (var ingreStep in temStep.IngredientTypeTemplateSteps)
                        {
                            var ingreType =
                                await _ingredientTypeRepository.GetIngredientTypeByIdAsync(ingreStep.IngredientTypeId);
                            foreach (var igre in ingreType.Ingredients)
                            {
                                if (igre.DefaultQuantity == 0) continue;

                                var sessions = await _sessionRepository.GetAllAsync();
                                var currentSession = sessions.FirstOrDefault(x =>
                                    x.StartTime <= DateTime.Now.TimeOfDay && x.EndTime >= DateTime.Now.TimeOfDay);

                                if (currentSession != null)
                                {
                                    var ingredientSession =
                                        await _ingredientSessionRepository.GetByIdAsync(igre.Id, currentSession.Id);
                                    if (ingredientSession != null)
                                    {
                                        var quantityExist = ingredientSession.SoldQuantity + igre.DefaultQuantity;
                                        if (quantityExist > ingredientSession.Quantity)
                                        {
                                            throw new Exception("Nguyên liệu không đủ số lượng tồn");
                                        }
                                        else
                                        {
                                            ingredientSession.SoldQuantity += igre.DefaultQuantity;
                                        }
                                    }
                                }

                                var ingredientProduct = new IngredientProduct()
                                {
                                    ProductId = product.Id,
                                    IngredientId = igre.Id,
                                    Quantity = igre.DefaultQuantity
                                };
                                ingredientProducts.Add(ingredientProduct);
                                product.Price += igre.Price;
                            }
                        }
                    }

                    products.Add(product);
                    orderProduct = new OrderProduct()
                    {
                        OrderId = order.Id,
                        ProductId = product.Id,
                        Quantity = obj.Quantity,
                        Price = product.Price
                    };
                    orderProducts.Add(orderProduct);
                    order.Amount += (double)product.Price * obj.Quantity;
                    order.Status = (int)OrderStatus.Pending;
                }
                else
                {
                    foreach (var ingre in obj.Ingredients)
                    {
                        var sessions = await _sessionRepository.GetAllAsync();
                        var currentSession = sessions.FirstOrDefault(x =>
                            x.StartTime <= DateTime.Now.TimeOfDay && x.EndTime >= DateTime.Now.TimeOfDay);

                        if (currentSession != null)
                        {
                            var ingredientSession =
                                await _ingredientSessionRepository.GetByIdAsync(ingre.Id, currentSession.Id);
                            if (ingredientSession != null)
                            {
                                var quantityExist = ingredientSession.SoldQuantity + ingre.Quantity;
                                if (quantityExist > ingredientSession.Quantity)
                                {
                                    throw new Exception("Nguyên liệu không đủ số lượng tồn");
                                }
                                else
                                {
                                    ingredientSession.SoldQuantity += ingre.Quantity;
                                }
                            }
                        }

                        var ingredientProduct = new IngredientProduct()
                        {
                            ProductId = product.Id,
                            IngredientId = ingre.Id,
                            Quantity = ingre.Quantity
                        };
                        ingredientProducts.Add(ingredientProduct);
                        product.Price += ingre.Price * ingre.Quantity;
                    }

                    products.Add(product);
                    orderProduct = new OrderProduct()
                    {
                        OrderId = order.Id,
                        ProductId = product.Id,
                        Quantity = obj.Quantity,
                        Price = product.Price
                    };
                    orderProducts.Add(orderProduct);
                    order.Amount += (double)product.Price * obj.Quantity;
                    order.Status = (int)OrderStatus.Pending;
                }
            }

            await _context.ProDucts.AddRangeAsync(products);
            if (ingredientProducts.Any())
                await _context.IngredientProducts.AddRangeAsync(ingredientProducts);

            await _context.Orders.AddRangeAsync(order);
            await _context.OrderProducts.AddRangeAsync(orderProducts);

            var result = await _unitOfWork.SaveChangesAsync();

            OrderResponse response = new OrderResponse()
            {
                OrderId = result ? order.Id.ToString() : null,
                Status = result ? (int)OrderStatus.Pending : (int)OrderStatus.Failed,
                BillCode = order.BillCode
            };

            return new BaseResult<OrderResponse>(response);
        }*/

        /*public async Task<BaseResult<OrderResponse>> CreateOrderForCustomerAsync(CreateOrderCommand command)
        {
            if (command.Products == null || !command.Products.Any())
                return new BaseResult<OrderResponse>(new Error(ErrorCode.NotFound));

            List<Product> products = new List<Product>();
            List<IngredientProduct> ingredientProducts = new List<IngredientProduct>();
            List<OrderProduct> orderProducts = new List<OrderProduct>();

            var userId = _authenticatedUserService.UserId;

            var currentUser = await _accountRepository.FindByIdAsync(Guid.Parse(userId));
            Guid? customerId = null;
            if (currentUser is Customer customer)
            {
                customerId = customer.Id; // Assuming the 'Id' in the Customer class is the customerId
            }

            if (customerId == null)
            {
                throw new Exception("User is not a customer");
            }

            // Create new order
            var order = new Order()
            {
                Id = int.Parse(DateTimeOffset.Now.ToString("ffffff")),
                StoreId = command.StoreId,
                BillCode = "Bill-" + EnumExtension.GenerateUniqueId(),
                CustomerId = customerId,
                Platform = 1
            };

            foreach (var obj in command.Products)
            {
                if (obj == null || obj.ProductTemplateId <= 0) continue;
                var productTemplate =
                    await _productTemplateRepository.GetProductTemplateByIdAsync(obj.ProductTemplateId);
                var orderProduct = new OrderProduct();

                var product = new Product()
                {
                    Id = EnumExtension.GenerateUniqueId(),
                    Name = productTemplate.Name,
                    Quantity = obj.Quantity,
                    ProductTemplateId = productTemplate.Id,
                };

                if (!obj.Ingredients.Any())
                {
                    foreach (var step in productTemplate.TemplateSteps)
                    {
                        var temStep = await _templateStepRepository.FindByIdAsync(step.Id);
                        foreach (var ingreStep in temStep.IngredientTypeTemplateSteps)
                        {
                            var ingreType =
                                await _ingredientTypeRepository.GetIngredientTypeByIdAsync(ingreStep.IngredientTypeId);
                            foreach (var igre in ingreType.Ingredients)
                            {
                                if (igre.DefaultQuantity == 0) continue;

                                // Lấy giờ hiện tại theo UTC+7
                                var utcNow = DateTime.UtcNow.AddHours(7);
                                var currentTimeOfDay = utcNow.TimeOfDay;

                                // Lấy phiên hiện tại dựa trên thời gian UTC+7
                                var sessions = await _sessionRepository.GetAllAsync();
                                var currentSession = sessions.FirstOrDefault(x =>
                                    x.StartTime <= currentTimeOfDay && x.EndTime >= currentTimeOfDay);

                                if (currentSession != null)
                                {
                                    var ingredientSession =
                                        await _ingredientSessionRepository.GetByIdAsync(igre.Id, currentSession.Id);
                                    if (ingredientSession != null)
                                    {
                                        // Kiểm tra số lượng yêu cầu có thỏa mãn số lượng tồn kho không
                                        if (igre.DefaultQuantity >
                                            (ingredientSession.Quantity - ingredientSession.SoldQuantity))
                                        {
                                            throw new Exception("Nguyên liệu không đủ số lượng tồn.");
                                        }
                                    }
                                }

                                var ingredientProduct = new IngredientProduct()
                                {
                                    ProductId = product.Id,
                                    IngredientId = igre.Id,
                                    Quantity = igre.DefaultQuantity
                                };
                                ingredientProducts.Add(ingredientProduct);
                                product.Price += igre.Price;
                            }
                        }
                    }

                    products.Add(product);
                    orderProduct = new OrderProduct()
                    {
                        OrderId = order.Id,
                        ProductId = product.Id,
                        Quantity = obj.Quantity,
                        Price = product.Price
                    };
                    orderProducts.Add(orderProduct);
                    order.Amount += (double)product.Price * obj.Quantity;
                    order.Status = (int)OrderStatus.Pending;
                }
                else
                {
                    foreach (var ingre in obj.Ingredients)
                    {
                        var sessions = await _sessionRepository.GetAllAsync();
                        var currentSession = sessions.FirstOrDefault(x =>
                            x.StartTime <= DateTime.Now.TimeOfDay && x.EndTime >= DateTime.Now.TimeOfDay);

                        if (currentSession != null)
                        {
                            var ingredientSession =
                                await _ingredientSessionRepository.GetByIdAsync(ingre.Id, currentSession.Id);
                            if (ingredientSession != null)
                            {
                                // Kiểm tra số lượng yêu cầu có thỏa mãn số lượng tồn kho không
                                if (ingre.Quantity > (ingredientSession.Quantity - ingredientSession.SoldQuantity))
                                {
                                    throw new Exception("Nguyên liệu không đủ số lượng tồn.");
                                }
                            }
                        }

                        var ingredientProduct = new IngredientProduct()
                        {
                            ProductId = product.Id,
                            IngredientId = ingre.Id,
                            Quantity = ingre.Quantity
                        };
                        ingredientProducts.Add(ingredientProduct);
                        product.Price += ingre.Price * ingre.Quantity;
                    }

                    products.Add(product);
                    orderProduct = new OrderProduct()
                    {
                        OrderId = order.Id,
                        ProductId = product.Id,
                        Quantity = obj.Quantity,
                        Price = product.Price
                    };
                    orderProducts.Add(orderProduct);
                    order.Amount += (double)product.Price * obj.Quantity;
                    order.Status = (int)OrderStatus.Pending;
                }
            }

            await _context.ProDucts.AddRangeAsync(products);
            if (ingredientProducts.Any())
                await _context.IngredientProducts.AddRangeAsync(ingredientProducts);

            await _context.Orders.AddRangeAsync(order);
            await _context.OrderProducts.AddRangeAsync(orderProducts);

            var result = await _unitOfWork.SaveChangesAsync();

            OrderResponse response = new OrderResponse()
            {
                OrderId = result ? order.Id.ToString() : null,
                Status = result ? (int)OrderStatus.Pending : (int)OrderStatus.Failed,
                BillCode = order.BillCode
            };

            return new BaseResult<OrderResponse>(response);
        }*/

        // Ngay 26/08/2024
        /*public async Task<BaseResult<OrderResponse>> CreateOrderForCustomerAsync(CreateOrderCommand command)
        {
            if (command.Products == null || !command.Products.Any())
                return new BaseResult<OrderResponse>(new Error(ErrorCode.NotFound));

            // Lấy giờ hiện tại theo UTC+7
            var utcNow = DateTime.UtcNow.AddHours(7);
            var currentTimeOfDay = utcNow.TimeOfDay;

            // Lấy phiên hiện tại dựa trên thời gian UTC+7
            var sessions = await _sessionRepository.GetAllAsync();
            var currentSession = sessions.FirstOrDefault(x =>
                x.StartTime <= currentTimeOfDay && x.EndTime >= currentTimeOfDay);

            // Kiểm tra xem có phiên hiện tại không, nếu không có thì không cho phép đặt hàng
            if (currentSession == null || currentSession.Status != 2)
            {
                return new BaseResult<OrderResponse>(new Error(ErrorCode.SessionNotFound,
                    "Không có phiên làm việc nào đang hoạt động, không thể đặt hàng"));
            }

            List<Product> products = new List<Product>();
            List<IngredientProduct> ingredientProducts = new List<IngredientProduct>();
            List<OrderProduct> orderProducts = new List<OrderProduct>();

            var userId = _authenticatedUserService.UserId;

            var currentUser = await _accountRepository.FindByIdAsync(Guid.Parse(userId));
            Guid? customerId = null;
            if (currentUser is Customer customer)
            {
                customerId = customer.Id; // Assuming the 'Id' in the Customer class is the customerId
            }

            if (customerId == null)
            {
                throw new Exception("User is not a customer");
            }

            // Create new order
            var order = new Order()
            {
                Id = int.Parse(DateTimeOffset.Now.ToString("ffffff")),
                StoreId = command.StoreId,
                BillCode = "Bill-" + EnumExtension.GenerateUniqueId(),
                CustomerId = customerId,
                Platform = 1
            };

            foreach (var obj in command.Products)
            {
                if (obj == null || obj.ProductTemplateId <= 0) continue;
                var productTemplate =
                    await _productTemplateRepository.GetProductTemplateByIdAsync(obj.ProductTemplateId);
                var orderProduct = new OrderProduct();

                var product = new Product()
                {
                    Id = EnumExtension.GenerateUniqueId(),
                    Name = productTemplate.Name,
                    Quantity = obj.Quantity,
                    ProductTemplateId = productTemplate.Id,
                };

                if (!obj.Ingredients.Any())
                {
                    foreach (var step in productTemplate.TemplateSteps)
                    {
                        var temStep = await _templateStepRepository.FindByIdAsync(step.Id);
                        foreach (var ingreStep in temStep.IngredientTypeTemplateSteps)
                        {
                            var ingreType =
                                await _ingredientTypeRepository.GetIngredientTypeByIdAsync(ingreStep.IngredientTypeId);
                            foreach (var igre in ingreType.Ingredients)
                            {
                                if (igre.DefaultQuantity == 0) continue;

                                var ingredientSession =
                                    await _ingredientSessionRepository.GetByIdAsync(igre.Id, currentSession.Id);
                                if (ingredientSession != null)
                                {
                                    // Kiểm tra số lượng yêu cầu có thỏa mãn số lượng tồn kho không
                                    if (igre.DefaultQuantity >
                                        (ingredientSession.Quantity - ingredientSession.SoldQuantity))
                                    {
                                        //throw new Exception("Nguyên liệu không đủ số lượng tồn.");
                                        return new BaseResult<OrderResponse>(new Error(
                                            ErrorCode.NotEnoughQuantityIngredient,
                                            "Nguyên liệu không đủ số lượng tồn."));
                                    }
                                }

                                var ingredientProduct = new IngredientProduct()
                                {
                                    ProductId = product.Id,
                                    IngredientId = igre.Id,
                                    Quantity = igre.DefaultQuantity
                                };
                                ingredientProducts.Add(ingredientProduct);
                                product.Price += igre.Price;
                            }
                        }
                    }

                    products.Add(product);
                    orderProduct = new OrderProduct()
                    {
                        OrderId = order.Id,
                        ProductId = product.Id,
                        Quantity = obj.Quantity,
                        Price = product.Price
                    };
                    orderProducts.Add(orderProduct);
                    order.Amount += (double)product.Price * obj.Quantity;
                    order.Status = (int)OrderStatus.Pending;
                }
                else
                {
                    foreach (var ingre in obj.Ingredients)
                    {
                        var ingredientSession =
                            await _ingredientSessionRepository.GetByIdAsync(ingre.Id, currentSession.Id);
                        if (ingredientSession != null)
                        {
                            // Kiểm tra số lượng yêu cầu có thỏa mãn số lượng tồn kho không
                            if (ingre.Quantity > (ingredientSession.Quantity - ingredientSession.SoldQuantity))
                            {
                                throw new Exception("Nguyên liệu không đủ số lượng tồn.");
                            }
                        }

                        var ingredientProduct = new IngredientProduct()
                        {
                            ProductId = product.Id,
                            IngredientId = ingre.Id,
                            Quantity = ingre.Quantity
                        };
                        ingredientProducts.Add(ingredientProduct);
                        product.Price += ingre.Price * ingre.Quantity;
                    }

                    products.Add(product);
                    orderProduct = new OrderProduct()
                    {
                        OrderId = order.Id,
                        ProductId = product.Id,
                        Quantity = obj.Quantity,
                        Price = product.Price
                    };
                    orderProducts.Add(orderProduct);
                    order.Amount += (double)product.Price * obj.Quantity;
                    order.Status = (int)OrderStatus.Pending;
                }
            }

            await _context.ProDucts.AddRangeAsync(products);
            if (ingredientProducts.Any())
                await _context.IngredientProducts.AddRangeAsync(ingredientProducts);

            await _context.Orders.AddRangeAsync(order);
            await _context.OrderProducts.AddRangeAsync(orderProducts);

            var result = await _unitOfWork.SaveChangesAsync();

            OrderResponse response = new OrderResponse()
            {
                OrderId = result ? order.Id.ToString() : null,
                Status = result ? (int)OrderStatus.Pending : (int)OrderStatus.Failed,
                BillCode = order.BillCode
            };

            return new BaseResult<OrderResponse>(response);
        }*/

        // 22:00-27-08-2024
        /*public async Task<BaseResult<OrderResponse>> CreateOrderForCustomerAsync(CreateOrderCommand command)
        {
            if (command.Products == null || !command.Products.Any())
                return new BaseResult<OrderResponse>(new Error(ErrorCode.NotFound));

            // Lấy giờ hiện tại theo UTC+7
            var utcNow = DateTime.UtcNow.AddHours(7);
            var currentTimeOfDay = utcNow.TimeOfDay;

            // Lấy phiên hiện tại dựa trên thời gian UTC+7
            var sessions = await _sessionRepository.GetAllAsync();
            var currentSession = sessions.FirstOrDefault(x =>
                x.StartTime <= currentTimeOfDay && x.EndTime >= currentTimeOfDay);

            // Kiểm tra xem có phiên hiện tại không và session này phải có status = 2
            if (currentSession == null || currentSession.Status != 1)
            {
                return new BaseResult<OrderResponse>(new Error(ErrorCode.SessionNotFound,
                    "Không có phiên làm việc nào đang hoạt động, không thể đặt hàng"));
            }

            List<Product> products = new List<Product>();
            List<IngredientProduct> ingredientProducts = new List<IngredientProduct>();
            List<OrderProduct> orderProducts = new List<OrderProduct>();

            var userId = _authenticatedUserService.UserId;

            var currentUser = await _accountRepository.FindByIdAsync(Guid.Parse(userId));
            Guid? customerId = null;
            if (currentUser is Customer customer)
            {
                customerId = customer.Id;
            }

            if (customerId == null)
            {
                throw new Exception("User is not a customer");
            }

            // Create new order
            var order = new Order()
            {
                Id = int.Parse(DateTimeOffset.Now.ToString("ffffff")),
                StoreId = command.StoreId,
                BillCode = "Bill-" + EnumExtension.GenerateUniqueId(),
                CustomerId = customerId,
                Platform = 1,
                Status = (int)OrderStatus.Pending
            };

            foreach (var obj in command.Products)
            {
                if (obj == null || obj.ProductTemplateId <= 0) continue;
                var productTemplate =
                    await _productTemplateRepository.GetProductTemplateByIdAsync(obj.ProductTemplateId);
                var orderProduct = new OrderProduct();

                var product = new Product()
                {
                    Id = EnumExtension.GenerateUniqueId(),
                    Name = productTemplate.Name,
                    Quantity = obj.Quantity,
                    ProductTemplateId = productTemplate.Id,
                };

                if (!obj.Ingredients.Any())
                {
                    foreach (var step in productTemplate.TemplateSteps)
                    {
                        var temStep = await _templateStepRepository.FindByIdAsync(step.Id);
                        foreach (var ingreStep in temStep.IngredientTypeTemplateSteps)
                        {
                            var ingreType = await _ingredientTypeRepository.GetIngredientTypeByIdAsync(ingreStep.IngredientTypeId);
                            foreach (var igre in ingreType.Ingredients)
                            {
                                if (igre.DefaultQuantity == 0) continue;

                                var ingredientSession = await _ingredientSessionRepository.GetByIdAsync(igre.Id, currentSession.Id);
                                if (ingredientSession != null)
                                {
                                    // Kiểm tra số lượng tồn kho dựa trên `ingredientSession.Quantity`
                                    if (ingredientSession.Quantity - ingredientSession.SoldQuantity < igre.DefaultQuantity)
                                    {
                                        return new BaseResult<OrderResponse>(new Error(
                                            ErrorCode.NotEnoughQuantityIngredient,
                                            "Nguyên liệu không đủ số lượng tồn."));
                                    }

                                    // Dự trữ trước (Pre-allocate)
                                    ingredientSession.SoldQuantity += igre.DefaultQuantity;
                                    _ingredientSessionRepository.Update(ingredientSession);
                                }

                                var ingredientProduct = new IngredientProduct()
                                {
                                    ProductId = product.Id,
                                    IngredientId = igre.Id,
                                    Quantity = igre.DefaultQuantity
                                };
                                ingredientProducts.Add(ingredientProduct);
                                product.Price += igre.Price;
                            }
                        }
                    }

                    products.Add(product);
                    orderProduct = new OrderProduct()
                    {
                        OrderId = order.Id,
                        ProductId = product.Id,
                        Quantity = obj.Quantity,
                        Price = product.Price
                    };
                    orderProducts.Add(orderProduct);
                    order.Amount += (double)product.Price * obj.Quantity;
                }
                else
                {
                    foreach (var ingre in obj.Ingredients)
                    {
                        var ingredientSession = await _ingredientSessionRepository.GetByIdAsync(ingre.Id, currentSession.Id);
                        if (ingredientSession != null)
                        {
                            // Kiểm tra số lượng tồn kho dựa trên `ingredientSession.Quantity`
                            if (ingredientSession.Quantity - ingredientSession.SoldQuantity < ingre.Quantity)
                            {
                                return new BaseResult<OrderResponse>(new Error(
                                    ErrorCode.NotEnoughQuantityIngredient,
                                    "Nguyên liệu không đủ số lượng tồn."));
                            }

                            // Dự trữ trước (Pre-allocate)
                            ingredientSession.SoldQuantity += ingre.Quantity;
                            _ingredientSessionRepository.Update(ingredientSession);
                        }

                        var ingredientProduct = new IngredientProduct()
                        {
                            ProductId = product.Id,
                            IngredientId = ingre.Id,
                            Quantity = ingre.Quantity
                        };
                        ingredientProducts.Add(ingredientProduct);
                        product.Price += ingre.Price * ingre.Quantity;
                    }

                    products.Add(product);
                    orderProduct = new OrderProduct()
                    {
                        OrderId = order.Id,
                        ProductId = product.Id,
                        Quantity = obj.Quantity,
                        Price = product.Price
                    };
                    orderProducts.Add(orderProduct);
                    order.Amount += (double)product.Price * obj.Quantity;
                }
            }

            await _context.ProDucts.AddRangeAsync(products);
            if (ingredientProducts.Any())
                await _context.IngredientProducts.AddRangeAsync(ingredientProducts);

            await _context.Orders.AddRangeAsync(order);
            await _context.OrderProducts.AddRangeAsync(orderProducts);

            var result = await _unitOfWork.SaveChangesAsync();

            if (result)
            {
                var storeStaff = await _context.Staffs
                    .Include(s=> s.Account)
                    .Include(s=> s.Store)
                    .Where(s => s.StoreId == command.StoreId && s.Account.UserName != s.Store.StoreManager)
                    .ToListAsync();

                foreach (var staff in storeStaff)
                {
                    await _hubContext.Clients.User(staff.EmployeeId.ToString())
                        .SendAsync("ReceiveNotification", $"Có một đơn hàng mới: {order.Id}");
                }
            }

            OrderResponse response = new OrderResponse()
            {
                OrderId = result ? order.Id.ToString() : null,
                Status = result ? (int)OrderStatus.Pending : (int)OrderStatus.Failed,
                BillCode = order.BillCode
            };

            return new BaseResult<OrderResponse>(response);
        }*/

        public async Task<BaseResult<OrderResponse>> CreateOrderForCustomerAsync(CreateOrderCommand command)
        {
            if (command.Products == null || !command.Products.Any())
                return new BaseResult<OrderResponse>(new Error(ErrorCode.NotFound));

            // Lấy giờ hiện tại theo UTC+7
            var utcNow = DateTime.UtcNow.AddHours(7);
            var currentTimeOfDay = utcNow.TimeOfDay;

            // Lấy phiên hiện tại dựa trên thời gian UTC+7
            var sessions = await _sessionRepository.GetAllAsync();
            var currentSession = sessions.FirstOrDefault(x =>
                x.StartTime <= currentTimeOfDay && x.EndTime >= currentTimeOfDay);

            // Kiểm tra xem có phiên hiện tại không và session này phải có status = 2
            if (currentSession == null || currentSession.Status != 1)
            {
                return new BaseResult<OrderResponse>(new Error(ErrorCode.SessionNotFound,
                    "Không có phiên làm việc nào đang hoạt động, không thể đặt hàng"));
            }

            List<Product> products = new List<Product>();
            List<IngredientProduct> ingredientProducts = new List<IngredientProduct>();
            List<OrderProduct> orderProducts = new List<OrderProduct>();

            var userId = _authenticatedUserService.UserId;

            var currentUser = await _accountRepository.FindByIdAsync(Guid.Parse(userId));
            Guid? customerId = null;
            if (currentUser is Customer customer)
            {
                customerId = customer.Id;
            }

            if (customerId == null)
            {
                throw new Exception("User is not a customer");
            }

            // Create new order
            var order = new Order()
            {
                Id = int.Parse(DateTimeOffset.Now.ToString("ffffff")),
                StoreId = command.StoreId,
                BillCode = "Bill-" + EnumExtension.GenerateUniqueId(),
                CustomerId = customerId,
                Platform = 1,
                Status = (int)OrderStatus.Pending
            };

            foreach (var obj in command.Products)
            {
                if (obj == null || obj.ProductTemplateId <= 0) continue;
                var productTemplate =
                    await _productTemplateRepository.GetProductTemplateByIdAsync(obj.ProductTemplateId);
                var orderProduct = new OrderProduct();

                var product = new Product()
                {
                    Id = EnumExtension.GenerateUniqueId(),
                    Name = productTemplate.Name,
                    Quantity = obj.Quantity,
                    ProductTemplateId = productTemplate.Id,
                };

                if (!obj.Ingredients.Any())
                {
                    foreach (var step in productTemplate.TemplateSteps)
                    {
                        var temStep = await _templateStepRepository.FindByIdAsync(step.Id);
                        foreach (var ingreStep in temStep.IngredientTypeTemplateSteps)
                        {
                            var ingreType =
                                await _ingredientTypeRepository.GetIngredientTypeByIdAsync(ingreStep.IngredientTypeId);
                            foreach (var igre in ingreType.Ingredients)
                            {
                                if (igre.DefaultQuantity == 0) continue;

                                var ingredientSession =
                                    await _ingredientSessionRepository.GetByIdAsync(igre.Id, currentSession.Id);
                                if (ingredientSession != null)
                                {
                                    // Kiểm tra số lượng tồn kho dựa trên `ingredientSession.Quantity`
                                    if (ingredientSession.Quantity - ingredientSession.SoldQuantity <
                                        obj.Quantity)
                                    {
                                        return new BaseResult<OrderResponse>(new Error(
                                            ErrorCode.NotEnoughQuantityIngredient,
                                            "Nguyên liệu không đủ số lượng tồn."));
                                    }

                                    // Dự trữ trước (Pre-allocate)
                                    ingredientSession.SoldQuantity += obj.Quantity;
                                    _ingredientSessionRepository.Update(ingredientSession);
                                }

                                var ingredientProduct = new IngredientProduct()
                                {
                                    ProductId = product.Id,
                                    IngredientId = igre.Id,
                                    Quantity = obj.Quantity
                                };
                                ingredientProducts.Add(ingredientProduct);
                                product.Price += igre.Price;
                            }
                        }
                    }

                    products.Add(product);
                    orderProduct = new OrderProduct()
                    {
                        OrderId = order.Id,
                        ProductId = product.Id,
                        Quantity = obj.Quantity,
                        Price = product.Price
                    };
                    orderProducts.Add(orderProduct);
                    order.Amount += (double)product.Price * obj.Quantity;
                }
                else
                {
                    foreach (var ingre in obj.Ingredients)
                    {
                        var ingredientSession =
                            await _ingredientSessionRepository.GetByIdAsync(ingre.Id, currentSession.Id);
                        if (ingredientSession != null)
                        {
                            // Kiểm tra số lượng tồn kho dựa trên `ingredientSession.Quantity`
                            if (ingredientSession.Quantity - ingredientSession.SoldQuantity < ingre.Quantity)
                            {
                                return new BaseResult<OrderResponse>(new Error(
                                    ErrorCode.NotEnoughQuantityIngredient,
                                    "Nguyên liệu không đủ số lượng tồn."));
                            }

                            // Dự trữ trước (Pre-allocate)
                            ingredientSession.SoldQuantity += ingre.Quantity;
                            _ingredientSessionRepository.Update(ingredientSession);
                        }

                        var ingredientProduct = new IngredientProduct()
                        {
                            ProductId = product.Id,
                            IngredientId = ingre.Id,
                            Quantity = ingre.Quantity
                        };
                        ingredientProducts.Add(ingredientProduct);
                        product.Price += ingre.Price * ingre.Quantity;
                    }

                    products.Add(product);
                    orderProduct = new OrderProduct()
                    {
                        OrderId = order.Id,
                        ProductId = product.Id,
                        Quantity = obj.Quantity,
                        Price = product.Price
                    };
                    orderProducts.Add(orderProduct);
                    order.Amount += (double)product.Price * obj.Quantity;
                }
            }

            await _context.ProDucts.AddRangeAsync(products);
            if (ingredientProducts.Any())
                await _context.IngredientProducts.AddRangeAsync(ingredientProducts);

            await _context.Orders.AddRangeAsync(order);
            await _context.OrderProducts.AddRangeAsync(orderProducts);

            var result = await _unitOfWork.SaveChangesAsync();

            /*   if (result)
            {
                var storeStaff = await _context.Staffs
                    .Include(s => s.Account)
                    .Include(s => s.Store)
                    .Where(s => s.StoreId == command.StoreId && s.Account.UserName != s.Store.StoreManager)
                    .ToListAsync();

                foreach (var staff in storeStaff)
                {
                    await _hubContext.Clients.User(staff.EmployeeId.ToString())
                        .SendAsync("ReceiveNotification", $"Có một đơn hàng mới: {order.Id}");
                }
            }*/

            OrderResponse response = new OrderResponse()
            {
                OrderId = result ? order.Id.ToString() : null,
                Status = result ? (int)OrderStatus.Pending : (int)OrderStatus.Failed,
                BillCode = order.BillCode
            };

            return new BaseResult<OrderResponse>(response);
        }
    }
}