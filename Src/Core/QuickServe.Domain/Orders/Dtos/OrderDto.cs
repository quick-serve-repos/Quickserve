using QuickServe.Domain.Orders.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using QuickServe.Domain.Products.Dtos;

namespace QuickServe.Domain.Orders.Dtos
{
    public class OrderDto
    {
        public OrderDto()
        {
        }
        public OrderDto(Order order)
        {
            Id = order.Id.ToString();
            CustomerId = order.CustomerId;
            TotalPrice = order.Amount;
            Status = order.Status;
            StoreId = order.StoreId;
            BillCode = order.BillCode;
        }
        
        public string Id { get; set; }
        public Guid? CustomerId { get; set; }
        public double TotalPrice { get; set; }
        public int Status { get; set; }
        public long StoreId { get; set; }

        public string BillCode { get; set; }

        public virtual ICollection<ProDuctsDto> Products { get; set; }
    }
}
