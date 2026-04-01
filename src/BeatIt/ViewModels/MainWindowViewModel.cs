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
    /// Gets the activity bar view model for binding sidebar visibility and activity bar items.
    /// </summary>
    public ActivityBarViewModel ActivityBar { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
    /// </summary>
    /// <param name="windowService">
    /// The window service used to perform window management operations.
    /// </param>
    /// <param name="activityBar">
    /// The activity bar view model providing sidebar visibility and item management.
    /// </param>
    public MainWindowViewModel(IWindowService windowService, ActivityBarViewModel activityBar)
    {
        _windowService = windowService;
        _isMaximized = _windowService.IsMaximized;
        ActivityBar = activityBar;
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
