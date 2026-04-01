namespace BeatIt;

using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using BeatIt.DependencyInjection;
using BeatIt.ViewModels;
using BeatIt.Views;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Avalonia application class. Configures DI and sets up the main window.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "DI setup and main window creation require IClassicDesktopStyleApplicationLifetime, which is unavailable in headless test environments.")]
public partial class App : Application
{
    /// <summary>
    /// Gets the application-wide service provider for dependency injection.
    /// </summary>
    public static IServiceProvider Services { get; private set; } = null!;

    /// <summary>
    /// Initializes the application by loading XAML resources.
    /// </summary>
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    /// <summary>
    /// Called when the framework initialization is completed. Configures the DI container
    /// and sets up the main window.
    /// </summary>
    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();
        services.AddServices();
        services.AddViewModels();
        services.AddViews();

        Services = services.BuildServiceProvider();

        DataTemplates.Add(new ViewLocator());

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = Services.GetRequiredService<MainWindow>();
            desktop.MainWindow.DataContext = Services.GetRequiredService<MainWindowViewModel>();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
