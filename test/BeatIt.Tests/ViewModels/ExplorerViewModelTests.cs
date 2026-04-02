using BeatIt.Services;
using BeatIt.ViewModels;
using FluentAssertions;
using Moq;
using Xunit;

namespace BeatIt.Tests.ViewModels;

/// <summary>
/// Unit tests for <see cref="ExplorerViewModel"/>.
/// Verifies initialization, folder opening, and property change notifications.
/// </summary>
public sealed class ExplorerViewModelTests
{
    /// <summary>
    /// Verifies that RootNodes is empty after construction.
    /// </summary>
    [Fact]
    public void Constructor_InitializesEmptyRootNodes()
    {
        // Arrange & Act
        var sut = new ExplorerViewModel(
            Mock.Of<IFolderPickerService>(),
            Mock.Of<IFileSystemService>());

        // Assert
        sut.RootNodes.Should().BeEmpty();
    }

    /// <summary>
    /// Verifies that HasFolder is false after construction.
    /// </summary>
    [Fact]
    public void Constructor_HasFolderIsFalse()
    {
        // Arrange & Act
        var sut = new ExplorerViewModel(
            Mock.Of<IFolderPickerService>(),
            Mock.Of<IFileSystemService>());

        // Assert
        sut.HasFolder.Should().BeFalse();
    }

    /// <summary>
    /// Verifies that FolderName is null after construction.
    /// </summary>
    [Fact]
    public void Constructor_FolderNameIsNull()
    {
        // Arrange & Act
        var sut = new ExplorerViewModel(
            Mock.Of<IFolderPickerService>(),
            Mock.Of<IFileSystemService>());

        // Assert
        sut.FolderName.Should().BeNull();
    }

    /// <summary>
    /// Verifies that executing OpenFolderCommand when a folder is picked
    /// populates RootNodes with the returned file system entries.
    /// </summary>
    [Fact]
    public async Task OpenFolderCommand_WhenFolderPicked_PopulatesRootNodes()
    {
        // Arrange
        var mockPicker = new Mock<IFolderPickerService>();
        mockPicker.Setup(p => p.PickFolderAsync())
            .ReturnsAsync(@"C:\test\MyFolder");

        var mockFs = new Mock<IFileSystemService>();
        mockFs.Setup(fs => fs.GetEntriesAsync(@"C:\test\MyFolder"))
            .ReturnsAsync(new List<FileSystemEntry>
            {
                new("src", @"C:\test\MyFolder\src", IsDirectory: true, Extension: string.Empty),
                new("readme.md", @"C:\test\MyFolder\readme.md", IsDirectory: false, Extension: ".md"),
            });

        var sut = new ExplorerViewModel(mockPicker.Object, mockFs.Object);

        // Act
        await sut.OpenFolderCommand.ExecuteAsync(null);

        // Assert
        sut.RootNodes.Should().HaveCount(2);
        sut.RootNodes[0].Name.Should().Be("src");
        sut.RootNodes[0].IsDirectory.Should().BeTrue();
        sut.RootNodes[1].Name.Should().Be("readme.md");
        sut.RootNodes[1].IsDirectory.Should().BeFalse();
    }

    /// <summary>
    /// Verifies that when the folder picker is cancelled (returns null),
    /// the state remains unchanged.
    /// </summary>
    [Fact]
    public async Task OpenFolderCommand_WhenCancelled_DoesNotChangeState()
    {
        // Arrange
        var mockPicker = new Mock<IFolderPickerService>();
        mockPicker.Setup(p => p.PickFolderAsync())
            .ReturnsAsync((string?)null);

        var sut = new ExplorerViewModel(
            mockPicker.Object,
            Mock.Of<IFileSystemService>());

        // Act
        await sut.OpenFolderCommand.ExecuteAsync(null);

        // Assert
        sut.RootNodes.Should().BeEmpty();
        sut.FolderName.Should().BeNull();
        sut.HasFolder.Should().BeFalse();
    }

    /// <summary>
    /// Verifies that FolderName is set to just the folder name (not the full path).
    /// </summary>
    [Fact]
    public async Task OpenFolderCommand_SetsFolderNameToFolderName()
    {
        // Arrange
        var mockPicker = new Mock<IFolderPickerService>();
        mockPicker.Setup(p => p.PickFolderAsync())
            .ReturnsAsync(@"C:\test\MyFolder");

        var mockFs = new Mock<IFileSystemService>();
        mockFs.Setup(fs => fs.GetEntriesAsync(@"C:\test\MyFolder"))
            .ReturnsAsync(Array.Empty<FileSystemEntry>());

        var sut = new ExplorerViewModel(mockPicker.Object, mockFs.Object);

        // Act
        await sut.OpenFolderCommand.ExecuteAsync(null);

        // Assert
        sut.FolderName.Should().Be("MyFolder");
    }

    /// <summary>
    /// Verifies that FolderName is correctly extracted even when the path has a trailing directory separator.
    /// </summary>
    [Fact]
    public async Task OpenFolderCommand_WhenPathHasTrailingSeparator_SetsFolderNameCorrectly()
    {
        // Arrange
        var mockPicker = new Mock<IFolderPickerService>();
        mockPicker.Setup(p => p.PickFolderAsync())
            .ReturnsAsync(@"C:\test\MyFolder\");

        var mockFs = new Mock<IFileSystemService>();
        mockFs.Setup(fs => fs.GetEntriesAsync(@"C:\test\MyFolder\"))
            .ReturnsAsync(Array.Empty<FileSystemEntry>());

        var sut = new ExplorerViewModel(mockPicker.Object, mockFs.Object);

        // Act
        await sut.OpenFolderCommand.ExecuteAsync(null);

        // Assert
        sut.FolderName.Should().Be("MyFolder");
    }

    /// <summary>
    /// Verifies that HasFolder is true after opening a folder.
    /// </summary>
    [Fact]
    public async Task OpenFolderCommand_SetsHasFolderTrue()
    {
        // Arrange
        var mockPicker = new Mock<IFolderPickerService>();
        mockPicker.Setup(p => p.PickFolderAsync())
            .ReturnsAsync(@"C:\test\MyFolder");

        var mockFs = new Mock<IFileSystemService>();
        mockFs.Setup(fs => fs.GetEntriesAsync(@"C:\test\MyFolder"))
            .ReturnsAsync(Array.Empty<FileSystemEntry>());

        var sut = new ExplorerViewModel(mockPicker.Object, mockFs.Object);

        // Act
        await sut.OpenFolderCommand.ExecuteAsync(null);

        // Assert
        sut.HasFolder.Should().BeTrue();
    }

    /// <summary>
    /// Verifies that changing FolderName raises PropertyChanged for FolderName.
    /// </summary>
    [Fact]
    public async Task FolderName_WhenChanged_RaisesPropertyChanged()
    {
        // Arrange
        var mockPicker = new Mock<IFolderPickerService>();
        mockPicker.Setup(p => p.PickFolderAsync())
            .ReturnsAsync(@"C:\test\MyFolder");

        var mockFs = new Mock<IFileSystemService>();
        mockFs.Setup(fs => fs.GetEntriesAsync(@"C:\test\MyFolder"))
            .ReturnsAsync(Array.Empty<FileSystemEntry>());

        var sut = new ExplorerViewModel(mockPicker.Object, mockFs.Object);
        using var monitor = sut.Monitor();

        // Act
        await sut.OpenFolderCommand.ExecuteAsync(null);

        // Assert
        monitor.Should().RaisePropertyChangeFor(e => e.FolderName);
    }

    /// <summary>
    /// Verifies that changing FolderName also raises PropertyChanged for HasFolder.
    /// </summary>
    [Fact]
    public async Task FolderName_WhenChanged_RaisesPropertyChangedForHasFolder()
    {
        // Arrange
        var mockPicker = new Mock<IFolderPickerService>();
        mockPicker.Setup(p => p.PickFolderAsync())
            .ReturnsAsync(@"C:\test\MyFolder");

        var mockFs = new Mock<IFileSystemService>();
        mockFs.Setup(fs => fs.GetEntriesAsync(@"C:\test\MyFolder"))
            .ReturnsAsync(Array.Empty<FileSystemEntry>());

        var sut = new ExplorerViewModel(mockPicker.Object, mockFs.Object);
        using var monitor = sut.Monitor();

        // Act
        await sut.OpenFolderCommand.ExecuteAsync(null);

        // Assert
        monitor.Should().RaisePropertyChangeFor(e => e.HasFolder);
    }
}
