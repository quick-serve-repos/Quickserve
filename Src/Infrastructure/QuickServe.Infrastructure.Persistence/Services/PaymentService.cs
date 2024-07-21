using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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

        public PaymentService(
            IVNPayService vnPayService,
            IOptions<AppSettings> appSettings,
            IHttpContextAccessor httpContextAccessor,
            IOrderRepository orderRepository,
            ApplicationDbContext context, 
            IUnitOfWork unitOfWork)
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
            _context = context;
        }
        public async Task<string> CreateVNPayPaymentUrlAsync(CreatePaymentCommand request, CancellationToken cancellationToken)
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
            var paymentUrl = await _vnPayService.CreatePaymentUrlAsync(_vnPayConfigModel, orderInfo, locale, _vnPaySettings.CallBackUrl);

            return paymentUrl;
        }

        public async Task<PaymentCallBackResult> VNPayCallBackResultAsync(GetVNPayPayment request, CancellationToken cancellationToken)
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
                order.Status = vnPayPayment?.TransactionStatus == "00" ? (int)OrderStatus.Success : (int)OrderStatus.Failed;
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

        
        public async Task<Application.Utils.Payments.Model.PaymentResponse> GetVNPayPaymentAsync(GetVNPayPayment request, CancellationToken cancellationToken)
        {
            IQueryCollection queryList = _httpContextAccessor.HttpContext.Request.Query;
            bool checkSignature = _vnPayService.ValidateSignature(queryList, request.SecureHash, _vnPaySettings.SecretKey);

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
    }
}
