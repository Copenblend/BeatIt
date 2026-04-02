namespace BeatIt.Services;

/// <summary>
/// Represents a single file or directory entry in the file system.
/// </summary>
/// <param name="Name">
/// The file or directory name without the path.
/// </param>
/// <param name="FullPath">
/// The absolute path of the file or directory.
/// </param>
/// <param name="IsDirectory">
/// <see langword="true"/> if the entry is a directory; otherwise, <see langword="false"/>.
/// </param>
/// <param name="Extension">
/// The file extension including the leading dot, or an empty string for directories and files without extensions.
/// </param>
public sealed record FileSystemEntry(string Name, string FullPath, bool IsDirectory, string Extension);

/// <summary>
/// Provides an abstraction over file system enumeration operations.
/// </summary>
public interface IFileSystemService
{
    /// <summary>
    /// Returns the subdirectories and files within the specified directory.
    /// </summary>
    /// <param name="directoryPath">
    /// The absolute path of the directory to enumerate.
    /// </param>
    /// <returns>
    /// A read-only list of entries sorted with directories first (alphabetical, ordinal ignore case),
    /// then files (alphabetical, ordinal ignore case).
    /// </returns>
    Task<IReadOnlyList<FileSystemEntry>> GetEntriesAsync(string directoryPath);
}
