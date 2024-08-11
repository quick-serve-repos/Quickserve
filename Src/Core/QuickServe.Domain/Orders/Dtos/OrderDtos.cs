using System;
using QuickServe.Domain.Orders.Entities;

namespace QuickServe.Domain.Orders.Dtos
{
    public class OrderDtos
    {
        public OrderDtos()
        {
        }

        public OrderDtos(Order order)
        {
            Id = order.Id.ToString();
            Amount = order.Amount;
            Status = order.Status;
            StoreId = order.StoreId;
            Created = order.Created;
            Platform = order.Platform;
        }

        public string Id { get; set; }
        public double Amount { get; set; }
        public int Status { get; set; }
        public long StoreId { get; set; }
        public DateTime Created { get; set; }
        public int Platform { get; set; }
    }
}