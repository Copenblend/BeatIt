using CommunityToolkit.Mvvm.ComponentModel;

namespace BeatIt.ViewModels;

/// <summary>
/// View model representing a single item in the activity bar.
/// Acts as a data holder for the item's icon, label, and selection state.
/// Selection logic is driven by the parent <see cref="ActivityBarViewModel"/>.
/// </summary>
public partial class ActivityBarItemViewModel : ViewModelBase
{
    /// <summary>
    /// Gets or sets the Codicon Unicode glyph character for this item's icon.
    /// </summary>
    [ObservableProperty]
    private string _iconGlyph;

    /// <summary>
    /// Gets or sets the tooltip label for this item.
    /// </summary>
    [ObservableProperty]
    private string _label;

    /// <summary>
    /// Gets or sets a value indicating whether this item is currently selected.
    /// </summary>
    [ObservableProperty]
    private bool _isSelected;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActivityBarItemViewModel"/> class.
    /// </summary>
    /// <param name="iconGlyph">
    /// The Codicon Unicode glyph character for this item's icon.
    /// </param>
    /// <param name="label">
    /// The tooltip label for this item.
    /// </param>
    public ActivityBarItemViewModel(string iconGlyph, string label)
    {
        _iconGlyph = iconGlyph;
        _label = label;
    }
}
