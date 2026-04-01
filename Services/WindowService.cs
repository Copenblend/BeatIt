namespace BeatIt.Services;

using System.Diagnostics.CodeAnalysis;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

/// <summary>
/// Provides window management operations by delegating to the Avalonia main window.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Requires a running Avalonia desktop application with a windowing system. All methods delegate to Application.Current.ApplicationLifetime which is not available in test environments.")]
public sealed class WindowService : IWindowService
{
    /// <inheritdoc />
    public bool IsMaximized => GetMainWindow().WindowState == WindowState.Maximized;

    /// <inheritdoc />
    public void Minimize()
    {
        GetMainWindow().WindowState = WindowState.Minimized;
    }

    /// <inheritdoc />
    public void MaximizeRestore()
    {
        var window = GetMainWindow();
        window.WindowState = window.WindowState == WindowState.Maximized
            ? WindowState.Normal
            : WindowState.Maximized;
    }

    /// <inheritdoc />
    public void Close()
    {
        GetMainWindow().Close();
    }

    private static Window GetMainWindow()
    {
        return ((IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!).MainWindow!;
    }
}
