using System.Collections.ObjectModel;

namespace BeatIt.Logging;

/// <summary>
/// Provides read-only observable access to log entries collected by the application's log sink.
/// </summary>
/// <remarks>
/// This interface does not expose an <c>Emit</c> method — writing log entries is an
/// implementation detail of the underlying Serilog sink. Consumers should only observe
/// the <see cref="Entries"/> collection.
/// </remarks>
public interface ILogSink
{
    /// <summary>
    /// Gets the read-only observable collection of log entries.
    /// </summary>
    /// <remarks>
    /// The collection satisfies <see cref="IReadOnlyList{T}"/> and provides
    /// <see cref="System.Collections.Specialized.INotifyCollectionChanged"/> so that
    /// view models can observe new entries as they arrive.
    /// </remarks>
    ReadOnlyObservableCollection<LogEntry> Entries { get; }
}
