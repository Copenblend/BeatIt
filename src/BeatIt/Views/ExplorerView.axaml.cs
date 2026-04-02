namespace BeatIt.Views;

using System.Diagnostics.CodeAnalysis;

using Avalonia.Controls;

/// <summary>
/// Displays the explorer file tree for browsing opened folder contents.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "XAML view code-behind contains only InitializeComponent with no testable logic.")]
public partial class ExplorerView : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExplorerView"/> class.
    /// </summary>
    public ExplorerView()
    {
        InitializeComponent();
    }
}
