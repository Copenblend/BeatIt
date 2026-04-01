using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using BeatIt.Services;

namespace BeatIt.ViewModels;

/// <summary>
/// View model for the main application window.
/// Provides commands for window chrome operations (minimize, maximize/restore, close).
/// </summary>
public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IWindowService _windowService;

    [ObservableProperty]
    private bool _isMaximized;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
    /// </summary>
    /// <param name="windowService">
    /// The window service used to perform window management operations.
    /// </param>
    public MainWindowViewModel(IWindowService windowService)
    {
        _windowService = windowService;
        _isMaximized = _windowService.IsMaximized;
    }

    /// <summary>
    /// Minimizes the application window.
    /// </summary>
    [RelayCommand]
    private void Minimize() => _windowService.Minimize();

    /// <summary>
    /// Toggles the application window between maximized and restored states.
    /// </summary>
    [RelayCommand]
    private void MaximizeRestore()
    {
        _windowService.MaximizeRestore();
        IsMaximized = _windowService.IsMaximized;
    }

    /// <summary>
    /// Closes the application window.
    /// </summary>
    [RelayCommand]
    private void Close() => _windowService.Close();
}
