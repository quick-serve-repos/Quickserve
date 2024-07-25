using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuickServe.Domain.Ingredients.Entities;

namespace QuickServe.Infrastructure.Persistence.Contexts.Configurations;

public class IngredientConfiguration : IEntityTypeConfiguration<Ingredient>
{
    public void Configure(EntityTypeBuilder<Ingredient> entity)
    {
        entity.ToTable("Ingredient");

        entity.Property(e => e.CreatedBy).HasMaxLength(255);

        entity.Property(e => e.Created).HasColumnType("timestamp with time zone");




        entity.Property(e => e.Description).HasMaxLength(255);

        entity.Property(e => e.ImageUrl)
            .HasMaxLength(255)
            .HasColumnName("Image_Url");

        entity.Property(e => e.IngredientTypeId).HasColumnName("IngredientType_id");

   

        entity.Property(e => e.LastModifiedBy).HasMaxLength(255);

        entity.Property(e => e.LastModified).HasColumnType("timestamp with time zone");

        entity.Property(e => e.Name).HasMaxLength(255);

        entity.Property(e => e.Price).HasColumnType("decimal(8, 2)");

        entity.HasOne(d => d.IngredientType)
            .WithMany(p => p.Ingredients)
            .HasForeignKey(d => d.IngredientTypeId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("ingredient_ingredienttype_id_foreign");
    }
}