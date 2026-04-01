namespace BeatIt.Views;

using System.Diagnostics.CodeAnalysis;

using Avalonia.Controls;
using Avalonia.Input;

/// <summary>
/// Main application window. Provides custom title bar drag and double-click behavior.
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Handles pointer press on the title bar drag region to initiate window move
    /// or toggle maximize/restore on double-click.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The pointer pressed event arguments.</param>
    [ExcludeFromCodeCoverage(Justification = "Requires a real windowing system for BeginMoveDrag and pointer events.")]
    private void TitleBarDragRegion_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            if (e.ClickCount == 2)
            {
                WindowState = WindowState == WindowState.Maximized
                    ? WindowState.Normal
                    : WindowState.Maximized;
            }
            else
            {
                BeginMoveDrag(e);
            }
        }
    }
}
