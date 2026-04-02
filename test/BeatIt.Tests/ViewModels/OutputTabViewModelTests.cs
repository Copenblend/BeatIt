using System.Collections.ObjectModel;
using BeatIt.Logging;
using BeatIt.ViewModels;
using FluentAssertions;
using Xunit;

namespace BeatIt.Tests.ViewModels;

/// <summary>
/// Unit tests for <see cref="OutputTabViewModel"/>.
/// Verifies default state, log entry filtering, bounded capacity,
/// and property change notifications.
/// </summary>
public sealed class OutputTabViewModelTests
{
    [Fact]
    public void Constructor_SetsTitleToOutput()
    {
        // Arrange & Act
        var sut = new OutputTabViewModel();

        // Assert
        sut.Title.Should().Be("Output");
    }

    [Fact]
    public void Constructor_SetsDefaultSelectedLogLevelToTrace()
    {
        // Arrange & Act
        var sut = new OutputTabViewModel();

        // Assert
        sut.SelectedLogLevel.Should().Be(LogLevel.Trace);
    }

    [Fact]
    public void Constructor_FilteredEntriesIsEmpty()
    {
        // Arrange & Act
        var sut = new OutputTabViewModel();

        // Assert
        sut.FilteredEntries.Should().BeEmpty();
    }

    [Fact]
    public void AvailableLogLevels_ContainsAllFiveLevels()
    {
        // Arrange & Act
        var levels = OutputTabViewModel.AvailableLogLevels;

        // Assert
        levels.Should().HaveCount(5);
        levels.Should().ContainInOrder(
            LogLevel.Trace,
            LogLevel.Debug,
            LogLevel.Info,
            LogLevel.Warn,
            LogLevel.Error);
    }

    [Fact]
    public void AddEntry_WhenLevelPassesFilter_AddsToFilteredEntries()
    {
        // Arrange
        var sut = new OutputTabViewModel();
        sut.SelectedLogLevel = LogLevel.Info;
        var entry = CreateEntry(LogLevel.Warn);

        // Act
        sut.AddEntry(entry);

        // Assert
        sut.FilteredEntries.Should().ContainSingle()
            .Which.Should().BeSameAs(entry);
    }

    [Fact]
    public void AddEntry_WhenLevelBelowFilter_DoesNotAddToFilteredEntries()
    {
        // Arrange
        var sut = new OutputTabViewModel();
        sut.SelectedLogLevel = LogLevel.Warn;
        var entry = CreateEntry(LogLevel.Info);

        // Act
        sut.AddEntry(entry);

        // Assert
        sut.FilteredEntries.Should().BeEmpty();
    }

    [Fact]
    public void AddEntry_TraceEntry_ShowsWhenFilterIsTrace()
    {
        // Arrange
        var sut = new OutputTabViewModel();
        sut.SelectedLogLevel = LogLevel.Trace;
        var entry = CreateEntry(LogLevel.Trace);

        // Act
        sut.AddEntry(entry);

        // Assert
        sut.FilteredEntries.Should().ContainSingle()
            .Which.Should().BeSameAs(entry);
    }

    [Fact]
    public void AddEntry_InfoEntry_DoesNotShowWhenFilterIsWarn()
    {
        // Arrange
        var sut = new OutputTabViewModel();
        sut.SelectedLogLevel = LogLevel.Warn;
        var entry = CreateEntry(LogLevel.Info);

        // Act
        sut.AddEntry(entry);

        // Assert
        sut.FilteredEntries.Should().BeEmpty();
    }

    [Fact]
    public void SelectedLogLevel_WhenChanged_RefiltersEntries()
    {
        // Arrange
        var sut = new OutputTabViewModel();
        sut.AddEntry(CreateEntry(LogLevel.Trace, "trace"));
        sut.AddEntry(CreateEntry(LogLevel.Info, "info"));
        sut.AddEntry(CreateEntry(LogLevel.Warn, "warn"));
        sut.FilteredEntries.Should().HaveCount(3);

        // Act
        sut.SelectedLogLevel = LogLevel.Info;

        // Assert
        sut.FilteredEntries.Should().HaveCount(2);
        sut.FilteredEntries.Should().OnlyContain(e => e.Level >= LogLevel.Info);
    }

    [Fact]
    public void AddEntry_BeyondMaxEntries_RemovesOldestAndCapsCount()
    {
        // Arrange
        var sut = new OutputTabViewModel();
        for (var i = 0; i < OutputTabViewModel.MaxEntries; i++)
        {
            sut.AddEntry(CreateEntry(LogLevel.Info, $"entry-{i}"));
        }

        sut.FilteredEntries.Should().HaveCount(OutputTabViewModel.MaxEntries);

        // Act
        sut.AddEntry(CreateEntry(LogLevel.Info, "overflow"));

        // Assert
        sut.FilteredEntries.Should().HaveCount(OutputTabViewModel.MaxEntries);
        sut.FilteredEntries[0].Message.Should().Be("entry-1");
        sut.FilteredEntries[^1].Message.Should().Be("overflow");
    }

    [Fact]
    public void AddEntry_BeyondMaxEntries_OldestBelowFilter_FilteredEntriesUnaffected()
    {
        // Arrange
        var sut = new OutputTabViewModel();
        sut.SelectedLogLevel = LogLevel.Info;

        // Add the first entry as Trace (below current filter).
        sut.AddEntry(CreateEntry(LogLevel.Trace, "oldest-trace"));

        // Fill the rest with Info entries to reach MaxEntries.
        for (var i = 1; i < OutputTabViewModel.MaxEntries; i++)
        {
            sut.AddEntry(CreateEntry(LogLevel.Info, $"entry-{i}"));
        }

        var filteredCountBefore = sut.FilteredEntries.Count;

        // Act — the overflow removes the Trace entry that was never in FilteredEntries.
        sut.AddEntry(CreateEntry(LogLevel.Info, "overflow"));

        // Assert — FilteredEntries gains one (the overflow) but loses zero (oldest was below filter).
        sut.FilteredEntries.Should().HaveCount(filteredCountBefore + 1);
    }

    [Fact]
    public void SelectedLogLevel_WhenChanged_RaisesPropertyChanged()
    {
        // Arrange
        var sut = new OutputTabViewModel();
        using var monitor = sut.Monitor();

        // Act
        sut.SelectedLogLevel = LogLevel.Error;

        // Assert
        monitor.Should().RaisePropertyChangeFor(s => s.SelectedLogLevel);
    }

    private static LogEntryViewModel CreateEntry(LogLevel level, string message = "test")
    {
        return new LogEntryViewModel(DateTimeOffset.UtcNow, level, message);
    }

    [Fact]
    public void Constructor_WithILogSink_SetsTitleToOutput()
    {
        // Arrange
        var sink = new ObservableLogSink(a => a());

        // Act
        var sut = new OutputTabViewModel(sink);

        // Assert
        sut.Title.Should().Be("Output");
    }

    [Fact]
    public void ILogSink_WhenEntryEmitted_AppearsInFilteredEntries()
    {
        // Arrange
        var sink = new ObservableLogSink(a => a());
        var sut = new OutputTabViewModel(sink);

        // Act
        sink.AddEntry(new LogEntry(DateTimeOffset.UtcNow, LogLevel.Info, "hello"));

        // Assert
        sut.FilteredEntries.Should().ContainSingle()
            .Which.Message.Should().Be("hello");
    }

    [Fact]
    public void ILogSink_WhenEntryBelowFilterLevel_DoesNotAppearInFilteredEntries()
    {
        // Arrange
        var sink = new ObservableLogSink(a => a());
        var sut = new OutputTabViewModel(sink);
        sut.SelectedLogLevel = LogLevel.Warn;

        // Act
        sink.AddEntry(new LogEntry(DateTimeOffset.UtcNow, LogLevel.Debug, "should not appear"));

        // Assert
        sut.FilteredEntries.Should().BeEmpty();
    }

    [Fact]
    public void ILogSink_MultipleEntries_FlowThroughCorrectly()
    {
        // Arrange
        var sink = new ObservableLogSink(a => a());
        var sut = new OutputTabViewModel(sink);

        // Act
        sink.AddEntry(new LogEntry(DateTimeOffset.UtcNow, LogLevel.Info, "one"));
        sink.AddEntry(new LogEntry(DateTimeOffset.UtcNow, LogLevel.Warn, "two"));
        sink.AddEntry(new LogEntry(DateTimeOffset.UtcNow, LogLevel.Error, "three"));

        // Assert
        sut.FilteredEntries.Should().HaveCount(3);
        sut.FilteredEntries[0].Message.Should().Be("one");
        sut.FilteredEntries[1].Message.Should().Be("two");
        sut.FilteredEntries[2].Message.Should().Be("three");
    }

    [Fact]
    public void ILogSink_EntryProperties_ArePreservedInConversion()
    {
        // Arrange
        var sink = new ObservableLogSink(a => a());
        var sut = new OutputTabViewModel(sink);
        var timestamp = new DateTimeOffset(2026, 4, 1, 12, 0, 0, TimeSpan.Zero);

        // Act
        sink.AddEntry(new LogEntry(timestamp, LogLevel.Warn, "preserved"));

        // Assert
        var vm = sut.FilteredEntries.Should().ContainSingle().Subject;
        vm.Timestamp.Should().Be(timestamp);
        vm.Level.Should().Be(LogLevel.Warn);
        vm.Message.Should().Be("preserved");
    }

    [Fact]
    public void ILogSink_WhenCollectionReset_ClearsFilteredEntries()
    {
        // Arrange
        var entries = new ObservableCollection<LogEntry>();
        var readOnly = new ReadOnlyObservableCollection<LogEntry>(entries);
        var sink = new TestLogSink(readOnly);

        var sut = new OutputTabViewModel(sink);

        // Add entries via the underlying collection (simulating sink additions)
        entries.Add(new LogEntry(DateTimeOffset.UtcNow, LogLevel.Info, "msg1"));
        entries.Add(new LogEntry(DateTimeOffset.UtcNow, LogLevel.Warn, "msg2"));
        sut.FilteredEntries.Should().HaveCount(2);

        // Act — Clear triggers Reset
        entries.Clear();

        // Assert
        sut.FilteredEntries.Should().BeEmpty();
    }

    private sealed class TestLogSink : ILogSink
    {
        public ReadOnlyObservableCollection<LogEntry> Entries { get; }

        public TestLogSink(ReadOnlyObservableCollection<LogEntry> entries)
        {
            Entries = entries;
        }
    }
}
