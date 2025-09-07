using System.Diagnostics;
using System.Reflection;
using Aristotle.Application;
using Aristotle.Application.Service;
using Aristotle.Domain.Interfaces;
using Aristotle.Infrastructure;
using Aristotle.Infrastructure.Data.Repositories;
using Aristotle.Infrastructure.Exceptions;
using Aristotle.Infrastructure.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var openBrowser = false;

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Aristotle API", Version = "v1" });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();

// Here I am super lazy but if you want to properly make it interchangeable,
// you can use a configuration file or environment variables to set the connection string.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<UserService>();

builder.Services.AddAutoMapper(config => config.AddProfile<MappingProfile>());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();

        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();

            logger.LogInformation("Validating if database is created and migrations are applied...");
            await context.Database.MigrateAsync();
            logger.LogInformation("Database initialization completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while creating/migrating the database: {Message}", ex.Message);
            throw new DatabaseException(ex.Message, "N/A", "Database Initialization", ex.ToString());
        }
    }

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Aristotle API v1");
        c.DocumentTitle = "Aristotle API Documentation";
    });
    
    var swaggerUrl = app.Configuration["Swagger:Url"];
    if (openBrowser)
    {
        _ = Task.Run(() =>
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = swaggerUrl,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Failed to open Swagger UI in the browser. Please visit {swaggerUrl} manually. Error: {ex.Message}");
            }
        });
    }
    else
    {
        Console.WriteLine($"Swagger UI is available at: {swaggerUrl}");
    }

    app.MapGet("/", context =>
    {
        context.Response.Redirect("/swagger/index.html");
        return Task.CompletedTask;
    });
}

// We are going to move this to a middleware handler or a service handler later.
// But feels useful to register the services like this in C# 
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.MapControllers();

// Check if this is being called from a test environment
// In test scenarios, we don't want to start the web server
// This feels ugly but it works for now.
var isTestEnvironment = builder.Configuration.GetValue<bool>("TestEnvironment:Enabled") ||
                        args.Contains("--test") ||
                        (Assembly.GetEntryAssembly()?.GetName().Name?.Contains("testhost") ?? false);

if (!isTestEnvironment) await app.RunAsync();

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