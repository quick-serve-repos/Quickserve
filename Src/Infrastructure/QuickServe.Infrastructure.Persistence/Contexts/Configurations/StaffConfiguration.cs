using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuickServe.Domain.Staffs.Entities;

namespace QuickServe.Infrastructure.Persistence.Contexts.Configurations
{
    public class StaffConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.ToTable("Employee");

            builder.HasKey(s => s.EmployeeId);

            builder.HasOne(s => s.Store)
                .WithMany(s => s.Staffs)
                .HasForeignKey(s => s.StoreId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(s => s.Account)
                .WithOne(a => a.Staff)
                .HasForeignKey<Employee>(s => s.EmployeeId)
                .HasConstraintName("FK_Staff_Account");
        }
    }
}
