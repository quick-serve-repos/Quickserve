using QuickServe.Domain.Categories.Entities;
using QuickServe.Domain.Nutritions.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuickServe.Domain.Nutritions.Dtos
{
    public class NutritionDTO
    {
        public NutritionDTO() { }
        public NutritionDTO(Nutrition nutrition) {
            Id = nutrition.Id;
            Name = nutrition.Name;
            ImageUrl = nutrition.ImageUrl;
            Description = nutrition.Description;
            Vitamin = nutrition.Vitamin;
            HealthValue = nutrition.HealthValue;
            Status = nutrition.Status;
            Created = nutrition.Created;
            CreatedBy = nutrition.CreatedBy;
            LastModified = nutrition.LastModified ?? null;  // Xử lý giá trị NULL
            LastModifiedBy = nutrition.LastModifiedBy ?? null;
        }
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
        public string? Vitamin { get; set; }
        public string? HealthValue { get; set; }
        public int Status { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime Created { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModified { get; set; }
    }
}
