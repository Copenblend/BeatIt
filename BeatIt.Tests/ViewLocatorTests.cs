namespace BeatIt.Tests;

using System;
using System.Reflection;
using Avalonia.Controls;
using BeatIt.Services;
using BeatIt.ViewModels;
using Avalonia.Headless.XUnit;
using FluentAssertions;
using Moq;
using Xunit;

/// <summary>
/// Unit tests for <see cref="ViewLocator"/>.
/// Verifies view model matching and view resolution by naming convention.
/// </summary>
public class ViewLocatorTests
{
    private readonly ViewLocator _sut = new();

    [Fact]
    public void Match_ViewModelBaseSubtype_ReturnsTrue()
    {
        // Arrange
        var viewModel = new MainWindowViewModel(Mock.Of<IWindowService>());

        // Act
        var result = _sut.Match(viewModel);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Match_PlainObject_ReturnsFalse()
    {
        // Arrange
        var data = new object();

        // Act
        var result = _sut.Match(data);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Match_String_ReturnsFalse()
    {
        // Act
        var result = _sut.Match("not a view model");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Match_Null_ReturnsFalse()
    {
        // Act
        var result = _sut.Match(null);

        // Assert
        result.Should().BeFalse();
    }

    [AvaloniaFact]
    public void Build_KnownViewModel_ReturnsMatchingView()
    {
        // Arrange
        var viewModel = new TestViewModel();

        // Type.GetType searches the calling assembly (BeatIt.dll), not BeatIt.Tests.dll.
        // Register a TypeResolve handler so the runtime can find TestView in this assembly.
        ResolveEventHandler handler = (_, _) => typeof(TestView).Assembly;
        AppDomain.CurrentDomain.TypeResolve += handler;
        try
        {
            // Act
            var result = _sut.Build(viewModel);

            // Assert
            result.Should().BeOfType<TestView>();
        }
        finally
        {
            AppDomain.CurrentDomain.TypeResolve -= handler;
        }
    }

    [AvaloniaFact]
    public void Build_UnknownViewModel_ReturnsFallbackTextBlock()
    {
        // Arrange
        var viewModel = new OrphanViewModel();

        // Act
        var result = _sut.Build(viewModel);

        // Assert
        result.Should().BeOfType<TextBlock>();
        ((TextBlock)result).Text.Should().Contain("Not Found:");
        ((TextBlock)result).Text.Should().Contain(nameof(OrphanViewModel));
    }

    [AvaloniaFact]
    public void Build_Null_ReturnsDataIsNullTextBlock()
    {
        // Act
        var result = _sut.Build(null);

        // Assert
        result.Should().BeOfType<TextBlock>();
        ((TextBlock)result).Text.Should().Be("Data is null");
    }
}

/// <summary>
/// A test view model with a matching <see cref="TestView"/> for view resolution testing.
/// </summary>
public partial class TestViewModel : ViewModelBase;

/// <summary>
/// The matching view for <see cref="TestViewModel"/>.
/// </summary>
public class TestView : UserControl;

/// <summary>
/// A view model with no corresponding view, used to test the fallback path.
/// </summary>
public partial class OrphanViewModel : ViewModelBase;
