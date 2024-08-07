using System.Collections.Generic;
using QuickServe.Domain.Ingredients.Dtos;
using QuickServe.Domain.Products.Entities;

namespace QuickServe.Domain.Products.Dtos
{
    public class ProDuctsDto
    {
        public ProDuctsDto()
        {
        }

        public ProDuctsDto(Product proDucts)
        {
            Id = proDucts.Id;
            Name = proDucts.Name;
            Price = proDucts.Price;
            ProductTemplateId = proDucts.ProductTemplateId;
            Quantity = proDucts.Quantity;
        }

        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }

        public long ProductTemplateId { get; set; }
        public int? Quantity { get; set; }
        public virtual ICollection<IngredientDTO>? Ingredients { get; set; }
    }
}
