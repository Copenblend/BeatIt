namespace BeatIt.Tests.Views;

using System.Globalization;
using BeatIt.Views;
using FluentAssertions;
using Xunit;

/// <summary>
/// Unit tests for <see cref="UpperCaseConverter"/>.
/// Verifies that the converter correctly transforms strings to their uppercase invariant representation.
/// </summary>
public class UpperCaseConverterTests
{
    [Fact]
    public void Convert_LowercaseString_ReturnsUppercase()
    {
        // Act
        var result = UpperCaseConverter.Instance.Convert("hello", typeof(string), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be("HELLO");
    }

    [Fact]
    public void Convert_MixedCaseString_ReturnsUppercase()
    {
        // Act
        var result = UpperCaseConverter.Instance.Convert("Hello World", typeof(string), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be("HELLO WORLD");
    }

    [Fact]
    public void Convert_NullInput_ReturnsNull()
    {
        // Act
        var result = UpperCaseConverter.Instance.Convert(null, typeof(string), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Convert_EmptyString_ReturnsEmptyString()
    {
        // Act
        var result = UpperCaseConverter.Instance.Convert(string.Empty, typeof(string), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(string.Empty);
    }
}
