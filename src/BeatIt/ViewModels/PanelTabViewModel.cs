using CommunityToolkit.Mvvm.ComponentModel;

namespace BeatIt.ViewModels;

/// <summary>
/// Abstract base class for view models representing a tab within the panel area.
/// </summary>
public abstract partial class PanelTabViewModel : ViewModelBase
{
    /// <summary>
    /// Gets or sets the display title of this tab.
    /// </summary>
    [ObservableProperty]
    private string _title;

    /// <summary>
    /// Initializes a new instance of the <see cref="PanelTabViewModel"/> class.
    /// </summary>
    /// <param name="title">
    /// The display title of this tab.
    /// </param>
    protected PanelTabViewModel(string title)
    {
        _title = title;
    }
}
