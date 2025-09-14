using System.Diagnostics;
using System.Reflection;
using Aristotle.Infrastructure;
using Aristotle.Infrastructure.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

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


//------------------------------------ Main Program Execution ------------------------------------ //
DatabaseConfigurator.ConfigureDatabase(builder);
ServiceRegistrar.RegisterServices(builder);

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

MiddlewareConfigurator.ConfigureMiddleware(app);

await app.RunAsync();


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