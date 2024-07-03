using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuickServe.Domain.Orders.Entities;

namespace QuickServe.Infrastructure.Persistence.Contexts.Configurations;

public class OrderConfiguration :  IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> entity)
    {
        
        entity.ToTable("Orders");

        entity.Property(e => e.CustomerId).HasColumnName("Customer_id");

        entity.Property(e => e.Amount).HasColumnName("amount"); 

        entity.Property(e => e.Created).HasColumnType("date");

        entity.Property(e => e.StoreId).HasColumnName("Store_id");

        entity.HasMany(d => d.OrderProducts)
            .WithOne(p => p.Order)
            .HasForeignKey(d => d.OrderId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("order_order_product_id_foreign");

        entity.HasMany(d => d.PaymentMethods)
            .WithOne(p => p.Order)
            .HasForeignKey(d => d.RefOrderId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("order_payment_method_id_foreign");

        entity.HasOne(d => d.Store)
            .WithMany(p => p.Orders)
            .HasForeignKey(d => d.StoreId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("order_store_id_foreign");
    }
}