namespace Aristotle.UnitTests.Config;

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
        return Task.CompletedTask;
    }
}