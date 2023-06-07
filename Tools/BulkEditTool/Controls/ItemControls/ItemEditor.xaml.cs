using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using BulkEditTool.Comparers;
using BulkEditTool.Extensions;
using BulkEditTool.Model;
using BulkEditTool.Model.Observables;
using Chaos.Extensions.Common;
using Chaos.Schemas.Templates;
using MaterialDesignExtensions.Controls;
using TOOL_CONSTANTS = BulkEditTool.Definitions.CONSTANTS;

namespace BulkEditTool.Controls.ItemControls;

/// <summary>
///     Interaction logic for ItemEditor.xaml
/// </summary>
public sealed partial class ItemEditor
{
    public ObservableCollection<ObservableListItem<ItemPropertyEditor>> ItemTemplates { get; }

    public ItemEditor()
    {
        ItemTemplates = new ObservableCollection<ObservableListItem<ItemPropertyEditor>>();

        InitializeComponent();

        AddBtn.IsEnabled = false;
        GridViewBtn.IsEnabled = false;
    }

    private async void UserControl_Initialized(object sender, EventArgs e)
    {
        var loadingTask = JsonContext.LoadingTask;

        Snackbar.MessageQueue?.Enqueue(
            "Loading...",
            null,
            null,
            null,
            false,
            true,
            TimeSpan.FromHours(1));

        while (true)
        {
            await Task.Delay(250);

            if (loadingTask.IsCompleted)
                break;
        }

        Snackbar.MessageQueue?.Clear();

        var objs = JsonContext.ItemTemplates.Objects.Select(
                                  wrapper =>
                                  {
                                      var editor = new ItemPropertyEditor(ItemTemplates, wrapper);

                                      var listItem = new ObservableListItem<ItemPropertyEditor>(editor)
                                      {
                                          Key = wrapper.Obj.TemplateKey
                                      };

                                      return listItem;
                                  })
                              .OrderBy(_ => _, ObservableListItemComparer.Instance);

        ItemTemplates.AddRange(objs);

        AddBtn.IsEnabled = true;
        GridViewBtn.IsEnabled = true;
    }

    #region CTRL+F (Find)
    private void FindTbox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        var tbox = (TextBox)sender;

        if (string.IsNullOrWhiteSpace(tbox.Text))
            return;

        var text = tbox.Text;

        var currentView = PropertyEditor.Children
                                        .OfType<object>()
                                        .FirstOrDefault();

        switch (currentView)
        {
            case DataGrid dataGrid:
            {
                var searchResult = dataGrid.ItemsSource
                                           .OfType<ItemTemplateSchema>()
                                           .Select(ItemTemplateSchemaExtensions.EnumerateProperties)
                                           .SelectMany(
                                               (rowValue, rowIndex) =>
                                                   rowValue.Select((cellValue, columnIndex) => (cellValue, columnIndex, rowIndex)))
                                           .FirstOrDefault(set => set.cellValue?.ContainsI(text) == true);

                if (searchResult.cellValue is null)
                    return;

                var column = dataGrid.Columns.SingleOrDefault(col => col.DisplayIndex == searchResult.columnIndex);
                var columnIndex = dataGrid.Columns.IndexOf(column);

                dataGrid.SelectCellByIndex(searchResult.rowIndex, columnIndex, false);

                break;
            }

            default:
            {
                var searchResult = ItemTemplates
                                   .Select(listItem => listItem.Control.Item.Obj.EnumerateProperties())
                                   .SelectMany(
                                       (rowValue, rowIndex) =>
                                           rowValue.Select((cellValue, columnIndex) => (cellValue, columnIndex, rowIndex)))
                                   .FirstOrDefault(set => set.cellValue?.ContainsI(text) == true);

                if (searchResult.cellValue is null)
                    return;

                ItemTemplatesView.SelectedIndex = searchResult.rowIndex;

                break;
            }
        }
    }

    private void ItemEditor_OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        switch (e.Key)
        {
            case Key.F when Keyboard.Modifiers == ModifierKeys.Control:
                e.Handled = true;

                FindTbox.Visibility = Visibility.Visible;
                FindTbox.Focus();

                break;
            case Key.Escape when FindTbox.IsVisible:
                e.Handled = true;

                FindTbox.Text = null;
                FindTbox.Visibility = Visibility.Collapsed;
                ItemTemplatesView.Focus();

                break;
        }
    }
    #endregion

    #region ListView
    private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selected = e.AddedItems.OfType<ObservableListItem<ItemPropertyEditor>>().FirstOrDefault();

        if (selected is null)
        {
            var listView = (ListView)sender;

            if (listView is { SelectedIndex: -1, Items.IsEmpty: false })
                listView.SelectedIndex = 0;

            return;
        }

        PropertyEditor.Children.Clear();
        PropertyEditor.Children.Add(selected.Control);
    }

    private async void AddButton_Click(object sender, RoutedEventArgs e)
    {
        var path = TOOL_CONSTANTS.TEMP_PATH;
        var baseDir = JsonContext.ItemTemplates.Options.Directory;
        var fullBaseDir = Path.GetFullPath(baseDir);

        var result = await OpenDirectoryDialog.ShowDialogAsync(
            DialogHost,
            new OpenDirectoryDialogArguments { CurrentDirectory = fullBaseDir, CreateNewDirectoryEnabled = true });

        if (result is null || result.Canceled)
            return;

        path = Path.Combine(result.Directory, path);

        var template = new ItemTemplateSchema { TemplateKey = Path.GetFileNameWithoutExtension(TOOL_CONSTANTS.TEMP_PATH) };
        var wrapper = new TraceWrapper<ItemTemplateSchema>(path, template);
        var editor = new ItemPropertyEditor(ItemTemplates, wrapper);

        var listItem = new ObservableListItem<ItemPropertyEditor>(editor)
        {
            Key = template.TemplateKey
        };

        ItemTemplates.Add(listItem);

        ItemTemplatesView.SelectedItem = listItem;
    }
    #endregion

    #region DataGrid
    private async void ToggleGridViewBtn_OnClick(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        var buttonStr = button.Content.ToString()!;

        if (buttonStr.EqualsI("Grid View"))
        {
            var dataGrid = new DataGrid();
            dataGrid.AutoGeneratedColumns += DataGrid_AutoGeneratedColumns;
            dataGrid.AutoGeneratingColumn += DataGrid_AutoGeneratingColumn;
            dataGrid.SelectionUnit = DataGridSelectionUnit.Cell;

            await JsonContext.LoadingTask;

            var lcv = new ListCollectionView(JsonContext.ItemTemplates.ToList());
            dataGrid.ItemsSource = lcv;

            PropertyEditor.Children.Clear();
            PropertyEditor.Children.Add(dataGrid);
            ItemTemplatesView.Visibility = Visibility.Collapsed;
            button.Content = "List View";
        } else if (buttonStr.EqualsI("List View"))
        {
            PropertyEditor.Children.Clear();
            ItemTemplatesView.Visibility = Visibility.Visible;
            var index = ItemTemplatesView.SelectedIndex;
            ItemTemplatesView.SelectedIndex = -1;
            ItemTemplatesView.SelectedIndex = index;

            button.Content = "Grid View";
        }
    }

    private void DataGrid_AutoGeneratedColumns(object? sender, EventArgs e)
    {
        if (sender is null)
            return;

        var dataGrid = (DataGrid)sender;
        var index = 0;

        foreach (var property in TOOL_CONSTANTS.PropertyOrder)
        {
            var column = dataGrid.Columns.FirstOrDefault(c => c.Header.ToString()!.EqualsI(property));

            if (column is null)
                continue;

            column.DisplayIndex = index++;
        }
    }

    private void DataGrid_AutoGeneratingColumn(object? sender, DataGridAutoGeneratingColumnEventArgs e)
    {
        if (sender is null)
            return;

        var dataGrid = (DataGrid)sender;

        if (e.PropertyName.EqualsI(nameof(ItemTemplateSchema.Modifiers)))
        {
            e.Cancel = true;

            foreach (var propertyName in TOOL_CONSTANTS.ModifierProperties)
            {
                var column = new DataGridTextColumn
                {
                    Header = propertyName,
                    Binding = new Binding($"{nameof(ItemTemplateSchema.Modifiers)}.{propertyName}")
                    {
                        ValidatesOnDataErrors = true
                    }
                };

                dataGrid.Columns.Add(column);
            }
        } else if (!TOOL_CONSTANTS.PropertyOrder.ContainsI(e.PropertyName))
            e.Cancel = true;
    }
    #endregion
}