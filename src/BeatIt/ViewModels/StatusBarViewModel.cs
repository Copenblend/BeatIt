using System.ComponentModel;

using BeatIt.Services;

using CommunityToolkit.Mvvm.ComponentModel;

namespace BeatIt.ViewModels;

/// <summary>
/// View model for the application status bar.
/// Reflects the current status text from the <see cref="IStatusBarService"/>.
/// </summary>
public sealed partial class StatusBarViewModel : ViewModelBase, IDisposable
{
    private readonly IStatusBarService _statusBarService;

    /// <summary>
    /// Gets the text currently displayed in the status bar.
    /// </summary>
    [ObservableProperty]
    private string _statusText;

    /// <summary>
    /// Initializes a new instance of the <see cref="StatusBarViewModel"/> class.
    /// </summary>
    /// <param name="statusBarService">
    /// The status bar service that provides the current status text.
    /// </param>
    public StatusBarViewModel(IStatusBarService statusBarService)
    {
        _statusBarService = statusBarService;
        _statusText = _statusBarService.StatusText;
        _statusBarService.PropertyChanged += OnServicePropertyChanged;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _statusBarService.PropertyChanged -= OnServicePropertyChanged;
    }

    private void OnServicePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IStatusBarService.StatusText))
        {
            StatusText = _statusBarService.StatusText;
        }
    }
}
