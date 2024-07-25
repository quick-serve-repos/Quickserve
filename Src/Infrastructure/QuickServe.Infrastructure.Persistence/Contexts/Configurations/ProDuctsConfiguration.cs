using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuickServe.Domain.Products.Entities;

namespace QuickServe.Infrastructure.Persistence.Contexts.Configurations;


    public class ProDuctsConfiguration : IEntityTypeConfiguration<Product>
    {

        public void Configure(EntityTypeBuilder<Product> entity)
        {
       
        entity.ToTable("Product");

        entity.Property(e => e.CreatedBy).HasMaxLength(255);

        entity.Property(e => e.Created).HasColumnType("timestamp with time zone");

        entity.Property(e => e.LastModifiedBy).HasMaxLength(255);

        entity.Property(e => e.LastModified).HasColumnType("timestamp with time zone");

        entity.Property(e => e.Name).HasMaxLength(255);

        entity.Property(e => e.Price).HasColumnType("decimal(8, 2)");

        entity.Property(e => e.ProductTemplateId).HasColumnName("ProductTemplate_Id"); // Đảm bảo kiểu dữ liệu của ProductTemplateId là long

        entity.HasOne(d => d.ProductTemplate)
            .WithMany(p => p.Products)
            .HasForeignKey(d => d.ProductTemplateId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_ProductTemplate_ID");
    }
    }
