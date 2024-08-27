using QuickServe.Domain.Accounts.Entities;
using QuickServe.Domain.Orders.Entities;
using System.Collections.Generic;

namespace QuickServe.Domain.Customers.Entities
{
    public class Customer : Account
    {
        public Customer() { }
        public long Point { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
