using QuickServe.Domain.Common;
using QuickServe.Domain.IngredientNutritions.Entities;
using QuickServe.Domain.ProductTemplates.Entities;
using System.Collections.Generic;

namespace QuickServe.Domain.Nutritions.Entities
{
    public class Nutrition : AuditableBaseEntity
    {
        public string? ImageUrl { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Vitamin { get; set; }
        public string? HealthValue { get; set; }
        public int Status { get; set; }

        public virtual ICollection<IngredientNutrition>? IngredientNutritions { get; set; }

        public Nutrition()
        {
            IngredientNutritions = new HashSet<IngredientNutrition>();
        }

        public Nutrition(string name, string image, string description, string vitamin, string healthValue, int status)
        {
            Name = name;
            ImageUrl = image;
            Description = description;
            Vitamin = vitamin;
            HealthValue = healthValue;
            IngredientNutritions = new HashSet<IngredientNutrition>();
        }

        public void Update(string name ,string description, string vitamin, string healthValue)
        {
            Name = name;
            Description = description;
            Vitamin = vitamin;
            HealthValue = healthValue;
        }
        public void Update(int status)
        {
            Status = status;
        }
    }
}