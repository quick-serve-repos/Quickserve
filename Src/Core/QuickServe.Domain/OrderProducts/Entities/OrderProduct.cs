using QuickServe.Domain.Common;
using QuickServe.Domain.Orders.Entities;
using QuickServe.Domain.Products.Entities;

namespace QuickServe.Domain.OrderProducts.Entities
{
    public class OrderProduct : AuditableBaseEntity
    {
        public long OrderId { get; set; }
        public long ProductId { get; set; }
        public decimal Price { get; set; }
        public int? Quantity { get; set; }

        public virtual Order Order { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }
}