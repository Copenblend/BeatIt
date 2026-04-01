namespace BeatIt;

using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using BeatIt.ViewModels;

/// <summary>
/// Maps view model types to their corresponding view types by naming convention.
/// Replaces "ViewModel" with "View" in the fully qualified type name to locate the view type.
/// </summary>
public class ViewLocator : IDataTemplate
{
    /// <summary>
    /// Checks whether the given data object is a view model that can be mapped to a view.
    /// </summary>
    /// <param name="data">The data object to check.</param>
    /// <returns><c>true</c> if <paramref name="data"/> is a <see cref="ViewModelBase"/>; otherwise, <c>false</c>.</returns>
    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }

    /// <summary>
    /// Creates the view for the given view model by resolving the type name convention.
    /// </summary>
    /// <param name="data">The view model instance to create a view for.</param>
    /// <returns>
    /// The resolved view <see cref="Control"/>, or a <see cref="TextBlock"/> with an error message
    /// if the view type cannot be found or <paramref name="data"/> is <c>null</c>.
    /// </returns>
    public Control Build(object? data)
    {
        if (data is null)
        {
            return new TextBlock { Text = "Data is null" };
        }

        var viewModelTypeName = data.GetType().FullName!;
        var viewTypeName = viewModelTypeName.Replace("ViewModel", "View", StringComparison.Ordinal);
        var viewType = Type.GetType(viewTypeName);

        if (viewType is not null)
        {
            return (Control)Activator.CreateInstance(viewType)!;
        }

        return new TextBlock { Text = $"Not Found: {viewModelTypeName}" };
    }
}
