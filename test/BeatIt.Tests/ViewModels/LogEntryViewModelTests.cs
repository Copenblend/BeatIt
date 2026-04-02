using BeatIt.ViewModels;
using FluentAssertions;
using Xunit;

namespace BeatIt.Tests.ViewModels;

/// <summary>
/// Unit tests for <see cref="LogEntryViewModel"/>.
/// Verifies that the constructor sets all read-only properties correctly
/// for every log level.
/// </summary>
public sealed class LogEntryViewModelTests
{
    [Fact]
    public void Constructor_SetsTimestamp()
    {
        // Arrange
        var timestamp = new DateTimeOffset(2026, 4, 1, 12, 0, 0, TimeSpan.Zero);

        // Act
        var sut = new LogEntryViewModel(timestamp, LogLevel.Info, "test");

        // Assert
        sut.Timestamp.Should().Be(timestamp);
    }

    [Fact]
    public void Constructor_SetsMessage()
    {
        // Arrange
        var timestamp = DateTimeOffset.UtcNow;

        // Act
        var sut = new LogEntryViewModel(timestamp, LogLevel.Info, "hello world");

        // Assert
        sut.Message.Should().Be("hello world");
    }

    [Theory]
    [InlineData(LogLevel.Trace)]
    [InlineData(LogLevel.Debug)]
    [InlineData(LogLevel.Info)]
    [InlineData(LogLevel.Warn)]
    [InlineData(LogLevel.Error)]
    public void Constructor_SetsLevel(LogLevel level)
    {
        // Arrange
        var timestamp = DateTimeOffset.UtcNow;

        // Act
        var sut = new LogEntryViewModel(timestamp, level, "msg");

        // Assert
        sut.Level.Should().Be(level);
    }
}
