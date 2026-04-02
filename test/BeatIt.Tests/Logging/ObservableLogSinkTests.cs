using BeatIt.Logging;
using BeatIt.ViewModels;
using FluentAssertions;
using Serilog.Events;
using Serilog.Parsing;
using Xunit;

namespace BeatIt.Tests.Logging;

/// <summary>
/// Unit tests for <see cref="ObservableLogSink"/>.
/// Verifies observable collection behaviour, bounded capacity, log event emission,
/// and Serilog-to-application log level mapping.
/// </summary>
public sealed class ObservableLogSinkTests
{
    private static readonly MessageTemplateParser TemplateParser = new();

    private static ObservableLogSink CreateSink() => new(a => a());

    private static LogEvent CreateLogEvent(
        LogEventLevel level,
        string message = "Test message",
        DateTimeOffset? timestamp = null)
    {
        var template = TemplateParser.Parse(message);
        return new LogEvent(timestamp ?? DateTimeOffset.UtcNow, level, null, template, []);
    }

    [Fact]
    public void Entries_Initially_IsEmpty()
    {
        // Arrange & Act
        var sut = CreateSink();

        // Assert
        sut.Entries.Should().BeEmpty();
    }

    [Fact]
    public void AddEntry_SingleEntry_AppearsInEntries()
    {
        // Arrange
        var sut = CreateSink();
        var entry = new LogEntry(DateTimeOffset.UtcNow, LogLevel.Info, "hello");

        // Act
        sut.AddEntry(entry);

        // Assert
        sut.Entries.Should().ContainSingle()
            .Which.Should().Be(entry);
    }

    [Fact]
    public void AddEntry_PreservesInsertionOrder()
    {
        // Arrange
        var sut = CreateSink();
        var first = new LogEntry(DateTimeOffset.UtcNow, LogLevel.Info, "first");
        var second = new LogEntry(DateTimeOffset.UtcNow, LogLevel.Warn, "second");
        var third = new LogEntry(DateTimeOffset.UtcNow, LogLevel.Error, "third");

        // Act
        sut.AddEntry(first);
        sut.AddEntry(second);
        sut.AddEntry(third);

        // Assert
        sut.Entries.Should().HaveCount(3);
        sut.Entries[0].Should().Be(first);
        sut.Entries[1].Should().Be(second);
        sut.Entries[2].Should().Be(third);
    }

    [Fact]
    public void AddEntry_AtExactlyMaxEntries_DoesNotRemoveAny()
    {
        // Arrange
        var sut = CreateSink();
        for (var i = 0; i < ObservableLogSink.MaxEntries; i++)
        {
            sut.AddEntry(new LogEntry(DateTimeOffset.UtcNow, LogLevel.Info, $"entry-{i}"));
        }

        // Assert
        sut.Entries.Should().HaveCount(ObservableLogSink.MaxEntries);
        sut.Entries[0].Message.Should().Be("entry-0");
    }

    [Fact]
    public void AddEntry_BeyondMaxEntries_RemovesOldestEntry()
    {
        // Arrange
        var sut = CreateSink();
        for (var i = 0; i <= ObservableLogSink.MaxEntries; i++)
        {
            sut.AddEntry(new LogEntry(DateTimeOffset.UtcNow, LogLevel.Info, $"entry-{i}"));
        }

        // Assert
        sut.Entries.Should().HaveCount(ObservableLogSink.MaxEntries);
        sut.Entries[0].Message.Should().Be("entry-1");
        sut.Entries[^1].Message.Should().Be($"entry-{ObservableLogSink.MaxEntries}");
    }

    [Fact]
    public void Emit_DispatchesToConfiguredDispatcher()
    {
        // Arrange
        var dispatched = false;
        var sut = new ObservableLogSink(a =>
        {
            dispatched = true;
            a();
        });
        var logEvent = CreateLogEvent(LogEventLevel.Information);

        // Act
        sut.Emit(logEvent);

        // Assert
        dispatched.Should().BeTrue();
    }

    [Fact]
    public void Emit_ConvertsTimestampCorrectly()
    {
        // Arrange
        var sut = CreateSink();
        var timestamp = new DateTimeOffset(2026, 4, 1, 10, 30, 0, TimeSpan.Zero);
        var logEvent = CreateLogEvent(LogEventLevel.Information, timestamp: timestamp);

        // Act
        sut.Emit(logEvent);

        // Assert
        sut.Entries.Should().ContainSingle()
            .Which.Timestamp.Should().Be(timestamp);
    }

    [Fact]
    public void Emit_ConvertsMessageCorrectly()
    {
        // Arrange
        var sut = CreateSink();
        var logEvent = CreateLogEvent(LogEventLevel.Information, "Hello from test");

        // Act
        sut.Emit(logEvent);

        // Assert
        sut.Entries.Should().ContainSingle()
            .Which.Message.Should().Be("Hello from test");
    }

    [Theory]
    [InlineData(LogEventLevel.Verbose, LogLevel.Trace)]
    [InlineData(LogEventLevel.Debug, LogLevel.Debug)]
    [InlineData(LogEventLevel.Information, LogLevel.Info)]
    [InlineData(LogEventLevel.Warning, LogLevel.Warn)]
    [InlineData(LogEventLevel.Error, LogLevel.Error)]
    [InlineData(LogEventLevel.Fatal, LogLevel.Error)]
    public void Emit_MapsLogEventLevel_ToExpectedLogLevel(
        LogEventLevel serilogLevel,
        LogLevel expectedLevel)
    {
        // Arrange
        var sut = CreateSink();
        var logEvent = CreateLogEvent(serilogLevel);

        // Act
        sut.Emit(logEvent);

        // Assert
        sut.Entries.Should().ContainSingle()
            .Which.Level.Should().Be(expectedLevel);
    }

    [Fact]
    public void Emit_UndefinedLogEventLevel_MapsToInfo()
    {
        // Arrange
        var sut = CreateSink();
        var logEvent = CreateLogEvent((LogEventLevel)999);

        // Act
        sut.Emit(logEvent);

        // Assert
        sut.Entries.Should().ContainSingle()
            .Which.Level.Should().Be(LogLevel.Info);
    }
}
