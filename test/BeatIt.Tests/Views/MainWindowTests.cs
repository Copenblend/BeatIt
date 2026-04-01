using Avalonia.Headless;

[assembly: AvaloniaTestApplication(typeof(BeatIt.Tests.TestAppBuilder))]

namespace BeatIt.Tests.Views;

using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using BeatIt.Services;
using BeatIt.ViewModels;
using BeatIt.Views;
using FluentAssertions;
using Moq;

/// <summary>
/// Headless integration tests for <see cref="MainWindow"/>.
/// Verifies the workbench visual tree structure, named zones,
/// title bar controls, and custom chrome configuration.
/// </summary>
public class MainWindowTests
{
    /// <summary>
    /// Creates a <see cref="MainWindow"/> configured with a mocked
    /// <see cref="IWindowService"/>. The window is not shown to avoid
    /// headless font rendering issues; named controls are available
    /// via the name scope populated during <c>InitializeComponent</c>.
    /// </summary>
    /// <returns>The configured <see cref="MainWindow"/> instance.</returns>
    private static MainWindow CreateWindow()
    {
        var windowService = Mock.Of<IWindowService>();
        var viewModel = new MainWindowViewModel(windowService);
        return new MainWindow { DataContext = viewModel };
    }

    /// <summary>
    /// Verifies that all workbench layout zones exist in the visual tree:
    /// ActivityBarZone, SideBarZone, WorkspaceZone, PanelZone, StatusBarZone,
    /// and TitleBarDragRegion.
    /// </summary>
    [AvaloniaFact]
    public void AllWorkbenchZones_ExistInVisualTree()
    {
        // Arrange
        var window = CreateWindow();

        // Act & Assert
        window.FindControl<Border>("ActivityBarZone").Should().NotBeNull();
        window.FindControl<Border>("SideBarZone").Should().NotBeNull();
        window.FindControl<Border>("WorkspaceZone").Should().NotBeNull();
        window.FindControl<Border>("PanelZone").Should().NotBeNull();
        window.FindControl<Border>("StatusBarZone").Should().NotBeNull();
        window.FindControl<Border>("TitleBarDragRegion").Should().NotBeNull();
    }

    /// <summary>
    /// Verifies that the title bar contains MinimizeButton, MaximizeRestoreButton,
    /// and CloseButton as <see cref="Button"/> controls.
    /// </summary>
    [AvaloniaFact]
    public void TitleBarButtons_ExistInVisualTree()
    {
        // Arrange
        var window = CreateWindow();

        // Act & Assert
        window.FindControl<Button>("MinimizeButton").Should().NotBeNull();
        window.FindControl<Button>("MaximizeRestoreButton").Should().NotBeNull();
        window.FindControl<Button>("CloseButton").Should().NotBeNull();
    }

    /// <summary>
    /// Verifies that the MenuBarPlaceholder control exists in the visual tree.
    /// </summary>
    [AvaloniaFact]
    public void MenuBarPlaceholder_ExistsInVisualTree()
    {
        // Arrange
        var window = CreateWindow();

        // Act & Assert
        window.FindControl<ContentControl>("MenuBarPlaceholder").Should().NotBeNull();
    }

    /// <summary>
    /// Verifies that the window is configured with custom chrome:
    /// <see cref="Window.ExtendClientAreaToDecorationsHint"/> is <c>true</c> and
    /// <see cref="Window.ExtendClientAreaChromeHints"/> is <see cref="Avalonia.Platform.ExtendClientAreaChromeHints.NoChrome"/>.
    /// </summary>
    [AvaloniaFact]
    public void CustomChrome_IsConfigured()
    {
        // Arrange
        var window = CreateWindow();

        // Act & Assert
        window.ExtendClientAreaToDecorationsHint.Should().BeTrue();
        window.ExtendClientAreaChromeHints.Should().Be(Avalonia.Platform.ExtendClientAreaChromeHints.NoChrome);
    }
}
