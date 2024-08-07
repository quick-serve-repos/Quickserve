using System;
using System.Collections.Generic;
using MediatR;
using QuickServe.Application.DTOs.Orders.Response;
using QuickServe.Application.Wrappers;

namespace QuickServe.Application.Features.Orders.Queries.GetCustomerOrderHistory;


    public class GetCustomerOrderHistoryQuery : IRequest<BaseResult<List<OrderHistoryDto>>>
    {
        public Guid CustomerId { get; set; }
        
    }
