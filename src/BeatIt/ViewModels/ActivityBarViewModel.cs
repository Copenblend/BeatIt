using System.Collections.ObjectModel;

using BeatIt.Assets;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BeatIt.ViewModels;

/// <summary>
/// View model for the activity bar, managing the collection of activity bar items
/// and the currently selected item.
/// </summary>
public partial class ActivityBarViewModel : ViewModelBase
{
    /// <summary>
    /// Gets or sets the currently selected activity bar item.
    /// When changed, updates <see cref="ActivityBarItemViewModel.IsSelected"/>
    /// on the old and new items and notifies <see cref="IsSideBarVisible"/>.
    /// </summary>
    [ObservableProperty]
    private ActivityBarItemViewModel? _selectedItem;

    /// <summary>
    /// Gets the collection of activity bar items.
    /// </summary>
    public ObservableCollection<ActivityBarItemViewModel> Items { get; } = [];

    /// <summary>
    /// Gets a value indicating whether the side bar should be visible.
    /// Returns <see langword="true"/> when an item is selected.
    /// </summary>
    public bool IsSideBarVisible => SelectedItem is not null;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActivityBarViewModel"/> class.
    /// Populates the activity bar with the default Explorer item.
    /// </summary>
    public ActivityBarViewModel()
    {
        Items.Add(new ActivityBarItemViewModel(Codicons.Files, "Explorer"));
        Items.Add(new ActivityBarItemViewModel(Codicons.Search, "Search"));
    }

    /// <summary>
    /// Selects or deselects an activity bar item.
    /// If the item is already selected, it is deselected (toggled off).
    /// Otherwise, the item becomes the new selection.
    /// </summary>
    /// <param name="item">
    /// The activity bar item to select or deselect.
    /// </param>
    [RelayCommand]
    private void SelectItem(ActivityBarItemViewModel item)
    {
        SelectedItem = item == SelectedItem ? null : item;
    }

    /// <summary>
    /// Adds an item to the activity bar.
    /// </summary>
    /// <param name="item">
    /// The activity bar item to add.
    /// </param>
    public void AddItem(ActivityBarItemViewModel item)
    {
        Items.Add(item);
    }

    /// <summary>
    /// Moves an item within the activity bar from one position to another.
    /// Silently returns if either index is out of range.
    /// </summary>
    /// <param name="oldIndex">
    /// The current index of the item to move.
    /// </param>
    /// <param name="newIndex">
    /// The target index to move the item to.
    /// </param>
    public void MoveItem(int oldIndex, int newIndex)
    {
        if (oldIndex < 0 || oldIndex >= Items.Count ||
            newIndex < 0 || newIndex >= Items.Count)
        {
            return;
        }

        Items.Move(oldIndex, newIndex);
    }

    partial void OnSelectedItemChanged(ActivityBarItemViewModel? oldValue, ActivityBarItemViewModel? newValue)
    {
        if (oldValue is not null)
        {
            oldValue.IsSelected = false;
        }

        if (newValue is not null)
        {
            newValue.IsSelected = true;
        }

        OnPropertyChanged(nameof(IsSideBarVisible));
    }
}
