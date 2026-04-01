namespace BeatIt.Views;

using System.Diagnostics.CodeAnalysis;

using Avalonia.Controls;
using Avalonia.Input;
using BeatIt.ViewModels;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Main application window. Provides custom title bar drag and double-click behavior.
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// Indicates whether a resize drag operation is in progress.
    /// </summary>
    private bool _isDragging;

    /// <summary>
    /// The X coordinate at which the drag operation started.
    /// </summary>
    private double _dragStartX;

    /// <summary>
    /// The width of the side bar when the drag operation started.
    /// </summary>
    private double _startWidth;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();

        var statusBarView = this.FindControl<StatusBarView>("StatusBarZone");
        if (statusBarView is not null)
        {
            statusBarView.DataContext = App.Services.GetRequiredService<StatusBarViewModel>();
        }
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

    /// <summary>
    /// Handles pointer press on the resize grip to begin a drag-resize operation.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The pointer pressed event arguments.</param>
    [ExcludeFromCodeCoverage(Justification = "Requires real pointer infrastructure for capture and position tracking.")]
    private void OnResizeGripPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (DataContext is not MainWindowViewModel vm)
        {
            return;
        }

        _isDragging = true;
        _dragStartX = e.GetPosition(this).X;
        _startWidth = vm.SideBar.Width;
        e.Pointer.Capture(ResizeGrip);
        ResizeGrip.Classes.Add("dragging");
        e.Handled = true;
    }

    /// <summary>
    /// Handles pointer move during a drag-resize operation to update the side bar width.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The pointer event arguments.</param>
    [ExcludeFromCodeCoverage(Justification = "Requires real pointer infrastructure for capture and position tracking.")]
    private void OnResizeGripPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_isDragging || DataContext is not MainWindowViewModel vm)
        {
            return;
        }

        var currentX = e.GetPosition(this).X;
        var proposedWidth = _startWidth + (currentX - _dragStartX);
        var maxWidth = Bounds.Width * 0.8;
        vm.SideBar.Width = SideBarViewModel.ClampWidth(proposedWidth, maxWidth);
        e.Handled = true;
    }

    /// <summary>
    /// Handles pointer release to end the drag-resize operation.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The pointer released event arguments.</param>
    [ExcludeFromCodeCoverage(Justification = "Requires real pointer infrastructure for capture and position tracking.")]
    private void OnResizeGripPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (!_isDragging)
        {
            return;
        }

        _isDragging = false;
        e.Pointer.Capture(null);
        ResizeGrip.Classes.Remove("dragging");
        e.Handled = true;
    }
}
