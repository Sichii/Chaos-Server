using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using BulkEditTool.Comparers;
using BulkEditTool.Model;
using BulkEditTool.Model.Observables;
using Chaos.Extensions.Common;
using Chaos.Schemas.Aisling;
using Chaos.Schemas.Templates;
using MaterialDesignExtensions.Controls;

namespace BulkEditTool.Controls.ItemControls;

/// <summary>
///     Interaction logic for ItemEditor.xaml
/// </summary>
public sealed partial class ItemEditor
{
    private const string TEMP_PATH = "NEW.json";

    private static readonly List<string> ModifierProperties = new()
    {
        nameof(AttributesSchema.AtkSpeedPct),
        nameof(AttributesSchema.Ac),
        nameof(AttributesSchema.MagicResistance),
        nameof(AttributesSchema.Hit),
        nameof(AttributesSchema.Dmg),
        nameof(AttributesSchema.FlatSkillDamage),
        nameof(AttributesSchema.FlatSpellDamage),
        nameof(AttributesSchema.SkillDamagePct),
        nameof(AttributesSchema.SpellDamagePct),
        nameof(AttributesSchema.MaximumHp),
        nameof(AttributesSchema.MaximumMp),
        nameof(AttributesSchema.Str),
        nameof(AttributesSchema.Int),
        nameof(AttributesSchema.Wis),
        nameof(AttributesSchema.Con),
        nameof(AttributesSchema.Dex)
    };
    private static readonly List<string> PropertyOrder = new()
    {
        nameof(ItemTemplateSchema.TemplateKey),
        nameof(ItemTemplateSchema.Name),
        nameof(ItemTemplateSchema.PanelSprite),
        nameof(ItemTemplateSchema.DisplaySprite),
        nameof(ItemTemplateSchema.Color),
        nameof(ItemTemplateSchema.PantsColor),
        nameof(ItemTemplateSchema.MaxStacks),
        nameof(ItemTemplateSchema.AccountBound),
        nameof(ItemTemplateSchema.BuyCost),
        nameof(ItemTemplateSchema.SellValue),
        nameof(ItemTemplateSchema.Weight),
        nameof(ItemTemplateSchema.MaxDurability),
        nameof(ItemTemplateSchema.Class),
        nameof(ItemTemplateSchema.AdvClass),
        nameof(ItemTemplateSchema.Level),
        nameof(ItemTemplateSchema.RequiresMaster),
        nameof(AttributesSchema.AtkSpeedPct),
        nameof(AttributesSchema.Ac),
        nameof(AttributesSchema.MagicResistance),
        nameof(AttributesSchema.Hit),
        nameof(AttributesSchema.Dmg),
        nameof(AttributesSchema.FlatSkillDamage),
        nameof(AttributesSchema.FlatSpellDamage),
        nameof(AttributesSchema.SkillDamagePct),
        nameof(AttributesSchema.SpellDamagePct),
        nameof(AttributesSchema.MaximumHp),
        nameof(AttributesSchema.MaximumMp),
        nameof(AttributesSchema.Str),
        nameof(AttributesSchema.Int),
        nameof(AttributesSchema.Wis),
        nameof(AttributesSchema.Con),
        nameof(AttributesSchema.Dex),
        nameof(ItemTemplateSchema.CooldownMs),
        nameof(ItemTemplateSchema.EquipmentType),
        nameof(ItemTemplateSchema.Gender),
        nameof(ItemTemplateSchema.IsDyeable),
        nameof(ItemTemplateSchema.IsModifiable),
        nameof(ItemTemplateSchema.Category),
        nameof(ItemTemplateSchema.Description)
    };
    public ObservableCollection<ObservableListItem<ItemPropertyEditor>> ItemTemplates { get; }

    public ItemEditor()
    {
        ItemTemplates = new ObservableCollection<ObservableListItem<ItemPropertyEditor>>();

        InitializeComponent();
    }

    private async void AddButton_Click(object sender, RoutedEventArgs e)
    {
        var path = TEMP_PATH;
        var baseDir = JsonContext.ItemTemplates.Options.Directory;
        var fullBaseDir = Path.GetFullPath(baseDir);

        var result = await OpenDirectoryDialog.ShowDialogAsync(
            DialogHost,
            new OpenDirectoryDialogArguments { CurrentDirectory = fullBaseDir });

        if (result is null || result.Canceled)
            return;

        path = Path.Combine(result.Directory, path);
        //path = Path.GetRelativePath(Environment.CurrentDirectory, path);

        var template = new ItemTemplateSchema { TemplateKey = Path.GetFileNameWithoutExtension(TEMP_PATH) };
        var wrapper = new TraceWrapper<ItemTemplateSchema>(path, template);
        var editor = new ItemPropertyEditor(ItemTemplates, wrapper);

        var listItem = new ObservableListItem<ItemPropertyEditor>(editor)
        {
            Key = template.TemplateKey
        };

        ItemTemplates.Add(listItem);

        ItemTemplatesView.SelectedItem = listItem;
    }

    private IEnumerable<string?> ConvertToRow(ItemTemplateSchema schema)
    {
        var modifiers = schema.Modifiers;

        yield return schema.TemplateKey;
        yield return schema.Name;
        yield return schema.PanelSprite.ToString();
        yield return schema.DisplaySprite?.ToString();
        yield return schema.Color.ToString();
        yield return schema.PantsColor?.ToString();
        yield return schema.MaxStacks.ToString();
        yield return schema.AccountBound.ToString();
        yield return schema.BuyCost.ToString();
        yield return schema.SellValue.ToString();
        yield return schema.Weight.ToString();
        yield return schema.MaxDurability?.ToString();
        yield return schema.Class?.ToString();
        yield return schema.AdvClass?.ToString();
        yield return schema.Level.ToString();
        yield return schema.RequiresMaster.ToString();
        yield return modifiers?.AtkSpeedPct.ToString();
        yield return modifiers?.Ac.ToString();
        yield return modifiers?.MagicResistance.ToString();
        yield return modifiers?.Hit.ToString();
        yield return modifiers?.Dmg.ToString();
        yield return modifiers?.FlatSkillDamage.ToString();
        yield return modifiers?.FlatSpellDamage.ToString();
        yield return modifiers?.SkillDamagePct.ToString();
        yield return modifiers?.SpellDamagePct.ToString();
        yield return modifiers?.MaximumHp.ToString();
        yield return modifiers?.MaximumMp.ToString();
        yield return modifiers?.Str.ToString();
        yield return modifiers?.Int.ToString();
        yield return modifiers?.Wis.ToString();
        yield return modifiers?.Con.ToString();
        yield return modifiers?.Dex.ToString();
        yield return schema.CooldownMs?.ToString();
        yield return schema.EquipmentType?.ToString();
        yield return schema.Gender?.ToString();
        yield return schema.IsDyeable.ToString();
        yield return schema.IsModifiable.ToString();
        yield return schema.Category;
        yield return schema.Description;
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
                                           .Select(ConvertToRow)
                                           .SelectMany(
                                               (rowValue, rowIndex) =>
                                                   rowValue.Select((cellValue, columnIndex) => (cellValue, columnIndex, rowIndex)))
                                           .FirstOrDefault(set => set.cellValue?.ContainsI(text) == true);

                if (searchResult.cellValue is null)
                    return;

                var column = dataGrid.Columns.SingleOrDefault(col => col.DisplayIndex == searchResult.columnIndex);
                var columnIndex = dataGrid.Columns.IndexOf(column);

                SelectCellByIndex(
                    dataGrid,
                    searchResult.rowIndex,
                    columnIndex,
                    false);

                break;
            }

            default:
            {
                var searchResult = ItemTemplates
                                   .Select(listItem => ConvertToRow(listItem.Control.Item.Obj))
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
    }

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

        foreach (var property in PropertyOrder)
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

            foreach (var propertyName in ModifierProperties)
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
        } else if (!PropertyOrder.ContainsI(e.PropertyName))
            e.Cancel = true;
    }

    public static void SelectCellByIndex(
        DataGrid dataGrid,
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

        var item = dataGrid.Items[rowIndex];
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

    public static DataGridCell? GetCell(DataGrid dataGrid, DataGridRow? rowContainer, int column)
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
                var cell = presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;

                if (cell == null)
                {
                    /* bring the column into view
                     * in case it has been virtualized away */
                    dataGrid.ScrollIntoView(rowContainer, dataGrid.Columns[column]);
                    cell = presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;
                }

                return cell;
            }
        }

        return null;
    }

    public static T? FindVisualChild<T>(DependencyObject obj) where T: DependencyObject
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
    #endregion
}