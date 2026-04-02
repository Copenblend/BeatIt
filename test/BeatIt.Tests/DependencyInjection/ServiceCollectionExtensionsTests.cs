using BeatIt.DependencyInjection;
using BeatIt.Services;
using BeatIt.ViewModels;
using BeatIt.Views;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BeatIt.Tests.DependencyInjection;

/// <summary>
/// Tests for <see cref="ServiceCollectionExtensions"/>.
/// Verifies that DI registration extension methods register the correct
/// service descriptors without building the service provider.
/// </summary>
public sealed class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddViewModels_RegistersMainWindowViewModelAsTransient()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddViewModels();

        // Assert
        services.Should().ContainSingle(d =>
            d.ServiceType == typeof(MainWindowViewModel)
            && d.Lifetime == ServiceLifetime.Transient);
    }

    [Fact]
    public void AddViewModels_ReturnsSameServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddViewModels();

        // Assert
        result.Should().BeSameAs(services);
    }

    [Fact]
    public void AddViews_RegistersMainWindowAsTransient()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddViews();

        // Assert
        services.Should().ContainSingle(d =>
            d.ServiceType == typeof(MainWindow)
            && d.Lifetime == ServiceLifetime.Transient);
    }

    [Fact]
    public void AddViews_ReturnsSameServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddViews();

        // Assert
        result.Should().BeSameAs(services);
    }

    [Fact]
    public void AddServices_RegistersWindowServiceAsSingleton()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddServices();

        // Assert
        services.Should().ContainSingle(d =>
            d.ServiceType == typeof(IWindowService)
            && d.ImplementationType == typeof(WindowService)
            && d.Lifetime == ServiceLifetime.Singleton);
    }

    [Fact]
    public void AddServices_RegistersStatusBarServiceAsSingleton()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddServices();

        // Assert
        services.Should().ContainSingle(d =>
            d.ServiceType == typeof(IStatusBarService)
            && d.ImplementationType == typeof(StatusBarService)
            && d.Lifetime == ServiceLifetime.Singleton);
    }

    [Fact]
    public void AddServices_ReturnsSameServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddServices();

        // Assert
        result.Should().BeSameAs(services);
    }

    [Fact]
    public void AddViewModels_RegistersStatusBarViewModelAsTransient()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddViewModels();

        // Assert
        services.Should().ContainSingle(d =>
            d.ServiceType == typeof(StatusBarViewModel)
            && d.Lifetime == ServiceLifetime.Transient);
    }

    [Fact]
    public void AddViewModels_RegistersActivityBarViewModelAsSingleton()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddViewModels();

        // Assert
        services.Should().ContainSingle(d =>
            d.ServiceType == typeof(ActivityBarViewModel)
            && d.Lifetime == ServiceLifetime.Singleton);
    }

    /// <summary>
    /// Verifies that <see cref="ServiceCollectionExtensions.AddViews"/>
    /// registers <see cref="ActivityBarView"/> as a transient service.
    /// </summary>
    [Fact]
    public void AddViews_RegistersActivityBarViewAsTransient()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddViews();

        // Assert
        services.Should().ContainSingle(d =>
            d.ServiceType == typeof(ActivityBarView)
            && d.Lifetime == ServiceLifetime.Transient);
    }

    /// <summary>
    /// Verifies that <see cref="ServiceCollectionExtensions.AddViews"/>
    /// registers <see cref="StatusBarView"/> as a transient service.
    /// </summary>
    [Fact]
    public void AddViews_RegistersStatusBarViewAsTransient()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddViews();

        // Assert
        services.Should().ContainSingle(d =>
            d.ServiceType == typeof(StatusBarView)
            && d.Lifetime == ServiceLifetime.Transient);
    }

    [Fact]
    public void AddViewModels_RegistersSideBarViewModelAsSingleton()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddViewModels();

        // Assert
        services.Should().ContainSingle(d =>
            d.ServiceType == typeof(SideBarViewModel)
            && d.Lifetime == ServiceLifetime.Singleton);
    }

    [Fact]
    public void AddViews_RegistersSideBarViewAsTransient()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddViews();

        // Assert
        services.Should().ContainSingle(d =>
            d.ServiceType == typeof(SideBarView)
            && d.Lifetime == ServiceLifetime.Transient);
    }

    /// <summary>
    /// Verifies that <see cref="ServiceCollectionExtensions.AddViewModels"/>
    /// registers <see cref="OutputTabViewModel"/> as a singleton service.
    /// </summary>
    [Fact]
    public void AddViewModels_RegistersOutputTabViewModelAsSingleton()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddViewModels();

        // Assert
        services.Should().ContainSingle(d =>
            d.ServiceType == typeof(OutputTabViewModel)
            && d.Lifetime == ServiceLifetime.Singleton);
    }

    /// <summary>
    /// Verifies that <see cref="ServiceCollectionExtensions.AddViewModels"/>
    /// registers <see cref="PanelViewModel"/> as a singleton service.
    /// </summary>
    [Fact]
    public void AddViewModels_RegistersPanelViewModelAsSingleton()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddViewModels();

        // Assert
        services.Should().ContainSingle(d =>
            d.ServiceType == typeof(PanelViewModel)
            && d.Lifetime == ServiceLifetime.Singleton);
    }

    /// <summary>
    /// Verifies that <see cref="ServiceCollectionExtensions.AddViews"/>
    /// registers <see cref="PanelView"/> as a transient service.
    /// </summary>
    [Fact]
    public void AddViews_RegistersPanelViewAsTransient()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddViews();

        // Assert
        services.Should().ContainSingle(d =>
            d.ServiceType == typeof(PanelView)
            && d.Lifetime == ServiceLifetime.Transient);
    }

    /// <summary>
    /// Verifies that <see cref="ServiceCollectionExtensions.AddViews"/>
    /// registers <see cref="OutputTabView"/> as a transient service.
    /// </summary>
    [Fact]
    public void AddViews_RegistersOutputTabViewAsTransient()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddViews();

        // Assert
        services.Should().ContainSingle(d =>
            d.ServiceType == typeof(OutputTabView)
            && d.Lifetime == ServiceLifetime.Transient);
    }
}
