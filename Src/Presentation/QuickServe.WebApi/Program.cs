using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QuickServe.Application;
using QuickServe.Application.Interfaces;
using QuickServe.Infrastructure.FileManager;

using QuickServe.Infrastructure.Identity;
using QuickServe.Infrastructure.Identity.Contexts;
using QuickServe.Infrastructure.Persistence;
using QuickServe.Infrastructure.Resources;
using QuickServe.WebApi.Infrastracture.Extensions;
using QuickServe.WebApi.Infrastracture.Middlewares;
using QuickServe.WebApi.Infrastracture.Services;
using Serilog;
using System.Reflection;
using Microsoft.AspNetCore.Routing;
using QuickServe.Infrastructure.Identity.Models;
using Microsoft.EntityFrameworkCore;
using QuickServe.Infrastructure.Identity.Seeds;
using QuickServe.Infrastructure.Persistence.Contexts;
using QuickServe.Application.Interfaces.IngredientInterfaces;
using QuickServe.Infrastructure.Persistence.Services;
using QuickServe.Infrastructure.FileManager.Services;
using QuickServe.Application.Interfaces.ImageInterfaces;
using QuickServe.Application.Interfaces.IProductTemplateServices;
using QuickServe.Application.Interfaces.IngredientTypeTemplateSteps;
using QuickServe.Application.Interfaces.Nutritions;
using QuickServe.Application.Interfaces.IngredientNutritions;
using QuickServe.Application.Interfaces.IOrderServices;
using QuickServe.Application.Interfaces.IngredientSessions;
using QuickServe.Application.Utils.Payments;
using Microsoft.Extensions.Configuration;
using QuickServe.Domain.Settings;
using Net.payOS;
using System;
using QuickServe.Application.Features.Orders.Commands.UpdateHub;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationLayer();
builder.Services.AddPersistenceInfrastructure(builder.Configuration);
builder.Services.AddFileManagerInfrastructure(builder.Configuration);
builder.Services.AddIdentityInfrastructure(builder.Configuration);
builder.Services.AddResourcesInfrastructure();

builder.Services.AddScoped<IAuthenticatedUserService, AuthenticatedUserService>();
builder.Services.AddScoped<IIngredientService, IngredientService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IProductTemplateService, ProductTemplateService>();
builder.Services.AddScoped<INutritionService, NutritionService>();
builder.Services.AddScoped<IIngredientNutritionService, IngredientNutritionService>();
builder.Services.AddScoped<IIngredientTypeTemplateStepService, IngredientTypeTemplateStepService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IIngredientSessionService, IngredientSessionService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IVNPayService, VNPayService>();
builder.Services.AddScoped<IPayOSService, PayOSService>();
builder.Services.AddDistributedMemoryCache();

#pragma warning disable CS0618 // Type or member is obsolete
builder.Services.AddControllers().AddFluentValidation(options =>
{
    options.ImplicitlyValidateChildProperties = true;
    options.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
});
#pragma warning restore CS0618 // Type or member is obsolete
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerWithVersioning();
builder.Services.AddCors(x =>
{
    x.AddPolicy("Any", b =>
    {
        b.AllowAnyOrigin();
        b.AllowAnyHeader();
        b.AllowAnyMethod();
    });
});
builder.Services.AddCustomLocalization(builder.Configuration);
builder.Services.AddSignalR();

#region Service
// Register third-party service
PayOS payOS = new PayOS(builder.Configuration["AppSettings:PaymentSettings:PayOSSettings:ClientId"] ?? throw new Exception("Cannot find PayOS ClientId"),
        builder.Configuration["AppSettings:PaymentSettings:PayOSSettings:ApiKey"] ?? throw new Exception("Cannot find PayOS ApiKey"),
        builder.Configuration["AppSettings:PaymentSettings:PayOSSettings:ChecksumKey"] ?? throw new Exception("Cannot find PayOS ChecksumKey"));
builder.Services.AddSingleton(payOS);
#endregion

//builder.Services.AddHealthChecks();
builder.Services.AddScoped<IAuthenticatedUserService, AuthenticatedUserService>();
builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));
builder.Services.AddJwt(builder.Configuration);

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection(nameof(AppSettings)));


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "QuickServe.WebApi v1");
    c.RoutePrefix = string.Empty;
});

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    await services.GetRequiredService<AppIdentityContext>().Database.MigrateAsync();

    //Seed Data
    await DefaultRoles.SeedAsync(services.GetRequiredService<RoleManager<ApplicationRole>>());
    await DefaultBasicUser.SeedAsync(services.GetRequiredService<UserManager<ApplicationUser>>());
   // await DefaultBasicUser.SeedAsync(
   //services.GetRequiredService<UserManager<ApplicationUser>>(),
   //services.GetRequiredService<RoleManager<ApplicationRole>>());

}

app.UseCustomLocalization();
app.UseCors("Any");
//app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseSwaggerWithVersioning();
app.UseMiddleware<ErrorHandlerMiddleware>();
//app.UseHealthChecks("/health");
app.UseSerilogRequestLogging();

app.MapHub<OrderUpdateHub>("/orderUpdateHub");


app.MapControllers();

app.Run();
