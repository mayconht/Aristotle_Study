namespace Aristotle.UnitTests.Config;

using System;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Configuration class for test environment setup
/// </summary>
public static class TestConfig
{
    /// <summary>
    /// Initializes the test configuration.
    /// </summary>
    private static void Initialize()
    {
        // VerifierSettings.InitializePlugins();
    }

    /// <summary>
    /// Method to be used for test setup (can be called in test class constructor or [SetUp] method)
    /// </summary>
    public static void SetUp()
    {
        Initialize();
    }

    /// <summary>
    /// Method to be used for test teardown (can be called in [TearDown] method)
    /// </summary>
    public static Task DisposeAsync()
    {
        // Only forcefully exit in non-CI environments to avoid interfering with test runners
        if (Environment.GetEnvironmentVariable("CI") != "true") Environment.Exit(0);

        return Task.CompletedTask;
    }
}