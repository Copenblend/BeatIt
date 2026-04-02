using BeatIt.Services;
using BeatIt.ViewModels;
using BeatIt.Views;
using Microsoft.Extensions.DependencyInjection;

namespace BeatIt.DependencyInjection;

/// <summary>
/// Extension methods for configuring dependency injection services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers application services with the service collection.
    /// </summary>
    /// <param name="services">
    /// The service collection to register services with.
    /// </param>
    /// <returns>
    /// The service collection for chaining.
    /// </returns>
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IWindowService, WindowService>();
        services.AddSingleton<IStatusBarService, StatusBarService>();
        return services;
    }

    /// <summary>
    /// Registers all view models with the service collection.
    /// </summary>
    /// <param name="services">
    /// The service collection to register view models with.
    /// </param>
    /// <returns>
    /// The service collection for chaining.
    /// </returns>
    public static IServiceCollection AddViewModels(this IServiceCollection services)
    {
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<StatusBarViewModel>();
        services.AddSingleton<ActivityBarViewModel>();
        services.AddSingleton<SideBarViewModel>();
        services.AddSingleton<OutputTabViewModel>();
        services.AddSingleton<PanelViewModel>();
        return services;
    }

    /// <summary>
    /// Registers all views with the service collection.
    /// </summary>
    /// <param name="services">
    /// The service collection to register views with.
    /// </param>
    /// <returns>
    /// The service collection for chaining.
    /// </returns>
    public static IServiceCollection AddViews(this IServiceCollection services)
    {
        services.AddTransient<MainWindow>();
        services.AddTransient<ActivityBarView>();
        services.AddTransient<StatusBarView>();
        services.AddTransient<SideBarView>();
        services.AddTransient<PanelView>();
        services.AddTransient<OutputTabView>();
        return services;
    }
}
