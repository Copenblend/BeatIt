namespace BeatIt.ViewModels;

/// <summary>
/// Represents the severity level of a log entry.
/// </summary>
public enum LogLevel
{
    /// <summary>
    /// Finest-grained informational events, typically only useful for detailed debugging.
    /// </summary>
    Trace,

    /// <summary>
    /// Detailed informational events useful for debugging.
    /// </summary>
    Debug,

    /// <summary>
    /// Informational messages highlighting normal application progress.
    /// </summary>
    Info,

    /// <summary>
    /// Potentially harmful situations that deserve attention.
    /// </summary>
    Warn,

    /// <summary>
    /// Error events that might still allow the application to continue running.
    /// </summary>
    Error,
}
