namespace BeatIt.Services;

/// <summary>
/// Provides an abstraction over window management operations.
/// </summary>
public interface IWindowService
{
    /// <summary>
    /// Gets a value indicating whether the window is currently maximized.
    /// </summary>
    bool IsMaximized { get; }

    /// <summary>
    /// Minimizes the window.
    /// </summary>
    void Minimize();

    /// <summary>
    /// Toggles the window between maximized and restored states.
    /// </summary>
    void MaximizeRestore();

    /// <summary>
    /// Closes the window.
    /// </summary>
    void Close();
}
