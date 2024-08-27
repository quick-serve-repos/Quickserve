using System.Collections.Generic;
using QuickServe.Domain.Categories.Entities;
using QuickServe.Domain.Common;
using QuickServe.Domain.Products.Entities;
using QuickServe.Domain.TemplateSteps.Entities;

namespace QuickServe.Domain.ProductTemplates.Entities
{
    public class ProductTemplate : AuditableBaseEntity
    {
        public ProductTemplate()
        {
            Products = new HashSet<Product>();
            TemplateSteps = new HashSet<TemplateStep>();
        }
        public ProductTemplate(long categoryId, string name, int? quantity, string size, string imageUrl, decimal price, string? description, int status, Category category, ICollection<Product> products, ICollection<TemplateStep> templateSteps)
        {
            CategoryId = categoryId;
            Name = name;
            Quantity = quantity;
            Size = size;
            ImageUrl = imageUrl;
            Price = price;
            Description = description;
            Status = status;
            Category = category;
            Products = new HashSet<Product>();
            TemplateSteps = new HashSet<TemplateStep>();
        }
        public void Update(string name, string size, string description, long categoryId)
        {
            Name = name;
            Size = size;
            Description = description;
            CategoryId = categoryId;
        }
        public void Update(int status)
        {
            Status = status;

        }
        public long CategoryId { get;  set; }
        public string Name { get;  set; } = null!;
        public int? Quantity { get; set; }
        public string Size { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public int Status { get; set; }

        public virtual Category Category { get; set; } = null!;
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<TemplateStep> TemplateSteps { get; set; }
    }
}