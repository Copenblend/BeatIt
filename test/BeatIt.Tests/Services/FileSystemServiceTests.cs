using BeatIt.Services;
using FluentAssertions;
using Xunit;

namespace BeatIt.Tests.Services;

/// <summary>
/// Unit tests for <see cref="FileSystemService"/>.
/// Verifies file system enumeration, sort order, and graceful error handling.
/// </summary>
public sealed class FileSystemServiceTests
{
    /// <summary>
    /// Verifies that directories are returned before files, each group sorted alphabetically.
    /// </summary>
    [Fact]
    public async Task GetEntriesAsync_ReturnsDirectoriesFirst_ThenFiles()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        try
        {
            Directory.CreateDirectory(Path.Combine(tempDir, "Bravo"));
            Directory.CreateDirectory(Path.Combine(tempDir, "Alpha"));
            File.WriteAllText(Path.Combine(tempDir, "delta.txt"), string.Empty);
            File.WriteAllText(Path.Combine(tempDir, "charlie.txt"), string.Empty);

            var sut = new FileSystemService();

            // Act
            var result = await sut.GetEntriesAsync(tempDir);

            // Assert
            result.Should().HaveCount(4);
            result[0].Name.Should().Be("Alpha");
            result[0].IsDirectory.Should().BeTrue();
            result[1].Name.Should().Be("Bravo");
            result[1].IsDirectory.Should().BeTrue();
            result[2].Name.Should().Be("charlie.txt");
            result[2].IsDirectory.Should().BeFalse();
            result[3].Name.Should().Be("delta.txt");
            result[3].IsDirectory.Should().BeFalse();
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    /// <summary>
    /// Verifies that returned entries have correct Name, FullPath, IsDirectory, and Extension properties.
    /// </summary>
    [Fact]
    public async Task GetEntriesAsync_ReturnsCorrectEntryProperties()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        try
        {
            var subDir = Path.Combine(tempDir, "SubFolder");
            Directory.CreateDirectory(subDir);
            var filePath = Path.Combine(tempDir, "readme.md");
            File.WriteAllText(filePath, string.Empty);

            var sut = new FileSystemService();

            // Act
            var result = await sut.GetEntriesAsync(tempDir);

            // Assert
            result.Should().HaveCount(2);

            var dirEntry = result[0];
            dirEntry.Name.Should().Be("SubFolder");
            dirEntry.FullPath.Should().Be(subDir);
            dirEntry.IsDirectory.Should().BeTrue();
            dirEntry.Extension.Should().BeEmpty();

            var fileEntry = result[1];
            fileEntry.Name.Should().Be("readme.md");
            fileEntry.FullPath.Should().Be(filePath);
            fileEntry.IsDirectory.Should().BeFalse();
            fileEntry.Extension.Should().Be(".md");
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    /// <summary>
    /// Verifies that an empty directory returns an empty list.
    /// </summary>
    [Fact]
    public async Task GetEntriesAsync_EmptyDirectory_ReturnsEmptyList()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        try
        {
            var sut = new FileSystemService();

            // Act
            var result = await sut.GetEntriesAsync(tempDir);

            // Assert
            result.Should().BeEmpty();
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    /// <summary>
    /// Verifies that a non-existent directory returns an empty list
    /// (the IOException is caught internally).
    /// </summary>
    [Fact]
    public async Task GetEntriesAsync_NonExistentDirectory_ReturnsEmptyList()
    {
        // Arrange
        var nonExistentDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var sut = new FileSystemService();

        // Act
        var result = await sut.GetEntriesAsync(nonExistentDir);

        // Assert
        result.Should().BeEmpty();
    }
}
