using CommunityToolkit.Mvvm.Input;

namespace BeatIt.ViewModels;

/// <summary>
/// View model for the application menu bar.
/// Provides commands for file operations such as opening and closing folders.
/// </summary>
/// <remarks>
/// Commands are placeholder no-ops that will be wired to real
/// functionality in future slices.
/// </remarks>
public partial class MenuBarViewModel : ViewModelBase
{
    /// <summary>
    /// Opens a folder in the workspace.
    /// </summary>
    /// <remarks>
    /// This is a placeholder command that will be wired to real
    /// folder-open logic in a future slice.
    /// </remarks>
    [RelayCommand]
    private void OpenFolder()
    {
    }

    /// <summary>
    /// Closes the currently open folder in the workspace.
    /// </summary>
    /// <remarks>
    /// This is a placeholder command that will be wired to real
    /// folder-close logic in a future slice.
    /// </remarks>
    [RelayCommand]
    private void CloseFolder()
    {
    }
}
