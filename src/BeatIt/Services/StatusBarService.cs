using System.ComponentModel;

namespace BeatIt.Services;

/// <summary>
/// Manages the application status bar state and notifies consumers of changes.
/// </summary>
public sealed class StatusBarService : IStatusBarService
{
    private string _statusText = "Ready";

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc />
    public string StatusText
    {
        get => _statusText;
        set
        {
            if (_statusText == value)
            {
                return;
            }

            _statusText = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StatusText)));
        }
    }
}
