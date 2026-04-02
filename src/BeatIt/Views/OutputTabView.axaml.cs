namespace BeatIt.Views;

using System.Diagnostics.CodeAnalysis;
using System.Text;

using Avalonia.Controls;
using Avalonia.Interactivity;

using BeatIt.ViewModels;

[ExcludeFromCodeCoverage(Justification = "XAML view code-behind contains only clipboard interaction with no testable logic.")]
public partial class OutputTabView : UserControl
{
    public OutputTabView()
    {
        InitializeComponent();
    }

    private async void OnCopySelectionClick(object? sender, RoutedEventArgs e)
    {
        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        if (clipboard is null)
            return;

        var sb = new StringBuilder();
        foreach (var item in LogEntryListBox.SelectedItems!)
        {
            if (item is LogEntryViewModel entry)
                sb.AppendLine($"{entry.Timestamp:HH:mm:ss.fff} {entry.Level,-5} {entry.Message}");
        }

        await clipboard.SetTextAsync(sb.ToString());
    }

    private async void OnCopyAllClick(object? sender, RoutedEventArgs e)
    {
        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        if (clipboard is null)
            return;

        var sb = new StringBuilder();
        foreach (var item in LogEntryListBox.Items)
        {
            if (item is LogEntryViewModel entry)
                sb.AppendLine($"{entry.Timestamp:HH:mm:ss.fff} {entry.Level,-5} {entry.Message}");
        }

        await clipboard.SetTextAsync(sb.ToString());
    }
}
