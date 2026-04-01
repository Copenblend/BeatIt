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
        return services;
    }
}
