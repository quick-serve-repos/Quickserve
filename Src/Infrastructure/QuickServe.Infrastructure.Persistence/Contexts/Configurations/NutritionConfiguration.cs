using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuickServe.Domain.Nutritions.Entities;

namespace QuickServe.Infrastructure.Persistence.Contexts.Configurations;

public class NutritionConfiguration  : IEntityTypeConfiguration<Nutrition>

{
    public void Configure(EntityTypeBuilder<Nutrition> entity)
    {
        entity.ToTable("Nutrition");

        entity.Property(e => e.CreatedBy).HasMaxLength(40);

        entity.Property(e => e.Created).HasColumnType("timestamp with time zone");



        entity.Property(e => e.HealthValue).HasMaxLength(100);

        entity.Property(e => e.ImageUrl).HasMaxLength(255);
        entity.Property(e => e.Description).HasMaxLength(100);
    

        entity.Property(e => e.LastModifiedBy).HasMaxLength(40);

        entity.Property(e => e.LastModified).HasColumnType("timestamp with time zone");

        entity.Property(e => e.Name).HasMaxLength(40);

        entity.Property(e => e.Vitamin).HasMaxLength(40);

        entity.HasMany(d => d.IngredientNutritions)
            .WithOne(p => p.Nutrition)
            .HasForeignKey(d => d.NutritionId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK__Ingredien__Nutri__6A30C649");
    }
}