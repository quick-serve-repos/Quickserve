using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuickServe.Domain.Ingredients.Entities;
using QuickServe.Domain.Sessions.Entities;

namespace QuickServe.Infrastructure.Persistence.Contexts.Configurations;

public class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> entity)
    {
        entity.ToTable("Session");

        entity.Property(e => e.EndTime).HasColumnName("End_Time");

        entity.Property(e => e.Name)
            .HasMaxLength(40)
            .IsUnicode(false);
        entity.Property(e => e.Created).HasColumnType("timestamp with time zone");
        entity.Property(e => e.StoreId).HasColumnName("Store_Id");
        entity.Property(e => e.LastModified).HasColumnType("timestamp with time zone");
        entity.Property(e => e.StartTime).HasColumnName("Start_Time").IsRequired();

        entity.HasMany(d => d.IngredientSessions)
            .WithOne(p => p.Session)
            .HasForeignKey(d => d.SessionId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK__Session__Session__6A30C649");
    }
}