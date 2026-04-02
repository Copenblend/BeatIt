namespace BeatIt.Services;

using System.Diagnostics.CodeAnalysis;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;

/// <summary>
/// Provides folder picker dialog functionality by delegating to the Avalonia storage provider.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Requires a running Avalonia desktop application with a windowing system and StorageProvider.")]
public sealed class FolderPickerService : IFolderPickerService
{
    /// <inheritdoc />
    public async Task<string?> PickFolderAsync()
    {
        var storageProvider = GetStorageProvider();
        var folders = await storageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Open Folder",
            AllowMultiple = false,
        });

        return folders.Count > 0 ? folders[0].Path.LocalPath : null;
    }

    private static IStorageProvider GetStorageProvider()
    {
        return ((IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!).MainWindow!.StorageProvider;
    }
}
