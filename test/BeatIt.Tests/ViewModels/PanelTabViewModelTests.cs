using BeatIt.ViewModels;
using FluentAssertions;
using Xunit;

namespace BeatIt.Tests.ViewModels;

/// <summary>
/// Unit tests for <see cref="PanelTabViewModel"/>.
/// Uses a concrete test subclass to verify the abstract base class behavior
/// including property initialization and change notifications.
/// </summary>
public sealed class PanelTabViewModelTests
{
    [Fact]
    public void Constructor_SetsTitle()
    {
        // Arrange & Act
        var sut = new TestPanelTab("Problems");

        // Assert
        sut.Title.Should().Be("Problems");
    }

    [Fact]
    public void Title_WhenChanged_RaisesPropertyChanged()
    {
        // Arrange
        var sut = new TestPanelTab("Original");
        using var monitor = sut.Monitor();

        // Act
        sut.Title = "Updated";

        // Assert
        monitor.Should().RaisePropertyChangeFor(s => s.Title);
    }

    [Fact]
    public void Title_WhenSetToSameValue_DoesNotRaisePropertyChanged()
    {
        // Arrange
        var sut = new TestPanelTab("Same");
        using var monitor = sut.Monitor();

        // Act
        sut.Title = "Same";

        // Assert
        monitor.Should().NotRaisePropertyChangeFor(s => s.Title);
    }

    /// <summary>
    /// Concrete implementation of <see cref="PanelTabViewModel"/> used exclusively for testing.
    /// </summary>
    private sealed class TestPanelTab : PanelTabViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestPanelTab"/> class.
        /// </summary>
        /// <param name="title">
        /// The display title of this tab.
        /// </param>
        public TestPanelTab(string title)
            : base(title)
        {
        }
    }
}
