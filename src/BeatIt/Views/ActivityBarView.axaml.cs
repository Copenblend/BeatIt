namespace BeatIt.Views;

using System.Diagnostics.CodeAnalysis;

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

using BeatIt.ViewModels;

/// <summary>
/// Displays the activity bar with icon buttons and supports drag-and-drop reordering.
/// </summary>
public partial class ActivityBarView : UserControl
{
    /// <summary>
    /// The drag threshold in pixels before initiating a drag operation.
    /// </summary>
    private const double DragThreshold = 5.0;

    /// <summary>
    /// The item being dragged during a drag-and-drop operation.
    /// </summary>
    private ActivityBarItemViewModel? _draggedItem;

    /// <summary>
    /// The starting pointer position for drag detection.
    /// </summary>
    private Avalonia.Point? _dragStartPosition;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActivityBarView"/> class.
    /// </summary>
    public ActivityBarView()
    {
        InitializeComponent();

        AddHandler(DragDrop.DropEvent, OnDrop);
        AddHandler(DragDrop.DragOverEvent, OnDragOver);
        AddHandler(PointerPressedEvent, OnItemPointerPressed, RoutingStrategies.Tunnel);
        AddHandler(PointerMovedEvent, OnItemPointerMoved, RoutingStrategies.Tunnel);
    }

    /// <summary>
    /// Handles pointer press on an activity bar item to begin tracking a potential drag.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The pointer pressed event arguments.</param>
    [ExcludeFromCodeCoverage(Justification = "Requires real pointer and DragDrop infrastructure.")]
    private void OnItemPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed &&
            FindItemViewModel(e.Source as Control) is { } item)
        {
            _draggedItem = item;
            _dragStartPosition = e.GetPosition(this);
        }
    }

    /// <summary>
    /// Handles pointer move to initiate a drag operation when the threshold is exceeded.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The pointer event arguments.</param>
    [ExcludeFromCodeCoverage(Justification = "Requires real pointer and DragDrop infrastructure.")]
    private async void OnItemPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_draggedItem is null || _dragStartPosition is null)
        {
            return;
        }

        var currentPosition = e.GetPosition(this);
        var delta = currentPosition - _dragStartPosition.Value;

        if (System.Math.Abs(delta.Y) > DragThreshold)
        {
            var data = new DataObject();
            data.Set("ActivityBarItem", _draggedItem);

            _dragStartPosition = null;
            await DragDrop.DoDragDrop(e, data, DragDropEffects.Move);
            _draggedItem = null;
        }
    }

    /// <summary>
    /// Handles the drag-over event to indicate move is allowed.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The drag event arguments.</param>
    [ExcludeFromCodeCoverage(Justification = "Requires real pointer and DragDrop infrastructure.")]
    private static void OnDragOver(object? sender, DragEventArgs e)
    {
        e.DragEffects = DragDropEffects.Move;
    }

    /// <summary>
    /// Handles the drop event to reorder activity bar items.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The drag event arguments.</param>
    [ExcludeFromCodeCoverage(Justification = "Requires real pointer and DragDrop infrastructure.")]
    private void OnDrop(object? sender, DragEventArgs e)
    {
        if (DataContext is not ActivityBarViewModel viewModel)
        {
            return;
        }

        if (!e.Data.Contains("ActivityBarItem") ||
            e.Data.Get("ActivityBarItem") is not ActivityBarItemViewModel draggedItem)
        {
            return;
        }

        var oldIndex = viewModel.Items.IndexOf(draggedItem);
        if (oldIndex < 0)
        {
            return;
        }

        var position = e.GetPosition(ActivityBarItemsControl);
        var targetItem = FindItemAtPosition(viewModel, position.Y);
        if (targetItem is null)
        {
            return;
        }

        var newIndex = viewModel.Items.IndexOf(targetItem);
        if (newIndex >= 0 && oldIndex != newIndex)
        {
            viewModel.MoveItem(oldIndex, newIndex);
        }

        _draggedItem = null;
        _dragStartPosition = null;
    }

    /// <summary>
    /// Finds the activity bar item whose container contains the given Y position,
    /// using position-based lookup against realized containers in the
    /// <see cref="ActivityBarItemsControl"/>.
    /// </summary>
    /// <param name="viewModel">The activity bar view model containing the items collection.</param>
    /// <param name="y">The Y coordinate relative to the <see cref="ActivityBarItemsControl"/>.</param>
    /// <returns>
    /// The <see cref="ActivityBarItemViewModel"/> at the given position, the last item if
    /// the position is beyond all items, or <see langword="null"/> if no items exist.
    /// </returns>
    [ExcludeFromCodeCoverage(Justification = "View-layer helper for position-based item lookup.")]
    private ActivityBarItemViewModel? FindItemAtPosition(ActivityBarViewModel viewModel, double y)
    {
        for (var i = 0; i < viewModel.Items.Count; i++)
        {
            var container = ActivityBarItemsControl.ContainerFromIndex(i);
            if (container is null)
            {
                continue;
            }

            var bounds = container.Bounds;
            if (y >= bounds.Top && y < bounds.Bottom)
            {
                return viewModel.Items[i];
            }
        }

        // If beyond all items, return the last item
        if (viewModel.Items.Count > 0)
        {
            return viewModel.Items[^1];
        }

        return null;
    }

    /// <summary>
    /// Walks up the visual tree from the given control to find the nearest
    /// <see cref="ActivityBarItemViewModel"/> data context.
    /// </summary>
    /// <param name="control">The starting control in the visual tree.</param>
    /// <returns>
    /// The <see cref="ActivityBarItemViewModel"/> if found; otherwise, <see langword="null"/>.
    /// </returns>
    [ExcludeFromCodeCoverage(Justification = "View-layer helper for visual tree traversal.")]
    private static ActivityBarItemViewModel? FindItemViewModel(Control? control)
    {
        while (control is not null)
        {
            if (control.DataContext is ActivityBarItemViewModel item)
            {
                return item;
            }

            control = control.GetVisualParent() as Control;
        }

        return null;
    }
}
