namespace BeatIt.Views;

using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using BeatIt.ViewModels;

/// <summary>
/// Provides a value converter that maps <see cref="LogLevel"/> values to foreground brushes
/// for colorized log entry display.
/// </summary>
public static class LogLevelBrushConverter
{
    private static readonly ImmutableSolidColorBrush s_gray = new(Color.Parse("#808080"));
    private static readonly ImmutableSolidColorBrush s_default = new(Color.Parse("#CCCCCC"));
    private static readonly ImmutableSolidColorBrush s_warn = new(Color.Parse("#CCA700"));
    private static readonly ImmutableSolidColorBrush s_error = new(Color.Parse("#F44747"));

    /// <summary>
    /// Gets a converter that maps a <see cref="LogLevel"/> to an <see cref="IBrush"/>
    /// for log entry level text colorization.
    /// </summary>
    public static FuncValueConverter<LogLevel, IBrush> Instance { get; } = new(level => level switch
    {
        LogLevel.Trace => s_gray,
        LogLevel.Debug => s_gray,
        LogLevel.Info => s_default,
        LogLevel.Warn => s_warn,
        LogLevel.Error => s_error,
        _ => s_default,
    });
}
