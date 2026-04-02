using BeatIt.ViewModels;

namespace BeatIt.Logging;

/// <summary>
/// Represents an immutable log entry captured by the logging infrastructure.
/// </summary>
/// <param name="Timestamp">
/// The point in time when the log event occurred.
/// </param>
/// <param name="Level">
/// The severity level of the log entry.
/// </param>
/// <param name="Message">
/// The rendered log message text.
/// </param>
public sealed record LogEntry(DateTimeOffset Timestamp, LogLevel Level, string Message);
