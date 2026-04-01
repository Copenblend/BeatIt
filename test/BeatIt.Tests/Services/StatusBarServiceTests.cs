using BeatIt.Services;
using FluentAssertions;
using Xunit;

namespace BeatIt.Tests.Services;

/// <summary>
/// Unit tests for <see cref="StatusBarService"/>.
/// Verifies default state, property change notification, and value updates.
/// </summary>
public sealed class StatusBarServiceTests
{
    [Fact]
    public void StatusText_DefaultValue_IsReady()
    {
        // Arrange & Act
        var sut = new StatusBarService();

        // Assert
        sut.StatusText.Should().Be("Ready");
    }

    [Fact]
    public void StatusText_Set_UpdatesValue()
    {
        // Arrange
        var sut = new StatusBarService();

        // Act
        sut.StatusText = "Building...";

        // Assert
        sut.StatusText.Should().Be("Building...");
    }

    [Fact]
    public void StatusText_SetNewValue_RaisesPropertyChanged()
    {
        // Arrange
        var sut = new StatusBarService();
        string? changedProperty = null;
        sut.PropertyChanged += (_, e) => changedProperty = e.PropertyName;

        // Act
        sut.StatusText = "Building...";

        // Assert
        changedProperty.Should().Be(nameof(StatusBarService.StatusText));
    }

    [Fact]
    public void StatusText_SetSameValue_DoesNotRaisePropertyChanged()
    {
        // Arrange
        var sut = new StatusBarService();
        var raised = false;
        sut.PropertyChanged += (_, _) => raised = true;

        // Act
        sut.StatusText = "Ready";

        // Assert
        raised.Should().BeFalse();
    }
}
