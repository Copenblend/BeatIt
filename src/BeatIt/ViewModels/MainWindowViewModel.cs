using System.ComponentModel;

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
    /// Gets the side bar view model for binding side bar width, content, and resize behavior.
    /// </summary>
    public SideBarViewModel SideBar { get; }

    /// <summary>
    /// Gets the panel view model for binding the bottom panel tabs and content.
    /// </summary>
    public PanelViewModel Panel { get; }

    /// <summary>
    /// Gets the menu bar view model for binding menu items and commands.
    /// </summary>
    public MenuBarViewModel MenuBar { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
    /// </summary>
    /// <param name="windowService">
    /// The window service used to perform window management operations.
    /// </param>
    /// <param name="activityBar">
    /// The activity bar view model providing sidebar visibility and item management.
    /// </param>
    /// <param name="sideBar">
    /// The side bar view model providing side bar width, content, and resize behavior.
    /// </param>
    /// <param name="panel">
    /// The panel view model providing bottom panel tabs and content.
    /// </param>
    /// <param name="menuBar">
    /// The menu bar view model providing menu items and commands.
    /// </param>
    public MainWindowViewModel(IWindowService windowService, ActivityBarViewModel activityBar, SideBarViewModel sideBar, PanelViewModel panel, MenuBarViewModel menuBar)
    {
        _windowService = windowService;
        _isMaximized = _windowService.IsMaximized;
        ActivityBar = activityBar;
        SideBar = sideBar;
        Panel = panel;
        MenuBar = menuBar;

        ActivityBar.PropertyChanged += OnActivityBarPropertyChanged;
    }

    /// <summary>
    /// Handles property changes on the activity bar view model.
    /// Restores the side bar width to <see cref="SideBarViewModel.DefaultWidth"/>
    /// when <see cref="ActivityBarViewModel.SelectedItem"/> becomes non-null
    /// and the side bar width is zero (e.g., after a drag-snap close).
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The event arguments containing the changed property name.
    /// </param>
    private void OnActivityBarPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ActivityBarViewModel.SelectedItem)
            && ActivityBar.SelectedItem is not null
            && SideBar.Width == 0)
        {
            SideBar.Width = SideBarViewModel.DefaultWidth;
        }
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
