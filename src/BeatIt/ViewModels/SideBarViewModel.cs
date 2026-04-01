using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BeatIt.ViewModels;

/// <summary>
/// View model for the side bar panel, managing its width, content, and resize behavior.
/// </summary>
public partial class SideBarViewModel : ViewModelBase
{
    /// <summary>
    /// The minimum width in pixels before the side bar snaps closed.
    /// </summary>
    public const double MinWidth = 170.0;

    /// <summary>
    /// The default width of the side bar in pixels.
    /// </summary>
    public const double DefaultWidth = 250.0;

    /// <summary>
    /// Gets or sets the current width of the side bar in pixels.
    /// </summary>
    [ObservableProperty]
    private double _width = DefaultWidth;

    /// <summary>
    /// Gets or sets the content view model displayed in the side bar.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasContent))]
    private ViewModelBase? _sideBarContent;

    /// <summary>
    /// Gets a value indicating whether the side bar has content to display.
    /// </summary>
    public bool HasContent => SideBarContent is not null;

    /// <summary>
    /// Clamps the proposed width to valid bounds, snapping to zero when below <see cref="MinWidth"/>.
    /// </summary>
    /// <param name="proposedWidth">
    /// The proposed width to clamp.
    /// </param>
    /// <param name="maxWidth">
    /// The maximum allowed width.
    /// </param>
    /// <returns>
    /// Zero if <paramref name="proposedWidth"/> is less than <see cref="MinWidth"/>,
    /// <paramref name="maxWidth"/> if <paramref name="proposedWidth"/> exceeds it,
    /// or <paramref name="proposedWidth"/> otherwise.
    /// </returns>
    public static double ClampWidth(double proposedWidth, double maxWidth)
    {
        if (proposedWidth < MinWidth)
        {
            return 0;
        }

        if (proposedWidth > maxWidth)
        {
            return maxWidth;
        }

        return proposedWidth;
    }

    /// <summary>
    /// Placeholder command for opening a folder. Implementation deferred to Slice 8.
    /// </summary>
    [RelayCommand]
    private void OpenFolder()
    {
    }
}
