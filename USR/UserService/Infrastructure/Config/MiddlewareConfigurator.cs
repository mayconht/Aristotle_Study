using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Aristotle.Infrastructure.Middleware;
using System.Text.Json;

namespace Aristotle.Infrastructure.Config;

/// <summary>
/// Middleware configuration helper class.
/// </summary>
public static class MiddlewareConfigurator
{
    /// <summary>
    /// Configures the middleware for the application, including exception handling, HTTPS redirection, controllers, and health checks.
    /// </summary>
    /// <param name="internalApp"></param>
    public static void ConfigureMiddleware(WebApplication internalApp)
    {
        // Redirect root requests to Swagger UI
        internalApp.Use(async (context, next) =>
        {
            if (context.Request.Path == "/")
            {
                context.Response.Redirect("/swagger");
                return;
            }
            await next();
        });
        internalApp.UseMiddleware<GlobalExceptionHandlingMiddleware>();
        // internalApp.UseHttpsRedirection(); // TODO: Enable when HTTPS is set up
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
}