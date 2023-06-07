using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using BulkEditTool.Comparers;
using BulkEditTool.Extensions;
using BulkEditTool.Model;
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
    public ObservableCollection<ListViewItem<ItemTemplateSchema, ItemPropertyEditor>> ListViewItems { get; }

    public ItemEditor()
    {
        ListViewItems = new ObservableCollection<ListViewItem<ItemTemplateSchema, ItemPropertyEditor>>();

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
                                  wrapper => new ListViewItem<ItemTemplateSchema, ItemPropertyEditor>
                                  {
                                      Name = wrapper.Object.TemplateKey,
                                      Wrapper = wrapper
                                  })
                              .OrderBy(_ => _, ListViewItemComparer.Instance);

        ListViewItems.AddRange(objs);

        var findCommand = new RoutedCommand();
        var escCommand = new RoutedCommand();

        var findInputBinding = new InputBinding(findCommand, new KeyGesture(Key.F, ModifierKeys.Control));
        var escInputBinding = new InputBinding(escCommand, new KeyGesture(Key.Escape));

        var findCmdBinding = new CommandBinding(findCommand, Show_FindTbox);
        var escCmdBinding = new CommandBinding(escCommand, Hide_FindTBox);

        InputBindings.Add(findInputBinding);
        InputBindings.Add(escInputBinding);
        CommandBindings.Add(findCmdBinding);
        CommandBindings.Add(escCmdBinding);

        AddBtn.IsEnabled = true;
        GridViewBtn.IsEnabled = true;
    }

    #region CTRL+F (Find)
    private void Show_FindTbox(object sender, RoutedEventArgs e)
    {
        e.Handled = true;

        FindTbox.Visibility = Visibility.Visible;
        FindTbox.Focus();
        FindTbox.SelectAll();
    }

    private void Hide_FindTBox(object sender, RoutedEventArgs e)
    {
        e.Handled = true;

        FindTbox.Text = string.Empty;
        FindTbox.Visibility = Visibility.Collapsed;

        if (ItemTemplatesView.IsVisible)
            Keyboard.Focus(ItemTemplatesView);
        else
        {
            var currentView = PropertyEditor.Children
                                            .OfType<Control>()
                                            .FirstOrDefault();

            if (currentView is not null)
                Keyboard.Focus(currentView);
        }
    }

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
                var searchResult = ListViewItems
                                   .Select(listItem => listItem.Object.EnumerateProperties())
                                   .SelectMany(
                                       (rowValue, rowIndex) =>
                                           rowValue.Select((cellValue, columnIndex) => (cellValue, columnIndex, rowIndex)))
                                   .FirstOrDefault(set => set.cellValue?.ContainsI(text) == true);

                if (searchResult.cellValue is null)
                    return;

                var selected = ListViewItems[searchResult.rowIndex];
                ItemTemplatesView.SelectedItem = selected;

                break;
            }
        }
    }
    #endregion

    #region ListView
    private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selected = e.AddedItems.OfType<ListViewItem<ItemTemplateSchema, ItemPropertyEditor>>().FirstOrDefault();

        if (selected is null)
        {
            var listView = (ListView)sender;

            if (listView is { SelectedIndex: -1, Items.IsEmpty: false })
                listView.SelectedIndex = 0;

            return;
        }

        selected.Control ??= new ItemPropertyEditor(selected);
        selected.Control.DeleteBtn.Click += DeleteBtnOnClick;

        PropertyEditor.Children.Clear();
        PropertyEditor.Children.Add(selected.Control);
    }

    private void DeleteBtnOnClick(object sender, RoutedEventArgs e)
    {
        if (ItemTemplatesView.SelectedItem is not ListViewItem<ItemTemplateSchema, ItemPropertyEditor> selected)
            return;

        ListViewItems.Remove(selected);
        JsonContext.ItemTemplates.Remove(selected.Name);
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

        var listItem = new ListViewItem<ItemTemplateSchema, ItemPropertyEditor>
        {
            Name = TOOL_CONSTANTS.TEMP_PATH,
            Wrapper = wrapper
        };

        ListViewItems.Add(listItem);

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