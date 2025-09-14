using Aristotle.Application.Service;
using Aristotle.Domain.Interfaces;
using Aristotle.Infrastructure.Data.Repositories;
using Aristotle.Application;
using Aristotle.Infrastructure;
using Aristotle.Infrastructure.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Aristotle.Infrastructure.Config;

/// <summary>
/// Service registration helper class.
/// </summary>
public static class ServiceRegistrar
{
    /// <summary>
    /// Registers application services and repositories for dependency injection.
    /// </summary>
    /// <param name="internalBuilder"></param>
    public static void RegisterServices(WebApplicationBuilder internalBuilder)
    {
        internalBuilder.Services.AddScoped<IUserRepository, UserRepository>();
        internalBuilder.Services.AddScoped<IUserService, UserService>();
        internalBuilder.Services.AddAutoMapper(config => config.AddProfile<MappingProfile>());

        internalBuilder.Services.AddHealthChecks()
            .AddCheck("Liveness", () => HealthCheckResult.Healthy("Service is running"))
            .AddCheck<DatabaseHealthCheck>("Database");
    }
}