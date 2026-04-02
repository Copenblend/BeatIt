using BeatIt.Services;
using BeatIt.ViewModels;
using FluentAssertions;
using Moq;
using Xunit;

namespace BeatIt.Tests.ViewModels;

/// <summary>
/// Unit tests for <see cref="MainWindowViewModel"/>.
/// Verifies window control commands delegate correctly to <see cref="IWindowService"/>
/// and that observable properties reflect service state.
/// </summary>
public sealed class MainWindowViewModelTests
{
    private readonly Mock<IWindowService> _mockWindowService;
    private readonly ActivityBarViewModel _activityBar = new();
    private readonly SideBarViewModel _sideBar = new();
    private readonly PanelViewModel _panel = new(new OutputTabViewModel());
    private readonly MenuBarViewModel _menuBar = new();

    /// <summary>
    /// Initializes test fixtures with a mocked <see cref="IWindowService"/>.
    /// </summary>
    public MainWindowViewModelTests()
    {
        _mockWindowService = new Mock<IWindowService>();
    }

    [Fact]
    public void Constructor_SetsIsMaximizedFromService()
    {
        // Arrange
        _mockWindowService.Setup(s => s.IsMaximized).Returns(true);

        // Act
        var sut = new MainWindowViewModel(_mockWindowService.Object, _activityBar, _sideBar, _panel, _menuBar);

        // Assert
        sut.IsMaximized.Should().BeTrue();
    }

    [Fact]
    public void Constructor_SetsIsMaximizedFalse_WhenServiceReturnsFalse()
    {
        // Arrange
        _mockWindowService.Setup(s => s.IsMaximized).Returns(false);

        // Act
        var sut = new MainWindowViewModel(_mockWindowService.Object, _activityBar, _sideBar, _panel, _menuBar);

        // Assert
        sut.IsMaximized.Should().BeFalse();
    }

    [Fact]
    public void MinimizeCommand_CallsServiceMinimize()
    {
        // Arrange
        var sut = new MainWindowViewModel(_mockWindowService.Object, _activityBar, _sideBar, _panel, _menuBar);

        // Act
        sut.MinimizeCommand.Execute(null);

        // Assert
        _mockWindowService.Verify(s => s.Minimize(), Times.Once);
    }

    [Fact]
    public void MaximizeRestoreCommand_CallsServiceMaximizeRestore()
    {
        // Arrange
        var sut = new MainWindowViewModel(_mockWindowService.Object, _activityBar, _sideBar, _panel, _menuBar);

        // Act
        sut.MaximizeRestoreCommand.Execute(null);

        // Assert
        _mockWindowService.Verify(s => s.MaximizeRestore(), Times.Once);
    }

    [Fact]
    public void MaximizeRestoreCommand_UpdatesIsMaximized()
    {
        // Arrange
        _mockWindowService.Setup(s => s.IsMaximized).Returns(false);
        var sut = new MainWindowViewModel(_mockWindowService.Object, _activityBar, _sideBar, _panel, _menuBar);

        _mockWindowService
            .Setup(s => s.MaximizeRestore())
            .Callback(() =>
                _mockWindowService.Setup(s => s.IsMaximized).Returns(true));

        // Act
        sut.MaximizeRestoreCommand.Execute(null);

        // Assert
        sut.IsMaximized.Should().BeTrue();
    }

    [Fact]
    public void CloseCommand_CallsServiceClose()
    {
        // Arrange
        var sut = new MainWindowViewModel(_mockWindowService.Object, _activityBar, _sideBar, _panel, _menuBar);

        // Act
        sut.CloseCommand.Execute(null);

        // Assert
        _mockWindowService.Verify(s => s.Close(), Times.Once);
    }

    [Fact]
    public void Constructor_SetsActivityBarProperty()
    {
        // Arrange & Act
        var sut = new MainWindowViewModel(_mockWindowService.Object, _activityBar, _sideBar, _panel, _menuBar);

        // Assert
        sut.ActivityBar.Should().BeSameAs(_activityBar);
    }

    [Fact]
    public void Constructor_SetsSideBarProperty()
    {
        // Arrange & Act
        var sut = new MainWindowViewModel(_mockWindowService.Object, _activityBar, _sideBar, _panel, _menuBar);

        // Assert
        sut.SideBar.Should().BeSameAs(_sideBar);
    }

    [Fact]
    public void Constructor_SetsMenuBarProperty()
    {
        // Arrange & Act
        var sut = new MainWindowViewModel(_mockWindowService.Object, _activityBar, _sideBar, _panel, _menuBar);

        // Assert
        sut.MenuBar.Should().BeSameAs(_menuBar);
    }

    [Fact]
    public void ActivityBarSelectedItemChanged_WhenSideBarWidthIsZero_RestoresDefaultWidth()
    {
        // Arrange
        var sut = new MainWindowViewModel(_mockWindowService.Object, _activityBar, _sideBar, _panel, _menuBar);
        _sideBar.Width = 0;
        _activityBar.SelectedItem = null;

        // Act
        _activityBar.SelectedItem = _activityBar.Items[0];

        // Assert
        sut.SideBar.Width.Should().Be(SideBarViewModel.DefaultWidth);
    }

    [Fact]
    public void ActivityBarSelectedItemChanged_WhenSideBarWidthIsNonZero_LeavesWidthUnchanged()
    {
        // Arrange
        var sut = new MainWindowViewModel(_mockWindowService.Object, _activityBar, _sideBar, _panel, _menuBar);
        const double customWidth = 300.0;
        _sideBar.Width = customWidth;
        _activityBar.SelectedItem = null;

        // Act
        _activityBar.SelectedItem = _activityBar.Items[0];

        // Assert
        sut.SideBar.Width.Should().Be(customWidth);
    }
}
