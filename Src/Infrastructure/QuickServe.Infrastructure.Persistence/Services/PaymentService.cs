using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Net.payOS.Types;
using QuickServe.Application.DTOs.Payment;
using QuickServe.Application.Features.Payments.Commands.CreatePayment;
using QuickServe.Application.Interfaces;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Utils.Enums;
using QuickServe.Application.Utils.Payments;
using QuickServe.Application.Utils.Payments.Model;
using QuickServe.Domain.Payments.Entities;
using QuickServe.Domain.Settings;
using QuickServe.Infrastructure.Persistence.Contexts;
using QuickServe.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.EntityFrameworkCore;

namespace QuickServe.Infrastructure.Persistence.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IVNPayService _vnPayService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOrderRepository _orderRepository;
        private readonly AppSettings _appSettings;
        private readonly VNPaySettings _vnPaySettings;
        private readonly VNPayConfigModel _vnPayConfigModel;
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPayOSService _payOSService;
        private readonly IIngredientSessionRepository _ingredientSessionRepository;
        private readonly ISessionRepository _sessionRepository;

        public PaymentService(
            IVNPayService vnPayService,
            IOptions<AppSettings> appSettings,
            IHttpContextAccessor httpContextAccessor,
            IOrderRepository orderRepository,
            ApplicationDbContext context,
            IUnitOfWork unitOfWork,
            IPayOSService payOSService,
            IIngredientSessionRepository ingredientSessionRepository, ISessionRepository sessionRepository
        )
        {
            _vnPayService = vnPayService;
            _httpContextAccessor = httpContextAccessor;
            _orderRepository = orderRepository;
            _appSettings = appSettings.Value;
            _vnPaySettings = _appSettings.PaymentSettings.VNPaySettings;
            _vnPayConfigModel = new VNPayConfigModel
            {
                TerminalId = _vnPaySettings.TerminalId,
                SecretKey = _vnPaySettings.SecretKey
            };
            _unitOfWork = unitOfWork;
            _payOSService = payOSService;
            _ingredientSessionRepository = ingredientSessionRepository;
            _sessionRepository = sessionRepository;
            _context = context;
        }

        public async Task<string> CreateVNPayPaymentUrlAsync(CreatePaymentCommand request,
            CancellationToken cancellationToken)
        {
            var locale = "vn";
            var title = $"Deposit for user has phone number {request.OrderInfo}, amount {request.TotalPrice}";

            var orderInfo = new VNPayOrderInfoModel()
            {
                OrderId = request.OrderId,
                Title = title,
                Amount = request.TotalPrice.Value,
                CreatedDate = DateTime.Now,
                BankCode = "VNBANK",
                CurrencyCode = "VND"
            };

            // Call the VNPay's service.
            var paymentUrl =
                await _vnPayService.CreatePaymentUrlAsync(_vnPayConfigModel, orderInfo, locale,
                    _vnPaySettings.CallBackUrl);

            return paymentUrl;
        }

        public async Task<PaymentCallBackResult> VNPayCallBackResultAsync(GetVNPayPayment request,
            CancellationToken cancellationToken)
        {
            var vnPayPayment = await GetVNPayPaymentAsync(request, cancellationToken);
            var payment = new Payment()
            {
                Id = EnumExtension.GenerateUniqueId(),
                Name = vnPayPayment.TransactionNo,
                RefOrderId = long.Parse(vnPayPayment.OrderId),
                PaymentType = 2
            };

            var order = await _orderRepository.GetByIdAsync(payment.RefOrderId);
            if (order == null)
                return null;

            else
            {
                order.Status = vnPayPayment?.TransactionStatus == "00"
                    ? (int)OrderStatus.Paided
                    : (int)OrderStatus.Failed;
            }

            await _context.Payments.AddRangeAsync(payment);

            await _unitOfWork.SaveChangesAsync();

            var result = new PaymentCallBackResult()
            {
                Id = payment.Id.ToString(),
                Name = vnPayPayment?.TransactionNo,
                RefOrderId = order.Id.ToString(),
                Status = order.Status,
                PaymentType = 2
            };

            return result;
        }


        public async Task<Application.Utils.Payments.Model.PaymentResponse> GetVNPayPaymentAsync(
            GetVNPayPayment request, CancellationToken cancellationToken)
        {
            IQueryCollection queryList = _httpContextAccessor.HttpContext.Request.Query;
            bool checkSignature =
                _vnPayService.ValidateSignature(queryList, request.SecureHash, _vnPaySettings.SecretKey);

            if (!checkSignature) return null;

            string orderInfo = $"Transaction data with receipt no {request.TxnRef}";

            // Create a new query to check the real order.
            var resultFromVnPay = await _vnPayService.QueryAsync(
                _vnPayConfigModel,
                request.TxnRef,
                orderInfo,
                request.TransDate,
                DateTime.Now,
                queryList
            );

            return resultFromVnPay;
        }

        public async Task<PaymentCallBackResult> SubmitOrder(long orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                return null;

            var payment = new Payment()
            {
                Id = EnumExtension.GenerateUniqueId(),
                Name = "COD",
                RefOrderId = orderId,
                PaymentType = 1
            };
            order.Status = (int)OrderStatus.Paided;

            await _context.Payments.AddRangeAsync(payment);
            await _unitOfWork.SaveChangesAsync();

            var result = new PaymentCallBackResult()
            {
                Id = payment.Id.ToString(),
                Name = "COD",
                RefOrderId = order.Id.ToString(),
                Status = order.Status,
                PaymentType = 1
            };

            return result;
        }

        #region PayOS

        public async Task<string> CreatePayOSPaymentAsync(CreatePaymentRequest request,
            CancellationToken cancellationToken)
        {
            var paymentRequest = new CreatePaymentRequest
            {
                Amount = request.Amount,
                OrderCode = request.OrderCode,
                Description = "Nap tien vao vi",
                CancelUrl = request.ReturnUrl,
                ReturnUrl = request.ReturnUrl,
                Items = new List<ItemData> { new ItemData("Thanh toán hoá đơn", 1, request.Amount) }
            };

            var reponse = await _payOSService.CreatePayment(paymentRequest);
            return reponse.checkoutUrl;
        }

        /*public async Task<PaymentCallBackResult> PayOSCallBackResultAsync(GetPayOSResponse request, CancellationToken cancellationToken)
        {
            var payment = new Payment()
            {
                Id = EnumExtension.GenerateUniqueId(),
                Name = request.OrderCode,
                RefOrderId = long.Parse(request.OrderCode),
                PaymentType = 2
            };

            var order = await _orderRepository.GetByIdAsync(payment.RefOrderId);
            if (order == null)
                return null;

            else
            {
                order.Status = request.Status == "PAID" ? (int)OrderStatus.Success : (int)OrderStatus.Failed;
            }

            await _context.Payments.AddRangeAsync(payment);

            await _unitOfWork.SaveChangesAsync();

            var result = new PaymentCallBackResult()
            {
                Id = payment.Id.ToString(),
                Name = request.Code,
                RefOrderId = order.Id.ToString(),
                Status = order.Status,
                PaymentType = 2
            };

            return result;
        }*/
        public async Task<PaymentCallBackResult> PayOSCallBackResultAsync(GetPayOSResponse request,
            CancellationToken cancellationToken)
        {
            var orderId = long.Parse(request.OrderCode);

            // Tìm kiếm đơn hàng dựa trên RefOrderId (OrderId)
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                return null;

            // Kiểm tra xem đã có Payment nào cho RefOrderId này chưa
            var existingPayment = await _context.Payments.FirstOrDefaultAsync(p => p.RefOrderId == order.Id);
            if (existingPayment != null)
            {
                // Nếu Payment đã tồn tại, trả về thông tin thanh toán hiện có
                return new PaymentCallBackResult()
                {
                    Id = existingPayment.Id.ToString(),
                    Name = existingPayment.Name,
                    RefOrderId = existingPayment.RefOrderId.ToString(),
                    Status = order.Status,
                    PaymentType = existingPayment.PaymentType
                };
            }

            // Nếu chưa có Payment nào, tạo mới
            var payment = new Payment()
            {
                Id = EnumExtension.GenerateUniqueId(),
                Name = request.OrderCode,
                RefOrderId = order.Id,
                PaymentType = 2
            };

            // Nếu thanh toán thành công, cập nhật trạng thái thành Paided và tăng soldQuantity
            if (request.Status == "PAID")
            {
                order.Status = (int)OrderStatus.Paided;

                // Lấy giờ hiện tại theo UTC+7
                var utcNow = DateTime.UtcNow.AddHours(7);
                var currentTimeOfDay = utcNow.TimeOfDay;

                // Lấy phiên hiện tại dựa trên thời gian UTC+7
                var sessions = await _sessionRepository.GetAllAsync();
                var currentSession = sessions.FirstOrDefault(x =>
                    x.StartTime <= currentTimeOfDay && x.EndTime >= currentTimeOfDay);


                if (currentSession != null)
                {
                    // Cập nhật số lượng soldQuantity trong IngredientSession mà không kiểm tra tồn kho
                    foreach (var orderProduct in order.OrderProducts)
                    {
                        foreach (var ingredientProduct in orderProduct.Product.IngredientProducts)
                        {
                            // Sử dụng ingredientId và sessionId để tìm ingredientSession
                            var ingredientSession =
                                await _ingredientSessionRepository.GetByIdAsync(ingredientProduct.IngredientId,
                                    currentSession.Id);
                            if (ingredientSession != null)
                            {
                                // Cập nhật soldQuantity trong IngredientSession
                                ingredientSession.SoldQuantity += ingredientProduct.Quantity;
                                _ingredientSessionRepository.Update(ingredientSession);
                            }
                        }
                    }
                }
            }
            else
            {
                order.Status = (int)OrderStatus.Failed;
            }

            // Cập nhật lại order vào repository
            _orderRepository.Update(order);

            // Lưu Payment và đơn hàng
            await _context.Payments.AddAsync(payment);
            await _unitOfWork.SaveChangesAsync();

            var result = new PaymentCallBackResult()
            {
                Id = payment.Id.ToString(),
                Name = payment.Name,
                RefOrderId = order.Id.ToString(),
                Status = order.Status,
                PaymentType = payment.PaymentType
            };

            return result;
        }

        #endregion


        public async Task<PaymentCallBackResult> PayOSCallBackResultForCustomerAsync(GetPayOSResponse request,
            Guid customerId, CancellationToken cancellationToken)
        {
            var payment = new Payment()
            {
                Id = EnumExtension.GenerateUniqueId(),
                Name = request.OrderCode,
                RefOrderId = long.Parse(request.OrderCode),
                PaymentType = 2
            };

            var order = await _orderRepository.GetByIdAsync(payment.RefOrderId);
            if (order == null)
                return null;

            // Assign the customerId to the order
            order.CustomerId = customerId;

            if (request.Status == "PAID")
            {
                order.Status = (int)OrderStatus.Paided;
            }
            else
            {
                order.Status = (int)OrderStatus.Failed;
            }

            await _context.Payments.AddRangeAsync(payment);
            await _unitOfWork.SaveChangesAsync();

            var result = new PaymentCallBackResult()
            {
                Id = payment.Id.ToString(),
                Name = request.Code,
                RefOrderId = order.Id.ToString(),
                Status = order.Status,
                PaymentType = 2
            };

            return result;
        }
    }
}