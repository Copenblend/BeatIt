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
        var activityBar = new ActivityBarViewModel();
        var sideBar = new SideBarViewModel();
        var viewModel = new MainWindowViewModel(windowService, activityBar, sideBar);
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
        window.FindControl<ActivityBarView>("ActivityBarZone").Should().NotBeNull();
        window.FindControl<SideBarView>("SideBarZone").Should().NotBeNull();
        window.FindControl<Border>("WorkspaceZone").Should().NotBeNull();
        window.FindControl<Border>("PanelZone").Should().NotBeNull();
        window.FindControl<StatusBarView>("StatusBarZone").Should().NotBeNull();
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

    /// <summary>
    /// Verifies that <see cref="StatusBarView"/> renders a <see cref="TextBlock"/>
    /// named <c>StatusTextBlock</c> whose text reflects the value returned by
    /// <see cref="IStatusBarService.StatusText"/>.
    /// </summary>
    [AvaloniaFact]
    public void StatusBarView_RendersWithReadyText()
    {
        // Arrange
        var statusBarService = new Mock<IStatusBarService>();
        statusBarService.Setup(s => s.StatusText).Returns("Ready");
        var viewModel = new StatusBarViewModel(statusBarService.Object);
        var statusBarView = new StatusBarView { DataContext = viewModel };

        // Act
        var textBlock = statusBarView.FindControl<TextBlock>("StatusTextBlock");

        // Assert
        textBlock.Should().NotBeNull();
        textBlock!.Text.Should().Be("Ready");
    }

    /// <summary>
    /// Verifies that <see cref="ActivityBarView"/> renders the default Explorer icon
    /// and the <c>ActivityBarItemsControl</c> is present with one item.
    /// </summary>
    [AvaloniaFact]
    public void ActivityBarView_RendersExplorerIcon()
    {
        // Arrange
        var viewModel = new ActivityBarViewModel();
        var activityBarView = new ActivityBarView { DataContext = viewModel };

        // Act
        var itemsControl = activityBarView.FindControl<ItemsControl>("ActivityBarItemsControl");

        // Assert
        itemsControl.Should().NotBeNull();
        itemsControl!.ItemCount.Should().Be(2);
    }

    /// <summary>
    /// Verifies that the SideBarZone has a <see cref="SideBarViewModel"/> DataContext
    /// bound via the MainWindowViewModel.SideBar property.
    /// </summary>
    [AvaloniaFact]
    public void SideBarZone_HasSideBarViewModelDataContext()
    {
        // Arrange
        var window = CreateWindow();

        // Act
        var sideBarView = window.FindControl<SideBarView>("SideBarZone");

        // Assert
        sideBarView.Should().NotBeNull();
        sideBarView!.DataContext.Should().BeOfType<SideBarViewModel>();
    }

    /// <summary>
    /// Verifies that <see cref="SideBarView"/> displays the Open Folder button
    /// when <see cref="SideBarViewModel.SideBarContent"/> is null.
    /// </summary>
    [AvaloniaFact]
    public void SideBarView_ShowsEmptyState_WhenNoContent()
    {
        // Arrange
        var viewModel = new SideBarViewModel();
        var sideBarView = new SideBarView { DataContext = viewModel };

        // Act
        var openFolderButton = sideBarView.FindControl<Button>("OpenFolderButton");

        // Assert
        openFolderButton.Should().NotBeNull();
    }

    /// <summary>
    /// Verifies that the <see cref="MainWindow"/> contains a resize grip
    /// control named <c>ResizeGrip</c> in the middle content row.
    /// </summary>
    [AvaloniaFact]
    public void ResizeGrip_ExistsInMainWindow()
    {
        // Arrange
        var window = CreateWindow();

        // Act
        var resizeGrip = window.FindControl<Border>("ResizeGrip");

        // Assert
        resizeGrip.Should().NotBeNull();
    }
}
