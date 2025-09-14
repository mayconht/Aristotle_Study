using System.Diagnostics;
using System.Reflection;
using Aristotle.Application;
using Aristotle.Application.Service;
using Aristotle.Domain.Interfaces;
using Aristotle.Infrastructure;
using Aristotle.Infrastructure.Data.Repositories;
using Aristotle.Infrastructure.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Text.Json;


//TODO This class is a mess and needs to be cleaned up and moved to proper files
// It is doing too many things at once and is hard to read and maintain
// I should move the configuration to separate files and use proper patterns
var builder = WebApplication.CreateBuilder(args);
var openBrowser = builder.Configuration.GetValue("Swagger:OpenBrowser", false);

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "UserService API", Version = "v1" });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();

DotNetEnv.Env.Load("../.env");

void ConfigureDatabase(WebApplicationBuilder builder)
{
    var connectionString = $"Host={Environment.GetEnvironmentVariable("POSTGRES_HOST")};" +
                          $"Port={Environment.GetEnvironmentVariable("POSTGRES_PORT")};" +
                          $"Database={Environment.GetEnvironmentVariable("POSTGRES_DB")};" +
                          $"Username={Environment.GetEnvironmentVariable("POSTGRES_USER")};" +
                          $"Password={Environment.GetEnvironmentVariable("POSTGRES_PASSWORD")}";

    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException("Connection string could not be resolved from environment variables.");
    }

    if (connectionString.StartsWith("Host=") || connectionString.StartsWith("postgresql://"))
    {
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));
    }
    else if (connectionString.StartsWith("Data Source="))
    {
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(connectionString));
    }
    else
    {
        throw new InvalidOperationException("Unsupported or missing connection string format for DefaultConnection.");
    }
}


//------------------------------------ Main Program Execution ------------------------------------ //
ConfigureDatabase(builder);
RegisterServices(builder);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        await dbContext.Database.EnsureCreatedAsync();
        await dbContext.Database.MigrateAsync();
        Console.WriteLine("Database migrations applied successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while applying migrations: {ex.Message}");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "UserService API v1");
        c.DocumentTitle = "UserService API Documentation";
    });

    if (openBrowser)
    {
        _ = Task.Run(() =>
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = app.Configuration["Swagger:Url"],
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to open Swagger UI in the browser. Error: {ex.Message}");
            }
        });
    }
}

ConfigureMiddleware(app);

await app.RunAsync();
return;

void RegisterServices(WebApplicationBuilder internalBuilder)
{
    internalBuilder.Services.AddScoped<IUserRepository, UserRepository>();
    internalBuilder.Services.AddScoped<IUserService, UserService>();
    internalBuilder.Services.AddAutoMapper(config => config.AddProfile<MappingProfile>());

    internalBuilder.Services.AddHealthChecks()
        .AddCheck("Liveness", () => HealthCheckResult.Healthy());
}

void ConfigureMiddleware(WebApplication internalApp)
{
    internalApp.UseMiddleware<GlobalExceptionHandlingMiddleware>();
    internalApp.UseHttpsRedirection();
    internalApp.MapControllers();
    internalApp.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = async (context, report) =>
        {
            context.Response.ContentType = "application/json";
            var result = JsonSerializer.Serialize(new
            {
                status = report.Status.ToString(),
                checks = report.Entries.Select(e => new
                {
                    name = e.Key,
                    status = e.Value.Status.ToString(),
                    error = e.Value.Exception?.Message
                }),
                duration = report.TotalDuration.TotalMilliseconds
            });
            await context.Response.WriteAsync(result);
        }
    });
}

/// <summary>
/// Program class for test access
/// </summary>
public abstract partial class Program
{
    /// <summary>
    /// Protected constructor for testing
    /// </summary>
    protected Program()
    {
    }
}