using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuickServe.Domain.Stores.Entities;

namespace QuickServe.Infrastructure.Persistence.Contexts.Configurations;

public class StoreConfiguration : IEntityTypeConfiguration<Store>
{
    public void Configure(EntityTypeBuilder<Store> entity)
    {
        entity.ToTable("Store");

        entity.Property(e => e.Address).HasMaxLength(255);

        entity.Property(e => e.CreatedBy).HasMaxLength(255);

        entity.Property(e => e.Created).HasColumnType("timestamp with time zone");

        entity.Property(e => e.LastModifiedBy).HasMaxLength(255);

        entity.Property(e => e.LastModified).HasColumnType("timestamp with time zone");

        entity.Property(e => e.Name).HasMaxLength(255);

    }
}