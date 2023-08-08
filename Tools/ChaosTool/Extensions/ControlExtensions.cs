using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace ChaosTool.Extensions;

internal static class ControlExtensions
{
    internal static T? FindVisualChild<T>(this DependencyObject obj) where T: DependencyObject
    {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
        {
            var child = VisualTreeHelper.GetChild(obj, i);

            if (child is T dependencyObject)
                return dependencyObject;

            var childOfChild = FindVisualChild<T>(child);

            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (childOfChild is not null)
                return childOfChild;
        }

        return null;
    }

    internal static T? FindVisualParent<T>(this DependencyObject child) where T: DependencyObject
    {
        while (true)
        {
            var parentObject = VisualTreeHelper.GetParent(child);

            switch (parentObject)
            {
                case null:
                    return null;
                case T parent:
                    return parent;
                default:
                    child = parentObject;

                    break;
            }
        }
    }

    internal static DataGridCell? GetCell(this DataGrid dataGrid, DataGridRow? rowContainer, int column)
    {
        if (rowContainer != null)
        {
            var presenter = FindVisualChild<DataGridCellsPresenter>(rowContainer);

            if (presenter == null)
            {
                /* if the row has been virtualized away, call its ApplyTemplate() method
                 * to build its visual tree in order for the DataGridCellsPresenter
                 * and the DataGridCells to be created */
                rowContainer.ApplyTemplate();
                presenter = FindVisualChild<DataGridCellsPresenter>(rowContainer);
            }

            if (presenter != null)
            {
                if (presenter.ItemContainerGenerator.ContainerFromIndex(column) is not DataGridCell cell)
                {
                    /* bring the column into view
                     * in case it has been virtualized away */
                    dataGrid.ScrollIntoView(rowContainer, dataGrid.Columns[column]);
                    cell = (presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell)!;
                }

                return cell;
            }
        }

        return null;
    }

    internal static void SelectCellByIndex(
        this DataGrid dataGrid,
        int rowIndex,
        int columnIndex,
        bool focus = true
    )
    {
        if (!dataGrid.SelectionUnit.Equals(DataGridSelectionUnit.Cell))
            throw new ArgumentException("The SelectionUnit of the DataGrid must be set to Cell.");

        if ((rowIndex < 0) || (rowIndex > dataGrid.Items.Count - 1))
            throw new ArgumentException($"{rowIndex} is an invalid row index.");

        if ((columnIndex < 0) || (columnIndex > dataGrid.Columns.Count - 1))
            throw new ArgumentException($"{columnIndex} is an invalid column index.");

        dataGrid.SelectedCells.Clear();

        var item = dataGrid.Items[rowIndex]!;
        // ReSharper disable once UseNegatedPatternMatching
        var row = dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;

        if (row == null)
        {
            dataGrid.ScrollIntoView(item);
            row = dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;
        }

        if (row != null)
        {
            var cell = GetCell(dataGrid, row, columnIndex);

            if (cell != null)
            {
                var dataGridCellInfo = new DataGridCellInfo(cell);
                dataGrid.SelectedCells.Add(dataGridCellInfo);

                if (focus)
                    cell.Focus();
            }
        }
    }
}