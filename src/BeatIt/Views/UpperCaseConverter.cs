namespace BeatIt.Views;

using Avalonia.Data.Converters;

/// <summary>
/// Provides a value converter that transforms a string to its uppercase representation.
/// </summary>
public static class UpperCaseConverter
{
    /// <summary>
    /// Gets a converter that transforms a string to uppercase using invariant culture rules.
    /// </summary>
    public static FuncValueConverter<string?, string?> Instance { get; } =
        new(s => s?.ToUpperInvariant());
}
