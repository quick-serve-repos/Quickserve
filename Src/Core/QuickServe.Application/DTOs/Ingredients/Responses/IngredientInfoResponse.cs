using QuickServe.Domain.Ingredients.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickServe.Application.DTOs.Ingredients.Responses
{
    public class IngredientInfoResponse
    {
        public IngredientInfoResponse() { }
        public IngredientInfoResponse(Ingredient ingredient) {
            Id = ingredient.Id;
            Name = ingredient.Name;
            Price = ingredient.Price;
            Calo  = ingredient.Calo;
            DefaultQuantity = ingredient.DefaultQuantity;
            ImageUrl = ingredient.ImageUrl;
        }
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public int Calo { get; set; }
        public int DefaultQuantity { get; set; }
        public string ImageUrl { get; set; } = null!;
    }
}
