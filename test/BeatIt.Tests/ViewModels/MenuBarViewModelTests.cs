using BeatIt.Services;
using BeatIt.ViewModels;
using FluentAssertions;
using Moq;
using Xunit;

namespace BeatIt.Tests.ViewModels;

/// <summary>
/// Unit tests for <see cref="MenuBarViewModel"/>.
/// Verifies that placeholder menu bar commands are properly generated
/// and executable without errors.
/// </summary>
public sealed class MenuBarViewModelTests
{
    [Fact]
    public void Constructor_CreatesInstance()
    {
        // Act
        var sut = new MenuBarViewModel(CreateExplorerViewModel());

        // Assert
        sut.Should().NotBeNull();
    }

    [Fact]
    public void OpenFolderCommand_IsNotNull()
    {
        // Arrange
        var sut = new MenuBarViewModel(CreateExplorerViewModel());

        // Act & Assert
        sut.OpenFolderCommand.Should().NotBeNull();
    }

    [Fact]
    public void CloseFolderCommand_IsNotNull()
    {
        // Arrange
        var sut = new MenuBarViewModel(CreateExplorerViewModel());

        // Act & Assert
        sut.CloseFolderCommand.Should().NotBeNull();
    }

    [Fact]
    public void OpenFolderCommand_CanExecute_ReturnsTrue()
    {
        // Arrange
        var sut = new MenuBarViewModel(CreateExplorerViewModel());

        // Act & Assert
        sut.OpenFolderCommand.CanExecute(null).Should().BeTrue();
    }

    [Fact]
    public void CloseFolderCommand_CanExecute_ReturnsTrue()
    {
        // Arrange
        var sut = new MenuBarViewModel(CreateExplorerViewModel());

        // Act & Assert
        sut.CloseFolderCommand.CanExecute(null).Should().BeTrue();
    }

    [Fact]
    public void OpenFolderCommand_Execute_DoesNotThrow()
    {
        // Arrange
        var sut = new MenuBarViewModel(CreateExplorerViewModel());

        // Act
        var act = () => sut.OpenFolderCommand.Execute(null);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void CloseFolderCommand_Execute_DoesNotThrow()
    {
        // Arrange
        var sut = new MenuBarViewModel(CreateExplorerViewModel());

        // Act
        var act = () => sut.CloseFolderCommand.Execute(null);

        // Assert
        act.Should().NotThrow();
    }

    /// <summary>
    /// Verifies that the OpenFolderCommand delegates to the ExplorerViewModel's OpenFolderCommand.
    /// </summary>
    [Fact]
    public void OpenFolderCommand_DelegatesToExplorerViewModel()
    {
        // Arrange
        var explorer = CreateExplorerViewModel();
        var sut = new MenuBarViewModel(explorer);

        // Act & Assert
        sut.OpenFolderCommand.Should().BeSameAs(explorer.OpenFolderCommand);
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
}
