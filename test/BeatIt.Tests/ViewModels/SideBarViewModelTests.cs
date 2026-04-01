using BeatIt.ViewModels;
using FluentAssertions;
using Xunit;

namespace BeatIt.Tests.ViewModels;

/// <summary>
/// Unit tests for <see cref="SideBarViewModel"/>.
/// Verifies default property values, property change notifications,
/// clamping logic, and command execution.
/// </summary>
public sealed class SideBarViewModelTests
{
    [Fact]
    public void Constructor_SetsDefaultWidth()
    {
        // Arrange & Act
        var sut = new SideBarViewModel();

        // Assert
        sut.Width.Should().Be(250.0);
    }

    [Fact]
    public void Constructor_SetsSideBarContentToNull()
    {
        // Arrange & Act
        var sut = new SideBarViewModel();

        // Assert
        sut.SideBarContent.Should().BeNull();
    }

    [Fact]
    public void Constructor_SetsHasContentToFalse()
    {
        // Arrange & Act
        var sut = new SideBarViewModel();

        // Assert
        sut.HasContent.Should().BeFalse();
    }

    [Fact]
    public void Width_WhenChanged_RaisesPropertyChanged()
    {
        // Arrange
        var sut = new SideBarViewModel();
        using var monitor = sut.Monitor();

        // Act
        sut.Width = 300.0;

        // Assert
        monitor.Should().RaisePropertyChangeFor(s => s.Width);
    }

    [Fact]
    public void SideBarContent_WhenChanged_RaisesPropertyChangedForSideBarContent()
    {
        // Arrange
        var sut = new SideBarViewModel();
        using var monitor = sut.Monitor();
        var content = new TestContentViewModel();

        // Act
        sut.SideBarContent = content;

        // Assert
        monitor.Should().RaisePropertyChangeFor(s => s.SideBarContent);
    }

    [Fact]
    public void SideBarContent_WhenChanged_RaisesPropertyChangedForHasContent()
    {
        // Arrange
        var sut = new SideBarViewModel();
        using var monitor = sut.Monitor();
        var content = new TestContentViewModel();

        // Act
        sut.SideBarContent = content;

        // Assert
        monitor.Should().RaisePropertyChangeFor(s => s.HasContent);
    }

    [Fact]
    public void HasContent_WhenSideBarContentIsSet_ReturnsTrue()
    {
        // Arrange
        var sut = new SideBarViewModel();

        // Act
        sut.SideBarContent = new TestContentViewModel();

        // Assert
        sut.HasContent.Should().BeTrue();
    }

    [Fact]
    public void HasContent_WhenSideBarContentIsSetBackToNull_ReturnsFalse()
    {
        // Arrange
        var sut = new SideBarViewModel();
        sut.SideBarContent = new TestContentViewModel();

        // Act
        sut.SideBarContent = null;

        // Assert
        sut.HasContent.Should().BeFalse();
    }

    [Fact]
    public void OpenFolderCommand_CanExecute_DoesNotThrow()
    {
        // Arrange
        var sut = new SideBarViewModel();

        // Act
        var act = () => sut.OpenFolderCommand.Execute(null);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void ClampWidth_WhenProposedWidthBelowMinWidth_ReturnsZero()
    {
        // Arrange
        var proposedWidth = 100.0;
        var maxWidth = 500.0;

        // Act
        var result = SideBarViewModel.ClampWidth(proposedWidth, maxWidth);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void ClampWidth_WhenProposedWidthEqualsMinWidth_ReturnsMinWidth()
    {
        // Arrange
        var proposedWidth = SideBarViewModel.MinWidth;
        var maxWidth = 500.0;

        // Act
        var result = SideBarViewModel.ClampWidth(proposedWidth, maxWidth);

        // Assert
        result.Should().Be(SideBarViewModel.MinWidth);
    }

    [Fact]
    public void ClampWidth_WhenProposedWidthBetweenMinAndMax_ReturnsProposedWidth()
    {
        // Arrange
        var proposedWidth = 300.0;
        var maxWidth = 500.0;

        // Act
        var result = SideBarViewModel.ClampWidth(proposedWidth, maxWidth);

        // Assert
        result.Should().Be(300.0);
    }

    [Fact]
    public void ClampWidth_WhenProposedWidthExceedsMaxWidth_ReturnsMaxWidth()
    {
        // Arrange
        var proposedWidth = 600.0;
        var maxWidth = 500.0;

        // Act
        var result = SideBarViewModel.ClampWidth(proposedWidth, maxWidth);

        // Assert
        result.Should().Be(500.0);
    }

    [Fact]
    public void ClampWidth_WhenProposedWidthEqualsMaxWidth_ReturnsMaxWidth()
    {
        // Arrange
        var proposedWidth = 500.0;
        var maxWidth = 500.0;

        // Act
        var result = SideBarViewModel.ClampWidth(proposedWidth, maxWidth);

        // Assert
        result.Should().Be(500.0);
    }

    /// <summary>
    /// A test view model used as side bar content in tests.
    /// </summary>
    private sealed class TestContentViewModel : ViewModelBase
    {
    }
}
