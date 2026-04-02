using BeatIt.Services;
using BeatIt.ViewModels;
using FluentAssertions;
using Moq;
using Xunit;

namespace BeatIt.Tests.ViewModels;

/// <summary>
/// Unit tests for <see cref="FileTreeNodeViewModel"/>.
/// Verifies constructor behavior, lazy-loading of children, and property values.
/// </summary>
public sealed class FileTreeNodeViewModelTests
{
    /// <summary>
    /// Verifies that a directory node has a single placeholder child named "Loading...".
    /// </summary>
    [Fact]
    public void Constructor_ForDirectory_AddsPlaceholderChild()
    {
        // Arrange & Act
        var sut = new FileTreeNodeViewModel(
            Mock.Of<IFileSystemService>(),
            "src",
            @"C:\project\src",
            isDirectory: true,
            extension: string.Empty);

        // Assert
        sut.Children.Should().ContainSingle()
            .Which.Name.Should().Be("Loading...");
    }

    /// <summary>
    /// Verifies that a file node has no children.
    /// </summary>
    [Fact]
    public void Constructor_ForFile_HasNoChildren()
    {
        // Arrange & Act
        var sut = new FileTreeNodeViewModel(
            Mock.Of<IFileSystemService>(),
            "readme.md",
            @"C:\project\readme.md",
            isDirectory: false,
            extension: ".md");

        // Assert
        sut.Children.Should().BeEmpty();
    }

    /// <summary>
    /// Verifies that constructor correctly sets Name, FullPath, IsDirectory, and Extension.
    /// </summary>
    [Fact]
    public void Constructor_SetsProperties()
    {
        // Arrange & Act
        var sut = new FileTreeNodeViewModel(
            Mock.Of<IFileSystemService>(),
            "readme.md",
            @"C:\project\readme.md",
            isDirectory: false,
            extension: ".md");

        // Assert
        sut.Name.Should().Be("readme.md");
        sut.FullPath.Should().Be(@"C:\project\readme.md");
        sut.IsDirectory.Should().BeFalse();
        sut.Extension.Should().Be(".md");
    }

    /// <summary>
    /// Verifies that IsLoaded defaults to false.
    /// </summary>
    [Fact]
    public void IsLoaded_DefaultIsFalse()
    {
        // Arrange & Act
        var sut = new FileTreeNodeViewModel(
            Mock.Of<IFileSystemService>(),
            "src",
            @"C:\project\src",
            isDirectory: true,
            extension: string.Empty);

        // Assert
        sut.IsLoaded.Should().BeFalse();
    }

    /// <summary>
    /// Verifies that setting IsExpanded to true on a directory triggers lazy-loading
    /// of child entries from the file system service.
    /// </summary>
    [Fact]
    public async Task IsExpanded_SetTrue_OnDirectory_LoadsChildren()
    {
        // Arrange
        var mockFs = new Mock<IFileSystemService>();
        mockFs.Setup(fs => fs.GetEntriesAsync(@"C:\project\src"))
            .ReturnsAsync(new List<FileSystemEntry>
            {
                new("Controllers", @"C:\project\src\Controllers", IsDirectory: true, Extension: string.Empty),
                new("Program.cs", @"C:\project\src\Program.cs", IsDirectory: false, Extension: ".cs"),
            });

        var sut = new FileTreeNodeViewModel(
            mockFs.Object,
            "src",
            @"C:\project\src",
            isDirectory: true,
            extension: string.Empty);

        // Act
        sut.IsExpanded = true;

        // Allow the fire-and-forget task to complete.
        await Task.Delay(200);

        // Assert
        sut.IsLoaded.Should().BeTrue();
        sut.Children.Should().HaveCount(2);
        sut.Children[0].Name.Should().Be("Controllers");
        sut.Children[0].IsDirectory.Should().BeTrue();
        sut.Children[1].Name.Should().Be("Program.cs");
        sut.Children[1].IsDirectory.Should().BeFalse();
    }

    /// <summary>
    /// Verifies that setting IsExpanded to true on a file node does not trigger loading.
    /// </summary>
    [Fact]
    public void IsExpanded_SetTrue_OnFile_DoesNotLoadChildren()
    {
        // Arrange
        var mockFs = new Mock<IFileSystemService>();
        var sut = new FileTreeNodeViewModel(
            mockFs.Object,
            "readme.md",
            @"C:\project\readme.md",
            isDirectory: false,
            extension: ".md");

        // Act
        sut.IsExpanded = true;

        // Assert
        sut.Children.Should().BeEmpty();
        sut.IsLoaded.Should().BeFalse();
        mockFs.Verify(fs => fs.GetEntriesAsync(It.IsAny<string>()), Times.Never);
    }

    /// <summary>
    /// Verifies that expanding an already-loaded directory does not trigger a second load.
    /// </summary>
    [Fact]
    public async Task IsExpanded_SetTrue_WhenAlreadyLoaded_DoesNotLoadAgain()
    {
        // Arrange
        var mockFs = new Mock<IFileSystemService>();
        mockFs.Setup(fs => fs.GetEntriesAsync(@"C:\project\src"))
            .ReturnsAsync(new List<FileSystemEntry>
            {
                new("file.cs", @"C:\project\src\file.cs", IsDirectory: false, Extension: ".cs"),
            });

        var sut = new FileTreeNodeViewModel(
            mockFs.Object,
            "src",
            @"C:\project\src",
            isDirectory: true,
            extension: string.Empty);

        // Load once.
        sut.IsExpanded = true;
        await Task.Delay(200);
        sut.IsLoaded.Should().BeTrue();

        // Act — collapse and re-expand.
        sut.IsExpanded = false;
        sut.IsExpanded = true;

        await Task.Delay(200);

        // Assert — service called only once.
        mockFs.Verify(fs => fs.GetEntriesAsync(@"C:\project\src"), Times.Once);
    }

    /// <summary>
    /// Verifies that <see cref="FileTreeNodeViewModel.LoadChildrenAsync"/>
    /// clears the placeholder and populates real entries.
    /// </summary>
    [Fact]
    public async Task LoadChildrenAsync_ClearsPlaceholderAndAddsEntries()
    {
        // Arrange
        var mockFs = new Mock<IFileSystemService>();
        mockFs.Setup(fs => fs.GetEntriesAsync(@"C:\project\src"))
            .ReturnsAsync(new List<FileSystemEntry>
            {
                new("Models", @"C:\project\src\Models", IsDirectory: true, Extension: string.Empty),
                new("App.cs", @"C:\project\src\App.cs", IsDirectory: false, Extension: ".cs"),
            });

        var sut = new FileTreeNodeViewModel(
            mockFs.Object,
            "src",
            @"C:\project\src",
            isDirectory: true,
            extension: string.Empty);

        sut.Children.Should().ContainSingle(c => c.Name == "Loading...");

        // Act
        await sut.LoadChildrenAsync();

        // Assert
        sut.IsLoaded.Should().BeTrue();
        sut.Children.Should().HaveCount(2);
        sut.Children[0].Name.Should().Be("Models");
        sut.Children[1].Name.Should().Be("App.cs");
    }
}
