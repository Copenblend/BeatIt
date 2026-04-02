using BeatIt.ViewModels;
using FluentAssertions;
using Xunit;

namespace BeatIt.Tests.ViewModels;

/// <summary>
/// Unit tests for <see cref="PanelViewModel"/>.
/// Verifies constructor behavior, default property values, tab management,
/// height clamping logic, and property change notifications.
/// </summary>
public sealed class PanelViewModelTests
{
    [Fact]
    public void Constructor_AddsOutputTabToTabs()
    {
        // Arrange
        var outputTab = new OutputTabViewModel();

        // Act
        var sut = new PanelViewModel(outputTab);

        // Assert
        sut.Tabs.Should().ContainSingle()
            .Which.Should().BeSameAs(outputTab);
    }

    [Fact]
    public void Constructor_SetsSelectedTabToOutputTab()
    {
        // Arrange
        var outputTab = new OutputTabViewModel();

        // Act
        var sut = new PanelViewModel(outputTab);

        // Assert
        sut.SelectedTab.Should().BeSameAs(outputTab);
    }

    [Fact]
    public void Constructor_SetsDefaultHeight()
    {
        // Arrange & Act
        var sut = new PanelViewModel(new OutputTabViewModel());

        // Assert
        sut.Height.Should().Be(PanelViewModel.DefaultHeight);
    }

    [Fact]
    public void Constructor_SetsIsVisibleToTrue()
    {
        // Arrange & Act
        var sut = new PanelViewModel(new OutputTabViewModel());

        // Assert
        sut.IsVisible.Should().BeTrue();
    }

    [Fact]
    public void Height_WhenChanged_RaisesPropertyChanged()
    {
        // Arrange
        var sut = new PanelViewModel(new OutputTabViewModel());
        using var monitor = sut.Monitor();

        // Act
        sut.Height = 350.0;

        // Assert
        monitor.Should().RaisePropertyChangeFor(s => s.Height);
    }

    [Fact]
    public void IsVisible_WhenChanged_RaisesPropertyChanged()
    {
        // Arrange
        var sut = new PanelViewModel(new OutputTabViewModel());
        using var monitor = sut.Monitor();

        // Act
        sut.IsVisible = false;

        // Assert
        monitor.Should().RaisePropertyChangeFor(s => s.IsVisible);
    }

    [Fact]
    public void SelectedTab_WhenChanged_RaisesPropertyChanged()
    {
        // Arrange
        var sut = new PanelViewModel(new OutputTabViewModel());
        using var monitor = sut.Monitor();
        var newTab = new OutputTabViewModel();

        // Act
        sut.SelectedTab = newTab;

        // Assert
        monitor.Should().RaisePropertyChangeFor(s => s.SelectedTab);
    }

    [Fact]
    public void AddTab_AddsTabToCollection()
    {
        // Arrange
        var sut = new PanelViewModel(new OutputTabViewModel());
        var newTab = new OutputTabViewModel();

        // Act
        sut.AddTab(newTab);

        // Assert
        sut.Tabs.Should().HaveCount(2);
        sut.Tabs[1].Should().BeSameAs(newTab);
    }

    [Fact]
    public void ClampHeight_BelowMinHeight_ReturnsZero()
    {
        // Arrange & Act
        var result = PanelViewModel.ClampHeight(50.0, 500.0);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void ClampHeight_AboveMaxHeight_ReturnsMaxHeight()
    {
        // Arrange & Act
        var result = PanelViewModel.ClampHeight(600.0, 500.0);

        // Assert
        result.Should().Be(500.0);
    }

    [Fact]
    public void ClampHeight_WithinRange_ReturnsProposedHeight()
    {
        // Arrange & Act
        var result = PanelViewModel.ClampHeight(250.0, 500.0);

        // Assert
        result.Should().Be(250.0);
    }

    [Fact]
    public void ClampHeight_ExactlyMinHeight_ReturnsMinHeight()
    {
        // Arrange & Act
        var result = PanelViewModel.ClampHeight(PanelViewModel.MinHeight, 500.0);

        // Assert
        result.Should().Be(PanelViewModel.MinHeight);
    }

    [Fact]
    public void ClampHeight_ExactlyMaxHeight_ReturnsMaxHeight()
    {
        // Arrange & Act
        var result = PanelViewModel.ClampHeight(500.0, 500.0);

        // Assert
        result.Should().Be(500.0);
    }

    /// <summary>
    /// Verifies that executing <see cref="PanelViewModel.TogglePanelCommand"/>
    /// when the panel is visible sets <see cref="PanelViewModel.IsVisible"/> to false.
    /// </summary>
    [Fact]
    public void TogglePanelCommand_WhenVisible_SetsIsVisibleFalse()
    {
        // Arrange
        var sut = new PanelViewModel(new OutputTabViewModel());
        sut.IsVisible.Should().BeTrue("panel is visible by default");

        // Act
        sut.TogglePanelCommand.Execute(null);

        // Assert
        sut.IsVisible.Should().BeFalse();
    }

    /// <summary>
    /// Verifies that executing <see cref="PanelViewModel.TogglePanelCommand"/>
    /// when the panel is visible sets <see cref="PanelViewModel.Height"/> to zero.
    /// </summary>
    [Fact]
    public void TogglePanelCommand_WhenVisible_SetsHeightToZero()
    {
        // Arrange
        var sut = new PanelViewModel(new OutputTabViewModel());
        sut.IsVisible.Should().BeTrue("panel is visible by default");

        // Act
        sut.TogglePanelCommand.Execute(null);

        // Assert
        sut.Height.Should().Be(0);
    }

    /// <summary>
    /// Verifies that executing <see cref="PanelViewModel.TogglePanelCommand"/>
    /// when the panel is hidden sets <see cref="PanelViewModel.IsVisible"/> to true.
    /// </summary>
    [Fact]
    public void TogglePanelCommand_WhenHidden_SetsIsVisibleTrue()
    {
        // Arrange
        var sut = new PanelViewModel(new OutputTabViewModel());
        sut.IsVisible = false;

        // Act
        sut.TogglePanelCommand.Execute(null);

        // Assert
        sut.IsVisible.Should().BeTrue();
    }

    /// <summary>
    /// Verifies that executing <see cref="PanelViewModel.TogglePanelCommand"/>
    /// when the panel is hidden restores <see cref="PanelViewModel.Height"/>
    /// to <see cref="PanelViewModel.DefaultHeight"/>.
    /// </summary>
    [Fact]
    public void TogglePanelCommand_WhenHidden_RestoresHeightToDefault()
    {
        // Arrange
        var sut = new PanelViewModel(new OutputTabViewModel());
        sut.IsVisible = false;
        sut.Height = 0;

        // Act
        sut.TogglePanelCommand.Execute(null);

        // Assert
        sut.Height.Should().Be(PanelViewModel.DefaultHeight);
    }
}
