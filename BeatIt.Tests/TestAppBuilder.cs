namespace BeatIt.Tests;

using Avalonia;
using Avalonia.Headless;

/// <summary>
/// Provides a configured Avalonia application for headless integration testing.
/// </summary>
public class TestAppBuilder
{
    /// <summary>
    /// Builds an Avalonia application configured for headless testing
    /// using the real <see cref="App"/> with all resources and styles.
    /// </summary>
    /// <returns>The configured <see cref="AppBuilder"/> instance.</returns>
    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UseSkia()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions());
    }
}
