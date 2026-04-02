using BeatIt.Services;
using BeatIt.ViewModels;
using FluentAssertions;
using Moq;
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
        var sut = new SideBarViewModel(CreateExplorerViewModel());

        // Assert
        sut.Width.Should().Be(250.0);
    }

    [Fact]
    public void Constructor_SetsSideBarContentToNull()
    {
        // Arrange & Act
        var sut = new SideBarViewModel(CreateExplorerViewModel());

        // Assert
        sut.SideBarContent.Should().BeNull();
    }

    [Fact]
    public void Constructor_SetsHasContentToFalse()
    {
        // Arrange & Act
        var sut = new SideBarViewModel(CreateExplorerViewModel());

        // Assert
        sut.HasContent.Should().BeFalse();
    }

    [Fact]
    public void Width_WhenChanged_RaisesPropertyChanged()
    {
        // Arrange
        var sut = new SideBarViewModel(CreateExplorerViewModel());
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
        var sut = new SideBarViewModel(CreateExplorerViewModel());
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
        var sut = new SideBarViewModel(CreateExplorerViewModel());
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
        var sut = new SideBarViewModel(CreateExplorerViewModel());

        // Act
        sut.SideBarContent = new TestContentViewModel();

        // Assert
        sut.HasContent.Should().BeTrue();
    }

    [Fact]
    public void HasContent_WhenSideBarContentIsSetBackToNull_ReturnsFalse()
    {
        // Arrange
        var sut = new SideBarViewModel(CreateExplorerViewModel());
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
        var sut = new SideBarViewModel(CreateExplorerViewModel());

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
    /// Verifies that the OpenFolderCommand delegates to the ExplorerViewModel's OpenFolderCommand.
    /// </summary>
    [Fact]
    public void OpenFolderCommand_DelegatesToExplorerViewModel()
    {
        // Arrange
        var explorer = CreateExplorerViewModel();
        var sut = new SideBarViewModel(explorer);

        // Act & Assert
        sut.OpenFolderCommand.Should().BeSameAs(explorer.OpenFolderCommand);
    }

    /// <summary>
    /// Verifies that when the explorer's FolderName changes to a non-empty value,
    /// the SideBarContent is set to the explorer view model.
    /// </summary>
    [Fact]
    public async Task ExplorerFolderNameChanged_SetsSideBarContentToExplorer()
    {
        // Arrange
        var mockPicker = new Mock<IFolderPickerService>();
        mockPicker.Setup(p => p.PickFolderAsync())
            .ReturnsAsync(@"C:\test\MyFolder");

        var mockFs = new Mock<IFileSystemService>();
        mockFs.Setup(fs => fs.GetEntriesAsync(@"C:\test\MyFolder"))
            .ReturnsAsync(Array.Empty<FileSystemEntry>());

        var explorer = new ExplorerViewModel(mockPicker.Object, mockFs.Object);
        var sut = new SideBarViewModel(explorer);

        // Act
        await explorer.OpenFolderCommand.ExecuteAsync(null);

        // Assert
        sut.SideBarContent.Should().BeSameAs(explorer);
    }

    /// <summary>
    /// Verifies that closing the explorer folder clears the side bar content.
    /// </summary>
    [Fact]
    public async Task ExplorerFolderClosed_ClearsSideBarContent()
    {
        // Arrange
        var mockPicker = new Mock<IFolderPickerService>();
        mockPicker.Setup(p => p.PickFolderAsync())
            .ReturnsAsync(@"C:\test\MyFolder");

        var mockFs = new Mock<IFileSystemService>();
        mockFs.Setup(fs => fs.GetEntriesAsync(@"C:\test\MyFolder"))
            .ReturnsAsync(Array.Empty<FileSystemEntry>());

        var explorer = new ExplorerViewModel(mockPicker.Object, mockFs.Object);
        var sut = new SideBarViewModel(explorer);

        await explorer.OpenFolderCommand.ExecuteAsync(null);
        sut.SideBarContent.Should().BeSameAs(explorer);

        // Act
        explorer.CloseFolderCommand.Execute(null);

        // Assert
        sut.SideBarContent.Should().BeNull();
        sut.HasContent.Should().BeFalse();
    }

    /// <summary>
    /// Verifies that closing a folder does not affect side bar visibility
    /// (no interaction with ActivityBarViewModel).
    /// </summary>
    [Fact]
    public async Task CloseFolderCommand_DoesNotAffectSideBarVisibility()
    {
        // Arrange
        var mockPicker = new Mock<IFolderPickerService>();
        mockPicker.Setup(p => p.PickFolderAsync())
            .ReturnsAsync(@"C:\test\MyFolder");

        var mockFs = new Mock<IFileSystemService>();
        mockFs.Setup(fs => fs.GetEntriesAsync(@"C:\test\MyFolder"))
            .ReturnsAsync(Array.Empty<FileSystemEntry>());

        var explorer = new ExplorerViewModel(mockPicker.Object, mockFs.Object);
        var sut = new SideBarViewModel(explorer);

        await explorer.OpenFolderCommand.ExecuteAsync(null);

        // Act
        explorer.CloseFolderCommand.Execute(null);

        // Assert — only SideBarContent is affected, not visibility
        sut.SideBarContent.Should().BeNull();
        sut.Width.Should().Be(SideBarViewModel.DefaultWidth);
    }

    /// <summary>
    /// Creates a default <see cref="ExplorerViewModel"/> with mocked dependencies.
    /// </summary>
    /// <returns>
    /// An <see cref="ExplorerViewModel"/> configured with <see cref="Moq.Mock"/> services.
    /// </returns>
    private static ExplorerViewModel CreateExplorerViewModel()
    {
        return new ExplorerViewModel(
            Mock.Of<IFolderPickerService>(),
            Mock.Of<IFileSystemService>());
    }

    /// <summary>
    /// A test view model used as side bar content in tests.
    /// </summary>
    private sealed class TestContentViewModel : ViewModelBase
    {
    }
}
