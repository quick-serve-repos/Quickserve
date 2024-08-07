using System.Collections.Generic;
using QuickServe.Domain.Common;
using QuickServe.Domain.IngredientProducts.Entities;
using QuickServe.Domain.OrderProducts.Entities;
using QuickServe.Domain.ProductTemplates.Entities;

namespace QuickServe.Domain.Products.Entities
{
    public class Product : AuditableBaseEntity
    {
        public Product()
        {
            IngredientProducts = new HashSet<IngredientProduct>();
            OrderProducts = new HashSet<OrderProduct>();
        }

  
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }

        public long ProductTemplateId { get; set; }
        public int? Quantity { get; set; }
        
        public bool IsCustomer { get; set; }

        public virtual ProductTemplate ProductTemplate { get; set; } = null!;
        public virtual ICollection<IngredientProduct> IngredientProducts { get; set; }
        public virtual ICollection<OrderProduct> OrderProducts { get; set; }
    }
}