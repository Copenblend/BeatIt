using BeatIt.DependencyInjection;
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
}
