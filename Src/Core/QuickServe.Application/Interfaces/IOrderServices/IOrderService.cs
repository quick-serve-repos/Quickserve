using QuickServe.Application.DTOs.Orders.Response;
using QuickServe.Application.Features.Orders.Commands.CreateOrder;
using QuickServe.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickServe.Application.Interfaces.IOrderServices
{
    public interface IOrderService
    {
        Task<BaseResult<OrderResponse>> CreateOrderAsync(CreateOrderCommand command);
    }
}
