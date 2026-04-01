using System.ComponentModel;

namespace BeatIt.Services;

/// <summary>
/// Provides an abstraction over the application status bar state.
/// </summary>
public interface IStatusBarService : INotifyPropertyChanged
{
    /// <summary>
    /// Gets or sets the text displayed in the status bar.
    /// </summary>
    string StatusText { get; set; }
}
