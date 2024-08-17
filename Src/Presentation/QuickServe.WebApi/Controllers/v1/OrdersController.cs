using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickServe.Application.DTOs.Nutritions.Request;
using QuickServe.Application.DTOs.Orders.Response;
using QuickServe.Application.Features.Orders.Commands.CreateOrder;
using QuickServe.Application.Features.Orders.Commands.UpdateOrder;
using QuickServe.Application.Features.Orders.Queries.GetBestSellingProductTemplates;
using QuickServe.Application.Features.Orders.Queries.GetBestStoreSellingProductTemplates;
using QuickServe.Application.Features.Orders.Queries.GetOrderById;
using QuickServe.Application.Features.Orders.Queries.GetPagedListOrder;
using QuickServe.Application.Features.Orders.Queries.GetPagedListOrderToWaitingScreen;
using QuickServe.Application.Features.Orders.Queries.GetRevenueReport;
using QuickServe.Application.Features.Orders.Queries.GetStoreRevenueReport;
using QuickServe.Application.Features.ProductTemplates.Queries.GetPagedListProductTemplate;
using QuickServe.Application.Features.ProductTemplates.Queries.GetProductTemplateById;
using QuickServe.Application.Features.Store.Commands.CreateStore;
using QuickServe.Application.Interfaces.IOrderServices;
using QuickServe.Application.Wrappers;
using QuickServe.Domain.Orders.Dtos;
using QuickServe.Domain.ProductTemplates.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;
using QuickServe.Application.Features.Orders.Queries.GetCustomerOrderHistory;
using QuickServe.Application.Features.Orders.Queries.GetOrders;
using QuickServe.Application.Features.Orders.Queries.GetOrdersForStaff;
using QuickServe.Application.Features.Payments.Queries.GetPaymentById;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace QuickServe.WebApi.Controllers.v1
{
    [ApiVersion("1")]
    public class OrdersController : BaseApiController
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet()]
        public async Task<PagedResponse<OrderDto>> GetOrders([FromQuery] GetPagedListOrderQuery command)
        {
            return await Mediator.Send(command);
        }

        [HttpGet("{id}")]
        public async Task<BaseResult<OrderDto>> GetOrderById(long id)
        {
            return await Mediator.Send(new GetOrderByIdQuery { Id = id });
        }

        [HttpPost("CreateOrder")]
        public async Task<BaseResult<OrderResponse>> CreateOrder(CreateOrderCommand command)
        {
            return await _orderService.CreateOrderAsync(command);
        }
        
        [HttpPost("CreateOrderForCustomer")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Customer")]
        public async Task<BaseResult<OrderResponse>> CreateOrderForCustomer(CreateOrderCommand command)
        {
            return await _orderService.CreateOrderForCustomerAsync(command);
        }


        [HttpPut("UpdateOrderStatus")]
        public async Task<BaseResult<OrderResponse>> UpdateOrderStatus(UpdateOrderCommand command)
        {
            return await Mediator.Send(command);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Brand_Manager, Admin")]
        [HttpGet("RevenueReport")]
        public async Task<BaseResult<RevenueReportDto>> GetRevenueReport([FromQuery] GetRevenueReportQuery query)
        {
            return await Mediator.Send(query);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Store_Manager")]
        [HttpGet("RevenueReport/Store")]
        public async Task<BaseResult<RevenueReportDto>> GetStoreRevenueReport([FromQuery] GetStoreRevenueReportQuery query)
        {
            return await Mediator.Send(query);
        }

        [HttpGet("BestSellingProductTemplates")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Brand_Manager, Admin")]
        public async Task<ActionResult<BaseResult<List<BestSellingReportDto>>>> GetBestSellingProductTemplates([FromQuery] GetBestSellingProductTemplatesQuery query)
        {
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("BestSellingProductTemplates/Store")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Store_Manager")]
        public async Task<ActionResult<BaseResult<List<BestSellingReportDto>>>> GetBestStoreSellingProductTemplates([FromQuery] GetBestStoreSellingProductTemplatesQuery query)
        {
            var result = await Mediator.Send(query);
            return Ok(result);
        }
        [HttpGet("Store/OrderStatus")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Employee, Store_Manager")]
        public async Task<ActionResult<BaseResult<List<OderStatusResponse>>>> GetOrdersToWaitingScreen([FromQuery] GetPagedListOrderToWaitingScreenQuery query)
        {

            var result = await Mediator.Send(query);
            return Ok(result);
        }
        [HttpGet("Staff/OrderStatus")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Staff, Store_Manager")]
        public async Task<ActionResult<BaseResult<List<OderStatusResponse>>>> GetOrdersForStaff([FromQuery] GetOrdersForStaffQuery query)
        {
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        
        
       /* [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("CustomerOrderHistory")]
        public async Task<BaseResult<List<OrderHistoryDto>>> GetCustomerOrderHistory()
        {
            var customerId = GetCurrentUserId(); // Method to get the current logged-in user's ID
            var query = new GetCustomerOrderHistoryQuery { CustomerId = customerId };
            return await Mediator.Send(query);
        }*/
       [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
       [HttpGet("CustomerOrderHistory")]
       public async Task<PagedResponse<OrderHistoryDto>> GetCustomerOrderHistory([FromQuery] GetCustomerOrderHistoryQuery query)
       {
           query.CustomerId = GetCurrentUserId(); // Method to get the current logged-in user's ID
           return await Mediator.Send(query);
       }
        
        [HttpGet("OrderList")]
        public async Task<PagedResponse<OrderDtos>> GetOrdersList([FromQuery] GetPagedOrderListQuery command)
        {
            return await Mediator.Send(command);
        }
        
        
        [HttpGet("OrderListByStoreId")]
        public async Task<PagedResponse<OrderDtos>> GetOrdersListByStoreId([FromQuery] GetOrderByStoreIdQuery command)
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