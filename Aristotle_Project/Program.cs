using Aristotle.Application.Service;
using Aristotle.Domain.Interfaces;
using Aristotle.Infrastructure;
using Aristotle.Infrastructure.Data.Repositories;
using Aristotle.Infrastructure.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Aristotle API", Version = "v1" });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
// Here I am super lazy but if you want to properly make it interchangeable,
// you can use a configuration file or environment variables to set the connection string.
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite("Data Source=users.db"));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<UserService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Aristotle API v1");
        c.DocumentTitle = "Aristotle API Documentation";
    });

    const string url = "http://localhost:3000/swagger/index.html";
    _ = Task.Run(() =>
    {
        try
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
        catch
        {
            Console.WriteLine("Failed to open Swagger UI in the browser. Please visit " + url + " manually.");
        }
    });
}

// We are going to move this to a middleware handler or a service handler later.
// But feels useful to register the services like this in C# 
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.MapControllers();

app.RunAsync();