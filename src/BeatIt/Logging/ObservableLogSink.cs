using System.Collections.ObjectModel;
using BeatIt.ViewModels;
using Serilog.Core;
using Serilog.Events;

namespace BeatIt.Logging;

/// <summary>
/// A Serilog sink that stores log entries in an observable collection for UI consumption.
/// </summary>
/// <remarks>
/// <para>
/// Implements both <see cref="ILogSink"/> (for view model observation) and
/// <see cref="ILogEventSink"/> (for Serilog pipeline integration).
/// </para>
/// <para>
/// Log entries are dispatched to the UI thread via the configured dispatcher action.
/// The collection is bounded at <see cref="MaxEntries"/>; the oldest entry is removed
/// when the limit is exceeded.
/// </para>
/// </remarks>
public sealed class ObservableLogSink : ILogSink, ILogEventSink
{
    /// <summary>
    /// The maximum number of log entries retained before the oldest entries are removed.
    /// </summary>
    public const int MaxEntries = 10_000;

    private readonly ObservableCollection<LogEntry> _entries = [];
    private readonly ReadOnlyObservableCollection<LogEntry> _readOnlyEntries;
    private readonly Action<Action> _dispatch;

    /// <summary>
    /// Gets the read-only observable collection of log entries.
    /// </summary>
    public ReadOnlyObservableCollection<LogEntry> Entries => _readOnlyEntries;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableLogSink"/> class
    /// using the Avalonia UI thread dispatcher.
    /// </summary>
    public ObservableLogSink()
        : this(action => Avalonia.Threading.Dispatcher.UIThread.Post(action))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableLogSink"/> class
    /// with a custom dispatcher action for testability.
    /// </summary>
    /// <param name="dispatch">
    /// An action that schedules work on the appropriate thread.
    /// Tests can pass <c>a =&gt; a()</c> for synchronous execution.
    /// </param>
    internal ObservableLogSink(Action<Action> dispatch)
    {
        _dispatch = dispatch;
        _readOnlyEntries = new(_entries);
    }

    /// <summary>
    /// Emits a Serilog log event by converting it to a <see cref="LogEntry"/>
    /// and dispatching it to the UI thread for collection insertion.
    /// </summary>
    /// <param name="logEvent">
    /// The Serilog log event to emit.
    /// </param>
    public void Emit(LogEvent logEvent)
    {
        var entry = new LogEntry(
            logEvent.Timestamp,
            MapLevel(logEvent.Level),
            logEvent.RenderMessage());

        _dispatch(() => AddEntry(entry));
    }

    /// <summary>
    /// Adds a log entry directly to the collection, enforcing the
    /// <see cref="MaxEntries"/> capacity limit.
    /// </summary>
    /// <param name="entry">
    /// The log entry to add.
    /// </param>
    /// <remarks>
    /// This method must be called on the UI thread (or the thread owning the collection).
    /// It is internal for direct testing without the dispatcher.
    /// </remarks>
    internal void AddEntry(LogEntry entry)
    {
        _entries.Add(entry);

        if (_entries.Count > MaxEntries)
        {
            _entries.RemoveAt(0);
        }
    }

    private static LogLevel MapLevel(LogEventLevel level) => level switch
    {
        LogEventLevel.Verbose => LogLevel.Trace,
        LogEventLevel.Debug => LogLevel.Debug,
        LogEventLevel.Information => LogLevel.Info,
        LogEventLevel.Warning => LogLevel.Warn,
        LogEventLevel.Error => LogLevel.Error,
        LogEventLevel.Fatal => LogLevel.Error,
        _ => LogLevel.Info,
    };
}
