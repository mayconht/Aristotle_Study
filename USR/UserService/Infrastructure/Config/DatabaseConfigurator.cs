using Microsoft.EntityFrameworkCore;

namespace Aristotle.Infrastructure.Config;

/// <summary>
/// Database configuration helper class.
/// </summary>
public static class DatabaseConfigurator
{
    /// <summary>
    /// Configures the database based on environment variables or connection strings.
    /// It allows switching between SQLite and PostgreSQL.
    /// Priority is given to the USE_SQLITE environment variable.
    /// </summary>
    /// <param name="internalBuilder"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public static void ConfigureDatabase(WebApplicationBuilder internalBuilder)
    {
        if (TryConfigureSqliteFromEnvironment(internalBuilder))
            return;

        var connectionString = internalBuilder.Configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(connectionString)) throw new InvalidOperationException("Missing connection string for DefaultConnection.");

        if (TryConfigurePostgresql(internalBuilder, connectionString))
            return;

        if (TryConfigureSqlite(internalBuilder, connectionString))
            return;

        throw new InvalidOperationException("Unsupported connection string format for DefaultConnection.");
    }

    private static bool TryConfigureSqliteFromEnvironment(WebApplicationBuilder configBuilder)
    {
        var useSqlite = string.Equals(Environment.GetEnvironmentVariable("USE_SQLITE"), "true", StringComparison.OrdinalIgnoreCase);
        if (!useSqlite)
            return false;

        configBuilder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite("Data Source=users.db"));
        Console.WriteLine("Using SQLite database with db path ./USR/UserService/users.db.");
        return true;
    }

    private static bool TryConfigurePostgresql(WebApplicationBuilder configBuilder, string connectionString)
    {
        if (!connectionString.StartsWith("Host="))
            return false;

        configBuilder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));
        Console.WriteLine("Using PostgreSQL database.");
        return true;
    }

    private static bool TryConfigureSqlite(WebApplicationBuilder configBuilder, string connectionString)
    {
        if (!connectionString.StartsWith("Data Source="))
            return false;

        configBuilder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(connectionString));
        Console.WriteLine("Using SQLite database from connection string.");
        return true;
    }
}