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
    /// Indicates whether a panel resize drag operation is in progress.
    /// </summary>
    private bool _isPanelDragging;

    /// <summary>
    /// The Y coordinate at which the panel drag operation started.
    /// </summary>
    private double _panelDragStartY;

    /// <summary>
    /// The height of the panel when the drag operation started.
    /// </summary>
    private double _panelStartHeight;

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

        // When drag-snap collapses the sidebar, deselect the activity bar item
        // so the resize grip hides and there is no gap against the activity bar.
        if (DataContext is MainWindowViewModel vm && vm.SideBar.Width == 0)
        {
            vm.ActivityBar.SelectedItem = null;
        }

        e.Handled = true;
    }

    /// <summary>
    /// Handles pointer press on the panel resize grip to begin a vertical drag-resize operation.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The pointer pressed event arguments.</param>
    [ExcludeFromCodeCoverage(Justification = "Requires real pointer infrastructure for capture and position tracking.")]
    private void OnPanelResizeGripPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (DataContext is not MainWindowViewModel vm)
        {
            return;
        }

        _isPanelDragging = true;
        _panelDragStartY = e.GetPosition(this).Y;
        _panelStartHeight = vm.Panel.Height;
        e.Pointer.Capture(PanelResizeGrip);
        PanelResizeGrip.Classes.Add("dragging");
        e.Handled = true;
    }

    /// <summary>
    /// Handles pointer move during a panel drag-resize operation to update the panel height.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The pointer event arguments.</param>
    [ExcludeFromCodeCoverage(Justification = "Requires real pointer infrastructure for capture and position tracking.")]
    private void OnPanelResizeGripPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_isPanelDragging || DataContext is not MainWindowViewModel vm)
        {
            return;
        }

        var currentY = e.GetPosition(this).Y;
        var proposedHeight = _panelStartHeight + (_panelDragStartY - currentY);
        var maxHeight = Bounds.Height - 60;
        vm.Panel.Height = PanelViewModel.ClampHeight(proposedHeight, maxHeight);

        e.Handled = true;
    }

    /// <summary>
    /// Handles pointer release to end the panel drag-resize operation.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The pointer released event arguments.</param>
    [ExcludeFromCodeCoverage(Justification = "Requires real pointer infrastructure for capture and position tracking.")]
    private void OnPanelResizeGripPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (!_isPanelDragging)
        {
            return;
        }

        _isPanelDragging = false;
        e.Pointer.Capture(null);
        PanelResizeGrip.Classes.Remove("dragging");

        if (DataContext is MainWindowViewModel vm && vm.Panel.Height == 0)
        {
            vm.Panel.IsVisible = false;
        }

        e.Handled = true;
    }
}
