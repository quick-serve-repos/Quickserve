using Microsoft.EntityFrameworkCore;
using QuickServe.Application.Interfaces;
using QuickServe.Domain.Common;
using QuickServe.Domain.Products.Entities;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using QuickServe.Domain.Accounts.Entities;
using QuickServe.Domain.Categories.Entities;

using QuickServe.Domain.IngredientProducts.Entities;
using QuickServe.Domain.Ingredients.Entities;
using QuickServe.Domain.IngredientTypes.Entities;
using QuickServe.Domain.IngredientTypeTemplateSteps.Entities;
using QuickServe.Domain.Nutritions.Entities;
using QuickServe.Domain.OrderProducts.Entities;
using QuickServe.Domain.Orders.Entities;
using QuickServe.Domain.Payments.Entities;
using QuickServe.Domain.ProductTemplates.Entities;
using QuickServe.Domain.Sessions.Entities;
using QuickServe.Domain.Stores.Entities;
using QuickServe.Domain.TemplateSteps.Entities;
using QuickServe.Domain.IngredientSessions.Entities;
using QuickServe.Domain.IngredientNutritions.Entities;
using QuickServe.Domain.Staffs.Entities;
using QuickServe.Domain.Customers.Entities;



namespace QuickServe.Infrastructure.Persistence.Contexts
{
    public class ApplicationDbContext :  DbContext

    {
    private readonly IAuthenticatedUserService authenticatedUser;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
        IAuthenticatedUserService authenticatedUser) : base(options)
    {
        this.authenticatedUser = authenticatedUser;
    }

    public virtual DbSet<Product> ProDucts { get; set; }  
 
    public virtual DbSet<Category> Categories { get; set; }
 
    public virtual DbSet<Ingredient> Ingredients { get; set; } 
    public virtual DbSet<IngredientProduct> IngredientProducts { get; set; }
    public virtual DbSet<IngredientSession> IngredientSessions { get; set; }
    public virtual DbSet<IngredientType> IngredientTypes { get; set; } 
    public virtual DbSet<IngredientTypeTemplateStep> IngredientTypeTemplateSteps { get; set; }
    public virtual DbSet<IngredientNutrition> IngredientNutritions { get; set; }
    public virtual DbSet<Nutrition> Nutritions { get; set; } 
    public virtual DbSet<Order> Orders { get; set; }
    public virtual DbSet<OrderProduct> OrderProducts { get; set; } 
    public virtual DbSet<Payment> Payments { get; set; } 

    public virtual DbSet<ProductTemplate> ProductTemplates { get; set; } 
    public virtual DbSet<Session> Sessions { get; set; }
    public virtual DbSet<Store> Stores { get; set; } 
    public virtual DbSet<TemplateStep> TemplateSteps { get; set; }
    public virtual DbSet<Employee> Staffs { get; set; }
    public virtual DbSet<Customer> Customers { get; set; }


        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var username = authenticatedUser.UserName ?? "";
        foreach (var entry in ChangeTracker.Entries<AuditableBaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.Created = DateTime.Now.ToUniversalTime();
                    entry.Entity.CreatedBy = username;
                    break;
                case EntityState.Modified:
                    entry.Entity.LastModified = DateTime.Now.ToUniversalTime();
                    entry.Entity.LastModifiedBy = username;
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {   
        // Product configurations
        foreach (var property in builder.Model.GetEntityTypes()
                     .SelectMany(t => t.GetProperties())
                     .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
        {
            property.SetColumnType("decimal(18,6)");
        }

        builder.ApplyConfigurationsFromAssembly(GetType().Assembly);

        base.OnModelCreating(builder);
    }
    }
}
