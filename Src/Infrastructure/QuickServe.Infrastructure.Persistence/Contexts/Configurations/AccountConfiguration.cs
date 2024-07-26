
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuickServe.Domain.Accounts.Entities;


namespace QuickServe.Infrastructure.Persistence.Contexts.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.Property(a => a.Name)
            .HasMaxLength(40)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(a => a.Created)
            .HasColumnType("timestamp without time zone")
            .IsRequired();


        // Bạn cũng có thể cấu hình các thuộc tính từ IdentityUser<Guid> nếu cần thiết
        builder.Property(a => a.UserName)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(a => a.Email)
            .HasMaxLength(256)
            .IsRequired(false); // Nếu bạn cho phép email không bắt buộc

    }
}