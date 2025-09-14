using Aristotle.Application.Service;
using Aristotle.Domain.Interfaces;
using Aristotle.Infrastructure.Data.Repositories;
using Aristotle.Application;
using Aristotle.Infrastructure.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Reflection;

namespace Aristotle.Infrastructure.Config;

/// <summary>
/// Service registration helper class.
/// </summary>
public static class RegisterService
{
    /// <summary>
    /// Registers application services and repositories for dependency injection.
    /// </summary>
    /// <param name="internalBuilder"></param>
    public static void Initialize(WebApplicationBuilder internalBuilder)
    {
        internalBuilder.Services.AddScoped<IUserRepository, UserRepository>();
        
        internalBuilder.Services.AddScoped<IUserService, UserService>();
        
        // Profiles are a way to organize AutoMapper configurations
        // Here we are adding the MappingProfile defined in the Application layer so that AutoMapper knows how to map between our entities and DTOs
        internalBuilder.Services.AddAutoMapper(config => config.AddProfile<MappingProfile>()); 
        
        internalBuilder.Services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        internalBuilder.Services.AddHealthChecks()
            .AddCheck("Liveness", () => HealthCheckResult.Healthy("Service is running")) //TODO: Add a proper liveness check 
            .AddCheck<DatabaseHealthCheck>("Database");
        // .AddNpgSql(internalBuilder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty, name: "PostgreSQL");
    }
}