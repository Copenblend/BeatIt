using System.Collections.ObjectModel;
using BeatIt.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BeatIt.ViewModels;

/// <summary>
/// Represents a single file or directory node in the explorer tree,
/// supporting lazy-loading of child entries when expanded.
/// </summary>
public partial class FileTreeNodeViewModel : ViewModelBase
{
    private readonly IFileSystemService _fileSystemService;

    /// <summary>
    /// Gets or sets a value indicating whether this directory node is expanded in the tree.
    /// When set to <see langword="true"/> on a directory that has not yet been loaded,
    /// child entries are fetched asynchronously.
    /// </summary>
    [ObservableProperty]
    private bool _isExpanded;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileTreeNodeViewModel"/> class.
    /// </summary>
    /// <param name="fileSystemService">
    /// The file system service used to enumerate child entries.
    /// </param>
    /// <param name="name">
    /// The display name of the file or directory.
    /// </param>
    /// <param name="fullPath">
    /// The absolute path of the file or directory.
    /// </param>
    /// <param name="isDirectory">
    /// <see langword="true"/> if this node represents a directory; otherwise, <see langword="false"/>.
    /// </param>
    /// <param name="extension">
    /// The file extension including the leading dot, or an empty string for directories.
    /// </param>
    public FileTreeNodeViewModel(
        IFileSystemService fileSystemService,
        string name,
        string fullPath,
        bool isDirectory,
        string extension)
    {
        _fileSystemService = fileSystemService;
        Name = name;
        FullPath = fullPath;
        IsDirectory = isDirectory;
        Extension = extension;

        if (isDirectory)
        {
            Children.Add(new FileTreeNodeViewModel(
                fileSystemService,
                "Loading...",
                string.Empty,
                isDirectory: false,
                extension: string.Empty));
        }
    }

    /// <summary>
    /// Gets the display name of the file or directory.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the absolute path of the file or directory.
    /// </summary>
    public string FullPath { get; }

    /// <summary>
    /// Gets a value indicating whether this node represents a directory.
    /// </summary>
    public bool IsDirectory { get; }

    /// <summary>
    /// Gets the file extension including the leading dot, or an empty string for directories.
    /// </summary>
    public string Extension { get; }

    /// <summary>
    /// Gets the child nodes of this directory node.
    /// </summary>
    public ObservableCollection<FileTreeNodeViewModel> Children { get; } = [];

    /// <summary>
    /// Gets a value indicating whether the child entries have been loaded from the file system.
    /// </summary>
    public bool IsLoaded { get; private set; }

    /// <summary>
    /// Called by the source generator when <see cref="IsExpanded"/> changes.
    /// Triggers lazy-loading of child entries for directory nodes.
    /// </summary>
    /// <param name="value">
    /// The new value of <see cref="IsExpanded"/>.
    /// </param>
    partial void OnIsExpandedChanged(bool value)
    {
        if (value && !IsLoaded && IsDirectory)
        {
            _ = LoadChildrenAsync();
        }
    }

    /// <summary>
    /// Loads the child entries from the file system and replaces the placeholder node.
    /// </summary>
    internal async Task LoadChildrenAsync()
    {
        var entries = await _fileSystemService.GetEntriesAsync(FullPath);

        Children.Clear();

        foreach (var entry in entries)
        {
            Children.Add(new FileTreeNodeViewModel(
                _fileSystemService,
                entry.Name,
                entry.FullPath,
                entry.IsDirectory,
                entry.Extension));
        }

        IsLoaded = true;
    }
}
