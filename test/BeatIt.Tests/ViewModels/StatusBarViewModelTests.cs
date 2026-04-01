using System.ComponentModel;
using BeatIt.Services;
using BeatIt.ViewModels;
using FluentAssertions;
using Moq;
using Xunit;

namespace BeatIt.Tests.ViewModels;

/// <summary>
/// Unit tests for <see cref="StatusBarViewModel"/>.
/// Verifies that the view model reflects the status bar service state
/// and properly manages event subscriptions.
/// </summary>
public sealed class StatusBarViewModelTests
{
    private readonly Mock<IStatusBarService> _mockStatusBarService;

    /// <summary>
    /// Initializes test fixtures with a mocked <see cref="IStatusBarService"/>.
    /// </summary>
    public StatusBarViewModelTests()
    {
        _mockStatusBarService = new Mock<IStatusBarService>();
        _mockStatusBarService.Setup(s => s.StatusText).Returns("Ready");
    }

    [Fact]
    public void Constructor_SetsStatusTextFromService()
    {
        // Arrange & Act
        var sut = new StatusBarViewModel(_mockStatusBarService.Object);

        // Assert
        sut.StatusText.Should().Be("Ready");
    }

    [Fact]
    public void StatusText_UpdatesWhenServiceStatusTextChanges()
    {
        // Arrange
        var sut = new StatusBarViewModel(_mockStatusBarService.Object);

        // Act
        _mockStatusBarService.Setup(s => s.StatusText).Returns("Building...");
        _mockStatusBarService.Raise(
            s => s.PropertyChanged += null,
            new PropertyChangedEventArgs(nameof(IStatusBarService.StatusText)));

        // Assert
        sut.StatusText.Should().Be("Building...");
    }

    [Fact]
    public void StatusText_RaisesPropertyChanged_WhenServiceStatusTextChanges()
    {
        // Arrange
        var sut = new StatusBarViewModel(_mockStatusBarService.Object);
        var raised = false;
        sut.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(StatusBarViewModel.StatusText))
            {
                raised = true;
            }
        };

        // Act
        _mockStatusBarService.Setup(s => s.StatusText).Returns("Error");
        _mockStatusBarService.Raise(
            s => s.PropertyChanged += null,
            new PropertyChangedEventArgs(nameof(IStatusBarService.StatusText)));

        // Assert
        raised.Should().BeTrue();
    }

    [Fact]
    public void ServicePropertyChanged_OtherProperty_DoesNotUpdateStatusText()
    {
        // Arrange
        var sut = new StatusBarViewModel(_mockStatusBarService.Object);

        // Act
        _mockStatusBarService.Raise(
            s => s.PropertyChanged += null,
            new PropertyChangedEventArgs("SomeOtherProperty"));

        // Assert
        sut.StatusText.Should().Be("Ready");
    }

    [Fact]
    public void Dispose_UnsubscribesFromServicePropertyChanged()
    {
        // Arrange
        var sut = new StatusBarViewModel(_mockStatusBarService.Object);
        sut.Dispose();

        // Act
        _mockStatusBarService.Setup(s => s.StatusText).Returns("After dispose");
        _mockStatusBarService.Raise(
            s => s.PropertyChanged += null,
            new PropertyChangedEventArgs(nameof(IStatusBarService.StatusText)));

        // Assert
        sut.StatusText.Should().Be("Ready");
    }
}
