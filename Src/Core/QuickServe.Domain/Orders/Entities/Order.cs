using System;
using System.Collections.Generic;
using QuickServe.Domain.Accounts.Entities;
using QuickServe.Domain.Common;

using QuickServe.Domain.OrderProducts.Entities;
using QuickServe.Domain.Payments.Entities;
using QuickServe.Domain.Sessions.Entities;
using QuickServe.Domain.Stores.Entities;

namespace QuickServe.Domain.Orders.Entities
{
    public class Order : AuditableBaseEntity
    {
        public Order()
        {
            OrderProducts = new HashSet<OrderProduct>();
            PaymentMethods = new HashSet<Payment>();
        }

        public Guid? CustomerId { get; set; }
        public double Amount { get; set; }
        public int Number {  get; set; }
        public string BillCode { get; set; }
        public int Status { get; set; }
        public long StoreId { get; set; }

        public virtual Account? Customer { get; set; } 
        public virtual ICollection<Payment> PaymentMethods { get; set; }
        public virtual Store Store { get; set; } = null!;
        public virtual ICollection<OrderProduct> OrderProducts { get; set; }
    }

   
}