using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using QuickServe.Application.Interfaces.UserInterfaces;
using QuickServe.Domain.Settings;
using QuickServe.Infrastructure.Identity.Contexts;
using QuickServe.Infrastructure.Identity.Models;
using QuickServe.Infrastructure.Identity.Services;
using System.Text;

namespace QuickServe.Infrastructure.Identity
{
    public static class ServiceRegistration
    {

        public static void AddIdentityCookie(this IServiceCollection services, IConfiguration configuration)
        {
            var identitySettings = configuration.GetSection(nameof(IdentitySettings)).Get<IdentitySettings>();
            services.AddSingleton(identitySettings);
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
           {

               options.SignIn.RequireConfirmedAccount = false;
               options.SignIn.RequireConfirmedEmail = false;
               options.User.RequireUniqueEmail = false;

               options.Password.RequireDigit = identitySettings.PasswordRequireDigit;
               options.Password.RequiredLength = identitySettings.PasswordRequiredLength;
               options.Password.RequireNonAlphanumeric = identitySettings.PasswordRequireNonAlphanumic;
               options.Password.RequireUppercase = identitySettings.PasswordRequireUppercase;
               options.Password.RequireLowercase = identitySettings.PasswordRequireLowercase;
           })
               .AddEntityFrameworkStores<IdentityContext>()
               .AddDefaultTokenProviders();
        }
        public static void AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddDbContext<IdentityContext>(options =>
            //options.UseSqlServer(
            //    configuration.GetConnectionString("IdentityServer"),//"IdentityConnection"
            //    b => b.MigrationsAssembly(typeof(IdentityContext).Assembly.FullName)));
            services.AddDbContext<AppIdentityContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("PostgresIdentity"), b => b.MigrationsAssembly(typeof(IdentityContext).Assembly.FullName)));
            //options.UseNpgsql(configuration.GetConnectionString("PostgresIdentityTest"), b => b.MigrationsAssembly(typeof(IdentityContext).Assembly.FullName)));

            //services.AddTransient<IGetUserServices, GetUserServices>();
            //services.AddTransient<IUpdateUserServices, UpdateUserServices>();
        }

        public static void AddJwt(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IAccountServices, AccountServices>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //.AddCookie(options =>
            //{
            //    options.LoginPath = "/Account/Login";
            //    options.AccessDeniedPath = "/Account/AccessDenied";
            //})
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = configuration["JWTSettings:Issuer"],
                    ValidAudience = configuration["JWTSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWTSettings:Key"])),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true
                };
            });

            services.AddAuthorizationBuilder();

            services.AddIdentityCore<ApplicationUser>()
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<AppIdentityContext>();

            services.AddTransient<UserManager<ApplicationUser>>();
            services.AddTransient<RoleManager<ApplicationRole>>();
            services.AddTransient<SignInManager<ApplicationUser>>();

            //services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //})
            //    .AddJwtBearer(o =>
            //    {
            //        o.TokenValidationParameters = new TokenValidationParameters
            //        {
            //            ValidateIssuerSigningKey = true,
            //            ValidateIssuer = true,
            //            ValidateAudience = true,
            //            ValidateLifetime = true,

            //            ValidIssuer = configuration["JWTSettings:Issuer"],
            //            ValidAudience = configuration["JWTSettings:Audience"],
            //            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWTSettings:Key"]))
            //        };
            //    });

        }

    }
}
