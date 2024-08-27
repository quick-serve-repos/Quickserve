using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuickServe.Application.DTOs.Payment;
using QuickServe.Application.Features.Payments.Commands.CreatePayment;
using QuickServe.Application.Utils.Payments.Model;
using QuickServe.Application.Wrappers;
using QuickServe.Domain.Settings;
using System.Threading;
using System;
using System.Threading.Tasks;
using QuickServe.Application.Interfaces;
using System.Linq;
using Azure.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using QuickServe.Application.Features.Payments.Queries.GetPagedListPayment;
using QuickServe.Application.Features.Payments.Queries.GetPaymentById;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Domain.Customers.Entities;
using QuickServe.Domain.Payments.Dtos;

namespace QuickServe.WebApi.Controllers.v1
{
    [ApiVersion("1")]
    public class PaymentsController : BaseApiController
    {
        private readonly IPaymentService _paymentService;
        private readonly IAccountRepository _accountRepository;

        public PaymentsController(IPaymentService paymentService,   IAccountRepository accountRepository)
        {
            _paymentService = paymentService;
            _accountRepository = accountRepository;
        }

        [HttpPost("CreateVNPayPayment")]
        public async Task<BaseResult<Application.DTOs.Payment.PaymentResponse>> CreateVNPayPaymentAsync([FromBody] CreatePaymentCommand command)
        {
            return await Mediator.Send(command);
        }

        [HttpGet("call-back")]
        public async Task<BaseResult<PaymentCallBackResult>> CallBack()
        {
            var payment = new GetVNPayPayment()
            {
                SecureHash = Request.Query["vnp_SecureHash"],
                TransDate = Request.Query["vnp_CreateDate"],
                TxnRef = Request.Query["vnp_TxnRef"]
            };

            var result = await _paymentService.VNPayCallBackResultAsync(payment, cancellationToken: default);

            return new BaseResult<PaymentCallBackResult>(result);
        }
        
        [HttpPost("submit-order/{orderId}")]
        public async Task<BaseResult<PaymentCallBackResult>> SubmitOrder(long orderId)
        {
            var result = await _paymentService.SubmitOrder(orderId);
            return new BaseResult<PaymentCallBackResult>(result);
        }

        [HttpPost("CreatePayOS")]
        public async Task<BaseResult<Application.DTOs.Payment.PaymentResponse>> CreatePayOSAsync([FromBody] CreatePayOSCommand command)
        {
            return await Mediator.Send(command);
        }

        [HttpGet("payos-call-back")]
        public async Task<BaseResult<PaymentCallBackResult>> PayOsCallBack()
        {
            var payment = new GetPayOSResponse()
            {
                Code = Request.Query["code"],
                Cancel = Request.Query["cancel"],
                Status = Request.Query["status"],
                OrderCode = Request.Query["orderCode"],
            };

            var result = await _paymentService.PayOSCallBackResultAsync(payment, cancellationToken: default);

            return new BaseResult<PaymentCallBackResult>(result);
        }

        [HttpGet("payos-call-back-for-customer")]
        public async Task<BaseResult<PaymentCallBackResult>> PayOsCallBackForCustomer()
        {
            // Retrieve the current user ID using the GetCurrentUserId method
            var userId = GetCurrentUserId();

            // Find the current user by userId
            var currentUser = await _accountRepository.FindByIdAsync(userId);

            if (!(currentUser is Customer customer))
            {
                throw new Exception("Current user is not a customer.");
            }

            var customerId = customer.Id;

            // Retrieve payment information from the request query
            var payment = new GetPayOSResponse()
            {
                Code = Request.Query["code"],
                Cancel = Request.Query["cancel"],
                Status = Request.Query["status"],
                OrderCode = Request.Query["orderCode"],
            };

            // Call the method that processes the PayOS callback, passing the customerId
            var result = await _paymentService.PayOSCallBackResultForCustomerAsync(payment, customerId, cancellationToken: default);

            return new BaseResult<PaymentCallBackResult>(result);
        }


        [HttpGet()]
        public async Task<PagedResponse<PaymentDto>> GetPayments([FromQuery] GetPagedListPaymentQuery command)
        {
            return await Mediator.Send(command);
        } 
        
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Store_Manager")]

        [HttpGet("by-store")]
        public async Task<PagedResponse<PaymentDto>> GetPaymentsByStoreId([FromQuery] GetPaymentByStoreIdQuery command)
        {
            return await Mediator.Send(command);
        }
        
        private Guid GetCurrentUserId()
        {
            // Lấy thông tin của người dùng từ ClaimsPrincipal
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return userId;
            }
            throw new Exception("User ID not found in token");
        }
    }
}