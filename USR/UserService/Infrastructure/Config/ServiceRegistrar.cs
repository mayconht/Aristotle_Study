using Aristotle.Application.Service;
using Aristotle.Domain.Interfaces;
using Aristotle.Infrastructure.Data.Repositories;
using Aristotle.Application;

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
            .AddCheck("Liveness", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy())
            .AddCheck("Database", () =>
            {
                using var scope = internalBuilder.Services.BuildServiceProvider().CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                return dbContext.Database.CanConnect()
                    ? Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy()
                    : Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Unhealthy("Cannot connect to the database.");
            });
    }
}