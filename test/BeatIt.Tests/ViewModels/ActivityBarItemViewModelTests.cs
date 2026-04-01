using BeatIt.ViewModels;
using FluentAssertions;
using Xunit;

namespace BeatIt.Tests.ViewModels;

/// <summary>
/// Unit tests for <see cref="ActivityBarItemViewModel"/>.
/// Verifies property defaults and property change notifications.
/// </summary>
public sealed class ActivityBarItemViewModelTests
{
    [Fact]
    public void Constructor_SetsIconGlyph()
    {
        // Arrange & Act
        var sut = new ActivityBarItemViewModel("\ueaf0", "Explorer");

        // Assert
        sut.IconGlyph.Should().Be("\ueaf0");
    }

    [Fact]
    public void Constructor_SetsLabel()
    {
        // Arrange & Act
        var sut = new ActivityBarItemViewModel("\ueaf0", "Explorer");

        // Assert
        sut.Label.Should().Be("Explorer");
    }

    [Fact]
    public void IsSelected_DefaultIsFalse()
    {
        // Arrange & Act
        var sut = new ActivityBarItemViewModel("\ueaf0", "Explorer");

        // Assert
        sut.IsSelected.Should().BeFalse();
    }

    [Fact]
    public void IsSelected_RaisesPropertyChanged()
    {
        // Arrange
        var sut = new ActivityBarItemViewModel("\ueaf0", "Explorer");
        var raised = false;
        sut.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(ActivityBarItemViewModel.IsSelected))
            {
                raised = true;
            }
        };

        // Act
        sut.IsSelected = true;

        // Assert
        raised.Should().BeTrue();
    }

    [Fact]
    public void IconGlyph_RaisesPropertyChanged()
    {
        // Arrange
        var sut = new ActivityBarItemViewModel("\ueaf0", "Explorer");
        var raised = false;
        sut.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(ActivityBarItemViewModel.IconGlyph))
            {
                raised = true;
            }
        };

        // Act
        sut.IconGlyph = "\uea6d";

        // Assert
        raised.Should().BeTrue();
    }

    [Fact]
    public void Label_RaisesPropertyChanged()
    {
        // Arrange
        var sut = new ActivityBarItemViewModel("\ueaf0", "Explorer");
        var raised = false;
        sut.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(ActivityBarItemViewModel.Label))
            {
                raised = true;
            }
        };

        // Act
        sut.Label = "Search";

        // Assert
        raised.Should().BeTrue();
    }
}
