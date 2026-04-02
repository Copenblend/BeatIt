using System.Collections.ObjectModel;
using System.Collections.Specialized;
using BeatIt.Logging;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BeatIt.ViewModels;

/// <summary>
/// View model for the Output panel tab, displaying filtered log entries with bounded capacity.
/// </summary>
public partial class OutputTabViewModel : PanelTabViewModel
{
    /// <summary>
    /// The maximum number of log entries retained before the oldest entries are removed.
    /// </summary>
    public const int MaxEntries = 10_000;

    private readonly ObservableCollection<LogEntryViewModel> _allEntries = [];

    /// <summary>
    /// Gets or sets the current minimum log level used to filter displayed entries.
    /// </summary>
    [ObservableProperty]
    private LogLevel _selectedLogLevel = LogLevel.Trace;

    /// <summary>
    /// Gets the filtered log entries matching the current <see cref="SelectedLogLevel"/>.
    /// </summary>
    public ObservableCollection<LogEntryViewModel> FilteredEntries { get; } = [];

    /// <summary>
    /// Gets the available log levels for filtering, suitable for combo box binding.
    /// </summary>
    public static IReadOnlyList<LogLevel> AvailableLogLevels { get; } = Enum.GetValues<LogLevel>();

    /// <summary>
    /// Initializes a new instance of the <see cref="OutputTabViewModel"/> class.
    /// </summary>
    public OutputTabViewModel()
        : base("Output")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OutputTabViewModel"/> class
    /// wired to an <see cref="ILogSink"/> for automatic log entry observation.
    /// </summary>
    /// <param name="logSink">
    /// The log sink whose entries will be observed and displayed.
    /// </param>
    public OutputTabViewModel(ILogSink logSink)
        : base("Output")
    {
        ((INotifyCollectionChanged)logSink.Entries).CollectionChanged += OnLogSinkEntriesChanged;
    }

    private void OnLogSinkEntriesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add when e.NewItems is not null:
                foreach (LogEntry entry in e.NewItems)
                {
                    AddEntry(new LogEntryViewModel(entry.Timestamp, entry.Level, entry.Message));
                }

                break;

            case NotifyCollectionChangedAction.Reset:
                _allEntries.Clear();
                FilteredEntries.Clear();
                break;
        }
    }

    /// <summary>
    /// Adds a log entry to the output, enforcing the <see cref="MaxEntries"/> capacity limit.
    /// </summary>
    /// <param name="entry">
    /// The log entry to add.
    /// </param>
    public void AddEntry(LogEntryViewModel entry)
    {
        _allEntries.Add(entry);

        if (_allEntries.Count > MaxEntries)
        {
            var oldest = _allEntries[0];
            _allEntries.RemoveAt(0);

            if (oldest.Level >= SelectedLogLevel)
            {
                FilteredEntries.Remove(oldest);
            }
        }

        if (entry.Level >= SelectedLogLevel)
        {
            FilteredEntries.Add(entry);
        }
    }

    /// <summary>
    /// Re-applies the log level filter when <see cref="SelectedLogLevel"/> changes.
    /// </summary>
    /// <param name="value">
    /// The new log level filter value.
    /// </param>
    partial void OnSelectedLogLevelChanged(LogLevel value)
    {
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        FilteredEntries.Clear();

        foreach (var entry in _allEntries)
        {
            if (entry.Level >= SelectedLogLevel)
            {
                FilteredEntries.Add(entry);
            }
        }
    }
}
