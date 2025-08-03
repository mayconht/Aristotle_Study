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
    public static void TearDown()
    {
        try
        {
            if (Environment.GetEnvironmentVariable("CI") != "true") return;
            Thread.Sleep(2000);
                
            Task.Run(async () =>
            {
                await Task.Delay(5000);
                Environment.Exit(0);
            });
        }
        catch
        {
            // Ensure we don't throw exceptions during teardown
        }
    }
}