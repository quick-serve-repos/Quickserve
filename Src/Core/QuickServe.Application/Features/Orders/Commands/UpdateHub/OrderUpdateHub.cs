using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
namespace QuickServe.Application.Features.Orders.Commands.UpdateHub;

public class OrderUpdateHub : Hub
{
    public async Task SendOrderUpdateReminder(long orderId, string message)
    {
        await Clients.All.SendAsync("OrderUpdateReminder", new { OrderId = orderId, Message = message });
    }
}
