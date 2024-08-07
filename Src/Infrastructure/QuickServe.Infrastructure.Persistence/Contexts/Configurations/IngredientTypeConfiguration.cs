using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuickServe.Domain.IngredientTypes.Entities;

namespace QuickServe.Infrastructure.Persistence.Contexts.Configurations;

public class IngredientTypeConfiguration  : IEntityTypeConfiguration<IngredientType>
{
    public void Configure(EntityTypeBuilder<IngredientType> entity)
    {
        entity.ToTable("IngredientType");

        entity.Property(e => e.CreatedBy).HasMaxLength(40);

        entity.Property(e => e.Created).HasColumnType("timestamp with time zone");


        entity.Property(e => e.LastModifiedBy).HasMaxLength(40);

        entity.Property(e => e.LastModified).HasColumnType("timestamp with time zone");
        entity.Property(e => e.Name).HasMaxLength(40);
    }
}