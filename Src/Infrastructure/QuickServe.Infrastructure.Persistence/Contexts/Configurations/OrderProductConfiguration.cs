using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuickServe.Domain.OrderProducts.Entities;

namespace QuickServe.Infrastructure.Persistence.Contexts.Configurations;

public class OrderProductConfiguration  :  IEntityTypeConfiguration<OrderProduct>
{
    public void Configure(EntityTypeBuilder<OrderProduct> entity)
    {
      

        entity.ToTable("OrderProduct");
        entity.HasKey(e => e.Id);

        entity.Property(e => e.OrderId).HasColumnName("OrderID");

        entity.Property(e => e.ProductId).HasColumnName("ProductID");
        entity.Property(e => e.Price).HasColumnType("decimal(8, 2)");
        entity.HasOne(d => d.Order)
            .WithMany(p => p.OrderProducts)
            .HasForeignKey(d => d.OrderId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK__OrderProd__Order__2A164134");

        entity.HasOne(d => d.Product)
            .WithMany(p => p.OrderProducts)
            .HasForeignKey(d => d.ProductId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK__OrderProd__Produ__2B0A656D");
    }
}