using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuickServe.Domain.ProductTemplates.Entities;

namespace QuickServe.Infrastructure.Persistence.Contexts.Configurations;

public class ProductTemplateConfiguration : IEntityTypeConfiguration<ProductTemplate>
{
    public void Configure(EntityTypeBuilder<ProductTemplate> entity
    )
    {
       
            entity.ToTable("ProductTemplate");

        entity.Property(e => e.CategoryId).HasColumnName("Category_Id");

        entity.Property(e => e.CreatedBy).HasMaxLength(40);

        entity.Property(e => e.Created).HasColumnType("timestamp with time zone");

        entity.Property(e => e.ImageUrl)
            .HasColumnName("Image_url");

        entity.Property(e => e.LastModifiedBy).HasMaxLength(40);

        entity.Property(e => e.LastModified).HasColumnType("timestamp with time zone");

        entity.Property(e => e.Name).HasMaxLength(40);

        entity.Property(e => e.Price).HasColumnType("decimal(8, 2)").IsRequired();

        entity.Property(e => e.Size)
            .HasMaxLength(255)
            .IsUnicode(false);

        entity.HasOne(d => d.Category)
            .WithMany(p => p.ProductTemplates)
            .HasForeignKey(d => d.CategoryId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("producttemplate_categoryid_foreign");
    }
}