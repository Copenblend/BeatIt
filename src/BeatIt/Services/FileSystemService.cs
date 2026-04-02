namespace BeatIt.Services;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Enumerates file system entries for a given directory, returning directories first then files,
/// each sorted alphabetically using ordinal ignore-case comparison.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Thin wrapper over System.IO file system APIs. Exception catch blocks for UnauthorizedAccessException and IOException cannot be reliably triggered in unit tests.")]
public sealed class FileSystemService : IFileSystemService
{
    /// <inheritdoc />
    public Task<IReadOnlyList<FileSystemEntry>> GetEntriesAsync(string directoryPath)
    {
        return Task.Run(() => GetEntries(directoryPath));
    }

    private static IReadOnlyList<FileSystemEntry> GetEntries(string directoryPath)
    {
        var results = new List<FileSystemEntry>();

        try
        {
            foreach (var dir in Directory.EnumerateDirectories(directoryPath))
            {
                try
                {
                    results.Add(new FileSystemEntry(
                        Path.GetFileName(dir),
                        dir,
                        IsDirectory: true,
                        Extension: string.Empty));
                }
                catch (UnauthorizedAccessException)
                {
                    // Skip directories the user cannot access.
                }
                catch (IOException)
                {
                    // Skip directories that are unavailable.
                }
            }
        }
        catch (UnauthorizedAccessException)
        {
            // Cannot enumerate directories at all — return what we have.
        }
        catch (IOException)
        {
            // Cannot enumerate directories at all — return what we have.
        }

        try
        {
            foreach (var file in Directory.EnumerateFiles(directoryPath))
            {
                try
                {
                    results.Add(new FileSystemEntry(
                        Path.GetFileName(file),
                        file,
                        IsDirectory: false,
                        Extension: Path.GetExtension(file)));
                }
                catch (UnauthorizedAccessException)
                {
                    // Skip files the user cannot access.
                }
                catch (IOException)
                {
                    // Skip files that are unavailable.
                }
            }
        }
        catch (UnauthorizedAccessException)
        {
            // Cannot enumerate files at all — return what we have.
        }
        catch (IOException)
        {
            // Cannot enumerate files at all — return what we have.
        }

        results.Sort(static (a, b) =>
        {
            if (a.IsDirectory != b.IsDirectory)
            {
                return a.IsDirectory ? -1 : 1;
            }

            return string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase);
        });

        return results;
    }
}
