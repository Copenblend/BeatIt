namespace BeatIt.Views;

using System.Diagnostics.CodeAnalysis;

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
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
    }

    /// <summary>
    /// Handles pointer press on an activity bar item to begin tracking a potential drag.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The pointer pressed event arguments.</param>
    [ExcludeFromCodeCoverage(Justification = "Requires real pointer and DragDrop infrastructure.")]
    private void OnItemPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Control control &&
            control.DataContext is ActivityBarItemViewModel item &&
            e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
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

        var targetItem = (e.Source as Control)?.FindAncestorOfType<Button>()?.DataContext as ActivityBarItemViewModel;
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
}
