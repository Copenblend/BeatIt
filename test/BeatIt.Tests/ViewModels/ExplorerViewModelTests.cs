using BeatIt.Services;
using BeatIt.ViewModels;
using FluentAssertions;
using Moq;
using Serilog;
using Serilog.Core;
using Serilog.Events;
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

    /// <summary>
    /// Verifies that CloseFolderCommand clears RootNodes, FolderName, and sets HasFolder to false.
    /// </summary>
    [Fact]
    public async Task CloseFolderCommand_ClearsRootNodesAndFolderName()
    {
        // Arrange
        var (sut, _, _) = CreateOpenedExplorer();
        await sut.OpenFolderCommand.ExecuteAsync(null);

        // Act
        sut.CloseFolderCommand.Execute(null);

        // Assert
        sut.RootNodes.Should().BeEmpty();
        sut.FolderName.Should().BeNull();
        sut.HasFolder.Should().BeFalse();
    }

    /// <summary>
    /// Verifies that CloseFolderCommand.CanExecute returns false when no folder is open.
    /// </summary>
    [Fact]
    public void CloseFolderCommand_CanExecute_ReturnsFalseWhenNoFolder()
    {
        // Arrange
        var sut = new ExplorerViewModel(
            Mock.Of<IFolderPickerService>(),
            Mock.Of<IFileSystemService>());

        // Act & Assert
        sut.CloseFolderCommand.CanExecute(null).Should().BeFalse();
    }

    /// <summary>
    /// Verifies that CloseFolderCommand.CanExecute returns true when a folder is open.
    /// </summary>
    [Fact]
    public async Task CloseFolderCommand_CanExecute_ReturnsTrueWhenFolderIsOpen()
    {
        // Arrange
        var (sut, _, _) = CreateOpenedExplorer();
        await sut.OpenFolderCommand.ExecuteAsync(null);

        // Act & Assert
        sut.CloseFolderCommand.CanExecute(null).Should().BeTrue();
    }

    /// <summary>
    /// Verifies that closing a folder logs the expected closure message.
    /// </summary>
    [Fact]
    public async Task CloseFolderCommand_LogsClosureMessage()
    {
        // Arrange
        var events = new List<LogEvent>();
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Sink(new DelegateSink(e => events.Add(e)))
            .CreateLogger();

        try
        {
            var (sut, _, _) = CreateOpenedExplorer();
            await sut.OpenFolderCommand.ExecuteAsync(null);

            // Act
            sut.CloseFolderCommand.Execute(null);

            // Assert
            events.Should().Contain(e =>
                e.MessageTemplate.Text == "Folder {FolderName} closed by user."
                && e.Properties.ContainsKey("FolderName"));
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    /// <summary>
    /// Verifies that selecting a file node logs the expected click message.
    /// </summary>
    [Fact]
    public void SelectedNode_WhenFileClicked_LogsClickMessage()
    {
        // Arrange
        var events = new List<LogEvent>();
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Sink(new DelegateSink(e => events.Add(e)))
            .CreateLogger();

        try
        {
            var sut = new ExplorerViewModel(
                Mock.Of<IFolderPickerService>(),
                Mock.Of<IFileSystemService>());

            var fileNode = new FileTreeNodeViewModel(
                Mock.Of<IFileSystemService>(),
                "readme",
                @"C:\test\readme.md",
                isDirectory: false,
                extension: ".md");

            // Act
            sut.SelectedNode = fileNode;

            // Assert
            events.Should().Contain(e =>
                e.MessageTemplate.Text == "User clicked {FileName}.{FileExtension}"
                && e.Properties.ContainsKey("FileName")
                && e.Properties.ContainsKey("FileExtension"));
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    /// <summary>
    /// Verifies that selecting a directory node toggles its IsExpanded state.
    /// </summary>
    [Fact]
    public void SelectedNode_WhenDirectoryClicked_TogglesIsExpanded()
    {
        // Arrange
        var sut = new ExplorerViewModel(
            Mock.Of<IFolderPickerService>(),
            Mock.Of<IFileSystemService>());

        var mockFs = new Mock<IFileSystemService>();
        mockFs.Setup(fs => fs.GetEntriesAsync(It.IsAny<string>()))
            .ReturnsAsync(Array.Empty<FileSystemEntry>());

        var dirNode = new FileTreeNodeViewModel(
            mockFs.Object,
            "src",
            @"C:\test\src",
            isDirectory: true,
            extension: string.Empty);

        dirNode.IsExpanded.Should().BeFalse();

        // Act — first click expands
        sut.SelectedNode = dirNode;

        // Assert
        dirNode.IsExpanded.Should().BeTrue();

        // Act — second click collapses
        sut.SelectedNode = dirNode;

        // Assert
        dirNode.IsExpanded.Should().BeFalse();
    }

    /// <summary>
    /// Verifies that setting SelectedNode to null does not throw or log.
    /// </summary>
    [Fact]
    public void SelectedNode_WhenSetToNull_DoesNothing()
    {
        // Arrange
        var events = new List<LogEvent>();
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Sink(new DelegateSink(e => events.Add(e)))
            .CreateLogger();

        try
        {
            var sut = new ExplorerViewModel(
                Mock.Of<IFolderPickerService>(),
                Mock.Of<IFileSystemService>());

            // Act
            var act = () => sut.SelectedNode = null;

            // Assert
            act.Should().NotThrow();
            events.Should().BeEmpty();
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    /// <summary>
    /// Verifies that SelectedNode resets to null after processing a file click.
    /// </summary>
    [Fact]
    public void SelectedNode_ResetsToNullAfterProcessing()
    {
        // Arrange
        var sut = new ExplorerViewModel(
            Mock.Of<IFolderPickerService>(),
            Mock.Of<IFileSystemService>());

        var fileNode = new FileTreeNodeViewModel(
            Mock.Of<IFileSystemService>(),
            "readme",
            @"C:\test\readme.md",
            isDirectory: false,
            extension: ".md");

        // Act
        sut.SelectedNode = fileNode;

        // Assert
        sut.SelectedNode.Should().BeNull();
    }

    /// <summary>
    /// Verifies that CloseFolderCommand is a no-op when no folder is open.
    /// </summary>
    [Fact]
    public void CloseFolderCommand_WhenNoFolderOpen_IsNoOp()
    {
        // Arrange
        var sut = new ExplorerViewModel(
            Mock.Of<IFolderPickerService>(),
            Mock.Of<IFileSystemService>());

        // Act & Assert
        sut.CloseFolderCommand.CanExecute(null).Should().BeFalse();
    }

    /// <summary>
    /// Creates an <see cref="ExplorerViewModel"/> with mocks configured to simulate
    /// a successfully opened folder named "MyFolder" with two entries.
    /// </summary>
    /// <returns>
    /// A tuple of the view model, the folder picker mock, and the file system mock.
    /// </returns>
    private static (ExplorerViewModel Sut, Mock<IFolderPickerService> Picker, Mock<IFileSystemService> Fs) CreateOpenedExplorer()
    {
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
        return (sut, mockPicker, mockFs);
    }

    /// <summary>
    /// A minimal Serilog sink that delegates log events to an <see cref="Action{LogEvent}"/>.
    /// </summary>
    private sealed class DelegateSink : ILogEventSink
    {
        private readonly Action<LogEvent> _action;

        public DelegateSink(Action<LogEvent> action) => _action = action;

        public void Emit(LogEvent logEvent) => _action(logEvent);
    }
}
