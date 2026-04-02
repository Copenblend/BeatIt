using CommunityToolkit.Mvvm.Input;

namespace BeatIt.ViewModels;

/// <summary>
/// View model for the application menu bar.
/// Provides commands for file operations such as opening and closing folders.
/// </summary>
public partial class MenuBarViewModel : ViewModelBase
{
    private readonly ExplorerViewModel _explorerViewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="MenuBarViewModel"/> class.
    /// </summary>
    /// <param name="explorerViewModel">
    /// The explorer view model providing folder-open functionality.
    /// </param>
    public MenuBarViewModel(ExplorerViewModel explorerViewModel)
    {
        _explorerViewModel = explorerViewModel;
    }

    /// <summary>
    /// Gets the command that opens a folder picker and loads the selected folder.
    /// </summary>
    public IRelayCommand OpenFolderCommand => _explorerViewModel.OpenFolderCommand;

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
