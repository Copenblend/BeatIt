using System.Collections.ObjectModel;
using BeatIt.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;

namespace BeatIt.ViewModels;

/// <summary>
/// View model for the explorer panel, managing the open folder and its file tree.
/// </summary>
public partial class ExplorerViewModel : ViewModelBase
{
    private readonly IFolderPickerService _folderPickerService;
    private readonly IFileSystemService _fileSystemService;

    /// <summary>
    /// Gets or sets the selected file tree node in the explorer.
    /// </summary>
    /// <remarks>
    /// Setting this property triggers click handling: directories toggle expansion,
    /// files log the click. The property resets to <see langword="null"/> after processing
    /// to allow re-clicking the same node.
    /// </remarks>
    [ObservableProperty]
    private FileTreeNodeViewModel? _selectedNode;

    /// <summary>
    /// Gets or sets the display name of the currently open folder.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasFolder))]
    [NotifyCanExecuteChangedFor(nameof(CloseFolderCommand))]
    private string? _folderName;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExplorerViewModel"/> class.
    /// </summary>
    /// <param name="folderPickerService">
    /// The service used to present a folder picker dialog.
    /// </param>
    /// <param name="fileSystemService">
    /// The service used to enumerate file system entries.
    /// </param>
    public ExplorerViewModel(
        IFolderPickerService folderPickerService,
        IFileSystemService fileSystemService)
    {
        _folderPickerService = folderPickerService;
        _fileSystemService = fileSystemService;
    }

    /// <summary>
    /// Gets the top-level file and directory nodes of the currently open folder.
    /// </summary>
    public ObservableCollection<FileTreeNodeViewModel> RootNodes { get; } = [];

    /// <summary>
    /// Gets a value indicating whether a folder is currently open.
    /// </summary>
    public bool HasFolder => !string.IsNullOrEmpty(FolderName);

    /// <summary>
    /// Opens a folder picker dialog and loads the selected folder's contents into the explorer tree.
    /// </summary>
    [RelayCommand]
    private async Task OpenFolderAsync()
    {
        var result = await _folderPickerService.PickFolderAsync();

        if (result is null)
        {
            return;
        }

        FolderName = Path.GetFileName(result.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));

        var entries = await _fileSystemService.GetEntriesAsync(result);

        RootNodes.Clear();

        foreach (var entry in entries)
        {
            RootNodes.Add(new FileTreeNodeViewModel(
                _fileSystemService,
                entry.Name,
                entry.FullPath,
                entry.IsDirectory,
                entry.Extension));
        }

        Log.Verbose("Folder {FolderName} open by user.", FolderName);
    }

    /// <summary>
    /// Closes the currently open folder, clearing the file tree and folder name.
    /// </summary>
    [RelayCommand(CanExecute = nameof(HasFolder))]
    private void CloseFolder()
    {
        var previousFolderName = FolderName;
        RootNodes.Clear();
        FolderName = null;
        Log.Verbose("Folder {FolderName} closed by user.", previousFolderName);
    }

    /// <summary>
    /// Handles selection changes: toggles directory expansion or logs file clicks,
    /// then resets the selection to allow re-clicking the same node.
    /// </summary>
    /// <param name="value">
    /// The newly selected node, or <see langword="null"/> when the selection is reset.
    /// </param>
    partial void OnSelectedNodeChanged(FileTreeNodeViewModel? value)
    {
        if (value is null)
        {
            return;
        }

        if (value.IsDirectory)
        {
            value.IsExpanded = !value.IsExpanded;
        }
        else
        {
            Log.Verbose("User clicked {FileName}.{FileExtension}", value.Name, value.Extension);
        }

        SelectedNode = null;
    }
}
