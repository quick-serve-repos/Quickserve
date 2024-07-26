using QuickServe.Domain.Accounts.Entities;

namespace QuickServe.Domain.Customers.Entities
{
    public class Customer : Account
    {
        public Customer() { }
        public long Point { get; set; }
    }
}
