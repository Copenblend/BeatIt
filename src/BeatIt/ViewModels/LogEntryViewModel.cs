namespace BeatIt.ViewModels;

/// <summary>
/// Read-only display model representing a single log entry.
/// </summary>
public sealed class LogEntryViewModel
{
    /// <summary>
    /// Gets the timestamp when the log entry was created.
    /// </summary>
    public DateTimeOffset Timestamp { get; }

    /// <summary>
    /// Gets the severity level of the log entry.
    /// </summary>
    public LogLevel Level { get; }

    /// <summary>
    /// Gets the log message text.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LogEntryViewModel"/> class.
    /// </summary>
    /// <param name="timestamp">
    /// The timestamp when the log entry was created.
    /// </param>
    /// <param name="level">
    /// The severity level of the log entry.
    /// </param>
    /// <param name="message">
    /// The log message text.
    /// </param>
    public LogEntryViewModel(DateTimeOffset timestamp, LogLevel level, string message)
    {
        Timestamp = timestamp;
        Level = level;
        Message = message;
    }
}
