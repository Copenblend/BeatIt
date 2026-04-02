namespace BeatIt.Tests.Views;

using System.Globalization;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using BeatIt.ViewModels;
using BeatIt.Views;
using FluentAssertions;
using Xunit;

/// <summary>
/// Unit tests for <see cref="LogLevelBrushConverter"/>.
/// Verifies that each <see cref="LogLevel"/> maps to the correct foreground brush color.
/// </summary>
public class LogLevelBrushConverterTests
{
    [Theory]
    [InlineData(LogLevel.Trace, "#FF808080")]
    [InlineData(LogLevel.Debug, "#FF808080")]
    [InlineData(LogLevel.Info, "#FFCCCCCC")]
    [InlineData(LogLevel.Warn, "#FFCCA700")]
    [InlineData(LogLevel.Error, "#FFF44747")]
    public void Convert_LogLevel_ReturnsExpectedBrushColor(LogLevel level, string expectedColor)
    {
        // Act
        var result = LogLevelBrushConverter.Instance.Convert(level, typeof(IBrush), null, CultureInfo.InvariantCulture);

        // Assert
        var brush = result.Should().BeOfType<ImmutableSolidColorBrush>().Subject;
        brush.Color.Should().Be(Color.Parse(expectedColor));
    }

    [Fact]
    public void Convert_TraceAndDebug_ReturnSameBrushInstance()
    {
        // Act
        var trace = LogLevelBrushConverter.Instance.Convert(LogLevel.Trace, typeof(IBrush), null, CultureInfo.InvariantCulture);
        var debug = LogLevelBrushConverter.Instance.Convert(LogLevel.Debug, typeof(IBrush), null, CultureInfo.InvariantCulture);

        // Assert
        trace.Should().BeSameAs(debug);
    }

    [Fact]
    public void Convert_UndefinedLogLevel_ReturnsDefaultBrush()
    {
        // Act
        var result = LogLevelBrushConverter.Instance.Convert((LogLevel)999, typeof(IBrush), null, CultureInfo.InvariantCulture);

        // Assert
        var brush = result.Should().BeOfType<ImmutableSolidColorBrush>().Subject;
        brush.Color.Should().Be(Color.Parse("#FFCCCCCC"));
    }
}
