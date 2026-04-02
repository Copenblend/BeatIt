using BeatIt.Logging;
using BeatIt.ViewModels;
using FluentAssertions;
using Xunit;

namespace BeatIt.Tests.Logging;

/// <summary>
/// Unit tests for <see cref="LogEntry"/>.
/// Verifies constructor property assignment, record equality, and all log level values.
/// </summary>
public sealed class LogEntryTests
{
    [Fact]
    public void Constructor_SetsTimestampCorrectly()
    {
        // Arrange
        var timestamp = new DateTimeOffset(2026, 4, 1, 12, 0, 0, TimeSpan.Zero);

        // Act
        var sut = new LogEntry(timestamp, LogLevel.Info, "msg");

        // Assert
        sut.Timestamp.Should().Be(timestamp);
    }

    [Fact]
    public void Constructor_SetsLevelCorrectly()
    {
        // Arrange & Act
        var sut = new LogEntry(DateTimeOffset.UtcNow, LogLevel.Warn, "msg");

        // Assert
        sut.Level.Should().Be(LogLevel.Warn);
    }

    [Fact]
    public void Constructor_SetsMessageCorrectly()
    {
        // Arrange & Act
        var sut = new LogEntry(DateTimeOffset.UtcNow, LogLevel.Info, "hello world");

        // Assert
        sut.Message.Should().Be("hello world");
    }

    [Fact]
    public void Equality_TwoRecordsWithSameValues_AreEqual()
    {
        // Arrange
        var timestamp = new DateTimeOffset(2026, 4, 1, 12, 0, 0, TimeSpan.Zero);
        var a = new LogEntry(timestamp, LogLevel.Error, "boom");
        var b = new LogEntry(timestamp, LogLevel.Error, "boom");

        // Act & Assert
        a.Should().Be(b);
        (a == b).Should().BeTrue();
    }

    [Fact]
    public void Inequality_TwoRecordsWithDifferentTimestamp_AreNotEqual()
    {
        // Arrange
        var a = new LogEntry(DateTimeOffset.MinValue, LogLevel.Info, "msg");
        var b = new LogEntry(DateTimeOffset.MaxValue, LogLevel.Info, "msg");

        // Act & Assert
        a.Should().NotBe(b);
        (a != b).Should().BeTrue();
    }

    [Fact]
    public void Inequality_TwoRecordsWithDifferentLevel_AreNotEqual()
    {
        // Arrange
        var timestamp = DateTimeOffset.UtcNow;
        var a = new LogEntry(timestamp, LogLevel.Trace, "msg");
        var b = new LogEntry(timestamp, LogLevel.Error, "msg");

        // Act & Assert
        a.Should().NotBe(b);
    }

    [Fact]
    public void Inequality_TwoRecordsWithDifferentMessage_AreNotEqual()
    {
        // Arrange
        var timestamp = DateTimeOffset.UtcNow;
        var a = new LogEntry(timestamp, LogLevel.Info, "alpha");
        var b = new LogEntry(timestamp, LogLevel.Info, "beta");

        // Act & Assert
        a.Should().NotBe(b);
    }

    [Theory]
    [InlineData(LogLevel.Trace)]
    [InlineData(LogLevel.Debug)]
    [InlineData(LogLevel.Info)]
    [InlineData(LogLevel.Warn)]
    [InlineData(LogLevel.Error)]
    public void Constructor_AllLogLevelValues_SetsLevelCorrectly(LogLevel level)
    {
        // Arrange & Act
        var sut = new LogEntry(DateTimeOffset.UtcNow, level, "test");

        // Assert
        sut.Level.Should().Be(level);
    }
}
