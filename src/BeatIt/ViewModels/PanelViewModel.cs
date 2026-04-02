using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BeatIt.ViewModels;

/// <summary>
/// View model for the bottom panel area, managing tabs, height, and visibility.
/// </summary>
public partial class PanelViewModel : ViewModelBase
{
    /// <summary>
    /// The minimum height in pixels before the panel snaps closed.
    /// </summary>
    public const double MinHeight = 100.0;

    /// <summary>
    /// The default height of the panel in pixels.
    /// </summary>
    public const double DefaultHeight = 200.0;

    /// <summary>
    /// Gets the collection of tabs displayed in the panel.
    /// </summary>
    public ObservableCollection<PanelTabViewModel> Tabs { get; } = [];

    /// <summary>
    /// Gets or sets the currently selected tab.
    /// </summary>
    [ObservableProperty]
    private PanelTabViewModel? _selectedTab;

    /// <summary>
    /// Gets or sets the current height of the panel in pixels.
    /// </summary>
    [ObservableProperty]
    private double _height = DefaultHeight;

    /// <summary>
    /// Gets or sets a value indicating whether the panel is visible.
    /// </summary>
    [ObservableProperty]
    private bool _isVisible = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="PanelViewModel"/> class.
    /// </summary>
    /// <param name="outputTab">
    /// The output tab to add as the initial tab.
    /// </param>
    public PanelViewModel(OutputTabViewModel outputTab)
    {
        Tabs.Add(outputTab);
        SelectedTab = outputTab;
    }

    /// <summary>
    /// Adds a tab to the panel.
    /// </summary>
    /// <param name="tab">
    /// The tab to add.
    /// </param>
    public void AddTab(PanelTabViewModel tab)
    {
        Tabs.Add(tab);
    }

    /// <summary>
    /// Clamps the proposed height to valid bounds, snapping to zero when below <see cref="MinHeight"/>.
    /// </summary>
    /// <param name="proposedHeight">
    /// The proposed height to clamp.
    /// </param>
    /// <param name="maxHeight">
    /// The maximum allowed height.
    /// </param>
    /// <returns>
    /// Zero if <paramref name="proposedHeight"/> is less than <see cref="MinHeight"/>,
    /// <paramref name="maxHeight"/> if <paramref name="proposedHeight"/> exceeds it,
    /// or <paramref name="proposedHeight"/> otherwise.
    /// </returns>
    public static double ClampHeight(double proposedHeight, double maxHeight)
    {
        if (proposedHeight < MinHeight)
        {
            return 0;
        }

        if (proposedHeight > maxHeight)
        {
            return maxHeight;
        }

        return proposedHeight;
    }

    /// <summary>
    /// Toggles the panel visibility. When visible, collapses the panel to zero height;
    /// when collapsed, restores the panel to <see cref="DefaultHeight"/>.
    /// </summary>
    [RelayCommand]
    private void TogglePanel()
    {
        if (IsVisible)
        {
            IsVisible = false;
            Height = 0;
        }
        else
        {
            IsVisible = true;
            Height = DefaultHeight;
        }
    }
}
