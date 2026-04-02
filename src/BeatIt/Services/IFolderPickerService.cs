namespace BeatIt.Services;

/// <summary>
/// Provides an abstraction over the platform folder picker dialog.
/// </summary>
public interface IFolderPickerService
{
    /// <summary>
    /// Opens a folder picker dialog and returns the selected folder path.
    /// </summary>
    /// <returns>
    /// The full path of the selected folder, or <see langword="null"/> if the user cancelled the dialog.
    /// </returns>
    Task<string?> PickFolderAsync();
}
