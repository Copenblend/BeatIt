using BeatIt.Assets;
using BeatIt.ViewModels;
using FluentAssertions;
using Xunit;

namespace BeatIt.Tests.ViewModels;

/// <summary>
/// Unit tests for <see cref="ActivityBarViewModel"/>.
/// Verifies item management, selection logic, and property change notifications.
/// </summary>
public sealed class ActivityBarViewModelTests
{
    [Fact]
    public void Constructor_PopulatesItemsWithExplorerItem()
    {
        // Arrange & Act
        var sut = new ActivityBarViewModel();

        // Assert
        sut.Items.Should().HaveCount(2);
        sut.Items[0].Label.Should().Be("Explorer");
        sut.Items[0].IconGlyph.Should().Be(Codicons.Files);
        sut.Items[1].Label.Should().Be("Search");
        sut.Items[1].IconGlyph.Should().Be(Codicons.Search);
    }

    [Fact]
    public void SelectedItem_IsNullByDefault()
    {
        // Arrange & Act
        var sut = new ActivityBarViewModel();

        // Assert
        sut.SelectedItem.Should().BeNull();
    }

    [Fact]
    public void IsSideBarVisible_IsFalseByDefault()
    {
        // Arrange & Act
        var sut = new ActivityBarViewModel();

        // Assert
        sut.IsSideBarVisible.Should().BeFalse();
    }

    [Fact]
    public void SelectItemCommand_WithUnselectedItem_SelectsItem()
    {
        // Arrange
        var sut = new ActivityBarViewModel();
        var item = sut.Items[0];

        // Act
        sut.SelectItemCommand.Execute(item);

        // Assert
        sut.SelectedItem.Should().BeSameAs(item);
    }

    [Fact]
    public void SelectItemCommand_WithUnselectedItem_SetsIsSideBarVisibleTrue()
    {
        // Arrange
        var sut = new ActivityBarViewModel();
        var item = sut.Items[0];

        // Act
        sut.SelectItemCommand.Execute(item);

        // Assert
        sut.IsSideBarVisible.Should().BeTrue();
    }

    [Fact]
    public void SelectItemCommand_WithAlreadySelectedItem_DeselectsItem()
    {
        // Arrange
        var sut = new ActivityBarViewModel();
        var item = sut.Items[0];
        sut.SelectItemCommand.Execute(item);

        // Act
        sut.SelectItemCommand.Execute(item);

        // Assert
        sut.SelectedItem.Should().BeNull();
    }

    [Fact]
    public void SelectItemCommand_Deselecting_SetsIsSideBarVisibleFalse()
    {
        // Arrange
        var sut = new ActivityBarViewModel();
        var item = sut.Items[0];
        sut.SelectItemCommand.Execute(item);

        // Act
        sut.SelectItemCommand.Execute(item);

        // Assert
        sut.IsSideBarVisible.Should().BeFalse();
    }

    [Fact]
    public void SelectItemCommand_WithDifferentItem_SwitchesSelection()
    {
        // Arrange
        var sut = new ActivityBarViewModel();
        var first = sut.Items[0];
        var second = sut.Items[1];
        sut.SelectItemCommand.Execute(first);

        // Act
        sut.SelectItemCommand.Execute(second);

        // Assert
        sut.SelectedItem.Should().BeSameAs(second);
    }

    [Fact]
    public void SelectItemCommand_SwitchingSelection_UpdatesIsSelectedOnBothItems()
    {
        // Arrange
        var sut = new ActivityBarViewModel();
        var first = sut.Items[0];
        var second = sut.Items[1];
        sut.SelectItemCommand.Execute(first);

        // Act
        sut.SelectItemCommand.Execute(second);

        // Assert
        first.IsSelected.Should().BeFalse();
        second.IsSelected.Should().BeTrue();
    }

    [Fact]
    public void SelectItemCommand_SelectingItem_SetsIsSelectedTrue()
    {
        // Arrange
        var sut = new ActivityBarViewModel();
        var item = sut.Items[0];

        // Act
        sut.SelectItemCommand.Execute(item);

        // Assert
        item.IsSelected.Should().BeTrue();
    }

    [Fact]
    public void SelectItemCommand_DeselectingItem_SetsIsSelectedFalse()
    {
        // Arrange
        var sut = new ActivityBarViewModel();
        var item = sut.Items[0];
        sut.SelectItemCommand.Execute(item);

        // Act
        sut.SelectItemCommand.Execute(item);

        // Assert
        item.IsSelected.Should().BeFalse();
    }

    [Fact]
    public void AddItem_AddsItemToCollection()
    {
        // Arrange
        var sut = new ActivityBarViewModel();
        var item = new ActivityBarItemViewModel("\uea6d", "Search");

        // Act
        sut.AddItem(item);

        // Assert
        sut.Items.Should().HaveCount(3);
        sut.Items[2].Should().BeSameAs(item);
    }

    [Fact]
    public void MoveItem_ReordersItemsCorrectly()
    {
        // Arrange
        var sut = new ActivityBarViewModel();
        var first = sut.Items[0];
        var second = sut.Items[1];

        // Act
        sut.MoveItem(0, 1);

        // Assert
        sut.Items[0].Should().BeSameAs(second);
        sut.Items[1].Should().BeSameAs(first);
    }

    [Fact]
    public void MoveItem_WithNegativeOldIndex_DoesNothing()
    {
        // Arrange
        var sut = new ActivityBarViewModel();
        var expected = sut.Items.ToList();

        // Act
        sut.MoveItem(-1, 0);

        // Assert
        sut.Items.Should().HaveCount(2);
        sut.Items.Should().ContainInOrder(expected);
    }

    [Fact]
    public void MoveItem_WithOldIndexOutOfRange_DoesNothing()
    {
        // Arrange
        var sut = new ActivityBarViewModel();
        var expected = sut.Items.ToList();

        // Act
        sut.MoveItem(5, 0);

        // Assert
        sut.Items.Should().HaveCount(2);
        sut.Items.Should().ContainInOrder(expected);
    }

    [Fact]
    public void MoveItem_WithNegativeNewIndex_DoesNothing()
    {
        // Arrange
        var sut = new ActivityBarViewModel();
        var expected = sut.Items.ToList();

        // Act
        sut.MoveItem(0, -1);

        // Assert
        sut.Items.Should().HaveCount(2);
        sut.Items.Should().ContainInOrder(expected);
    }

    [Fact]
    public void MoveItem_WithNewIndexOutOfRange_DoesNothing()
    {
        // Arrange
        var sut = new ActivityBarViewModel();
        var expected = sut.Items.ToList();

        // Act
        sut.MoveItem(0, 5);

        // Assert
        sut.Items.Should().HaveCount(2);
        sut.Items.Should().ContainInOrder(expected);
    }

    [Fact]
    public void SelectedItem_Change_RaisesPropertyChangedForSelectedItem()
    {
        // Arrange
        var sut = new ActivityBarViewModel();
        var raised = false;
        sut.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(ActivityBarViewModel.SelectedItem))
            {
                raised = true;
            }
        };

        // Act
        sut.SelectItemCommand.Execute(sut.Items[0]);

        // Assert
        raised.Should().BeTrue();
    }

    [Fact]
    public void SelectedItem_Change_RaisesPropertyChangedForIsSideBarVisible()
    {
        // Arrange
        var sut = new ActivityBarViewModel();
        var raised = false;
        sut.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(ActivityBarViewModel.IsSideBarVisible))
            {
                raised = true;
            }
        };

        // Act
        sut.SelectItemCommand.Execute(sut.Items[0]);

        // Assert
        raised.Should().BeTrue();
    }
}
