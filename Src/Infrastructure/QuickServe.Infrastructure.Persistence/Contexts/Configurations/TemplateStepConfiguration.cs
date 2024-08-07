using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuickServe.Domain.TemplateSteps.Entities;

namespace QuickServe.Infrastructure.Persistence.Contexts.Configurations;

public class TemplateStepConfiguration  : IEntityTypeConfiguration<TemplateStep>
{
    public void Configure(EntityTypeBuilder<TemplateStep> entity)
    {
        entity.ToTable("TemplateStep");

        entity.HasKey(e => e.Id);

        entity.Property(e => e.CreatedBy).HasMaxLength(255);

        entity.Property(e => e.Created).HasColumnType("timestamp with time zone");

        entity.Property(e => e.LastModifiedBy).HasMaxLength(255);

        entity.Property(e => e.LastModified).HasColumnType("timestamp with time zone");

        entity.Property(e => e.Name).HasMaxLength(255);

        entity.Property(e => e.ProductTemplateId).HasColumnName("ProductTemplate_Id");

        entity.HasOne(d => d.ProductTemplate)
            .WithMany(p => p.TemplateSteps)
            .HasForeignKey(d => d.ProductTemplateId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("templatestep_proucttemplate_id_foreign");
    }
}