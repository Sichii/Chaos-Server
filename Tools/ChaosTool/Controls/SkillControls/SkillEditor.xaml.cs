using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Chaos.Extensions.Common;
using Chaos.Schemas.Data;
using Chaos.Schemas.Templates;
using ChaosTool.Comparers;
using ChaosTool.Converters;
using ChaosTool.Extensions;
using ChaosTool.Model;
using MaterialDesignExtensions.Controls;
using TOOL_CONSTANTS = ChaosTool.Definitions.CONSTANTS;

namespace ChaosTool.Controls.SkillControls;

public sealed partial class SkillEditor
{
    public ObservableCollection<ListViewItem<SkillTemplateSchema, SkillPropertyEditor>> ListViewItems { get; }

    public SkillEditor()
    {
        ListViewItems = new ObservableCollection<ListViewItem<SkillTemplateSchema, SkillPropertyEditor>>();

        InitializeComponent();
    }

    private async void UserControl_Initialized(object sender, EventArgs e)
    {
        AddBtn.IsEnabled = false;
        GridViewBtn.IsEnabled = false;

        await WaitForJsonContextAsync(Snackbar);

        PopulateListView();
        AddFindBindings(FindTbox);

        AddBtn.IsEnabled = true;
        GridViewBtn.IsEnabled = true;
    }

    /// <inheritdoc />

    #region CTRL+F (Find)
    protected override UIElement? GetFocusElement()
    {
        if (TemplatesView.IsVisible)
            return TemplatesView;

        return PropertyEditor.Children
                             .OfType<UIElement>()
                             .FirstOrDefault();
    }

    private void FindTbox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        var tbox = (TextBox)sender;

        if (string.IsNullOrWhiteSpace(tbox.Text))
            return;

        var text = tbox.Text;

        var currentView = PropertyEditor.Children
                                        .OfType<Control>()
                                        .FirstOrDefault();

        switch (currentView)
        {
            case DataGrid dataGrid:
            {
                var searchResult = dataGrid.ItemsSource
                                           .OfType<SkillTemplateSchema>()
                                           .Select(SchemaExtensions.EnumerateProperties)
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
                TemplatesView.SelectedItem = selected;

                break;
            }
        }
    }
    #endregion

    #region ListView
    private void PopulateListView()
    {
        var objs = JsonContext.SkillTemplates.Objects.Select(
                                  wrapper => new ListViewItem<SkillTemplateSchema, SkillPropertyEditor>
                                  {
                                      Name = wrapper.Object.TemplateKey,
                                      Wrapper = wrapper
                                  })
                              .OrderBy(_ => _, ListViewItemComparer.Instance);

        ListViewItems.AddRange(objs);
    }

    private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selected = e.AddedItems.OfType<ListViewItem<SkillTemplateSchema, SkillPropertyEditor>>().FirstOrDefault();

        if (selected is null)
        {
            var listView = (ListView)sender;

            if (listView is { SelectedIndex: -1, Items.IsEmpty: false })
                listView.SelectedIndex = 0;

            return;
        }

        selected.Control ??= new SkillPropertyEditor(selected);
        selected.Control.DeleteBtn.Click += DeleteBtnOnClick;

        PropertyEditor.Children.Clear();
        PropertyEditor.Children.Add(selected.Control);
    }

    private void DeleteBtnOnClick(object sender, RoutedEventArgs e)
    {
        if (TemplatesView.SelectedItem is not ListViewItem<SkillTemplateSchema, SkillPropertyEditor> selected)
            return;

        ListViewItems.Remove(selected);
        JsonContext.SkillTemplates.Remove(selected.Name);
    }

    private async void AddButton_Click(object sender, RoutedEventArgs e)
    {
        var path = TOOL_CONSTANTS.TEMP_PATH;
        var baseDir = JsonContext.SkillTemplates.RootDirectory;
        var fullBaseDir = Path.GetFullPath(baseDir);

        var result = await OpenDirectoryDialog.ShowDialogAsync(
            DialogHost,
            new OpenDirectoryDialogArguments { CurrentDirectory = fullBaseDir, CreateNewDirectoryEnabled = true });

        if (result is null || result.Canceled)
            return;

        path = Path.Combine(result.Directory, path);

        var template = new SkillTemplateSchema { TemplateKey = Path.GetFileNameWithoutExtension(TOOL_CONSTANTS.TEMP_PATH) };
        var wrapper = new TraceWrapper<SkillTemplateSchema>(path, template);

        var listItem = new ListViewItem<SkillTemplateSchema, SkillPropertyEditor>
        {
            Name = TOOL_CONSTANTS.TEMP_PATH,
            Wrapper = wrapper
        };

        ListViewItems.Add(listItem);

        TemplatesView.SelectedItem = listItem;
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
            dataGrid.IsReadOnly = true;

            await JsonContext.LoadingTask;

            var lcv = new ListCollectionView(JsonContext.SkillTemplates.ToList());
            dataGrid.ItemsSource = lcv;

            PropertyEditor.Children.Clear();
            PropertyEditor.Children.Add(dataGrid);
            TemplatesView.Visibility = Visibility.Collapsed;
            button.Content = "List View";
        } else if (buttonStr.EqualsI("List View"))
        {
            PropertyEditor.Children.Clear();
            TemplatesView.Visibility = Visibility.Visible;
            var selected = TemplatesView.SelectedItem;
            TemplatesView.SelectedItem = null;
            TemplatesView.SelectedItem = selected;

            button.Content = "Grid View";
        }
    }

    private void DataGrid_AutoGeneratedColumns(object? sender, EventArgs e)
    {
        if (sender is not DataGrid dataGrid)
            return;

        var index = 0;

        foreach (var property in TOOL_CONSTANTS.SkillPropertyOrder)
        {
            var column = dataGrid.Columns.FirstOrDefault(c => c.Header.ToString()!.EqualsI(property));

            if (column is null)
                continue;

            column.DisplayIndex = index++;
        }
    }

    private void DataGrid_AutoGeneratingColumn(object? sender, DataGridAutoGeneratingColumnEventArgs e)
    {
        if (sender is not DataGrid dataGrid)
            return;

        if (e.PropertyName.EqualsI(nameof(SkillTemplateSchema.LearningRequirements)))
        {
            e.Cancel = true;

            foreach (var property in TOOL_CONSTANTS.LearningRequirementProperties)
                switch (property)
                {
                    case nameof(LearningRequirementsSchema.PrerequisiteSkillTemplateKeys):
                    case nameof(LearningRequirementsSchema.PrerequisiteSpellTemplateKeys):
                    {
                        var column = new DataGridTextColumn
                        {
                            Header = property,
                            Binding = new Binding($"{nameof(SkillTemplateSchema.LearningRequirements)}.{property}")
                            {
                                Converter = JoinStringCollectionConverter.Instance,
                                ValidatesOnDataErrors = true
                            }
                        };

                        dataGrid.Columns.Add(column);

                        break;
                    }
                    case nameof(LearningRequirementsSchema.RequiredStats):
                    {
                        foreach (var stat in TOOL_CONSTANTS.StatProperties)
                        {
                            var column = new DataGridTextColumn
                            {
                                Header = stat,
                                Binding = new Binding($"{nameof(SkillTemplateSchema.LearningRequirements)}.{property}.{stat}")
                                {
                                    ValidatesOnDataErrors = true
                                }
                            };

                            dataGrid.Columns.Add(column);
                        }

                        break;
                    }
                    case nameof(LearningRequirementsSchema.ItemRequirements):
                    {
                        var itemRequirementsColumn = new DataGridTextColumn
                        {
                            Header = nameof(LearningRequirementsSchema.ItemRequirements),
                            Binding = new Binding(
                                $"{nameof(SkillTemplateSchema.LearningRequirements)}.{nameof(LearningRequirementsSchema.ItemRequirements)}")
                            {
                                Converter = ItemRequirementCollectionConverter.Instance,
                                ValidatesOnDataErrors = true
                            }
                        };

                        dataGrid.Columns.Add(itemRequirementsColumn);

                        break;
                    }
                    default:
                    {
                        var column = new DataGridTextColumn
                        {
                            Header = property,
                            Binding = new Binding($"{nameof(SkillTemplateSchema.LearningRequirements)}.{property}")
                            {
                                ValidatesOnDataErrors = true
                            }
                        };

                        dataGrid.Columns.Add(column);

                        break;
                    }
                }
        } else if (e.PropertyName.EqualsI(nameof(SkillTemplateSchema.ScriptKeys)))
        {
            e.Cancel = true;

            var column = new DataGridTextColumn
            {
                Header = e.PropertyName,
                Binding = new Binding(e.PropertyName)
                {
                    Converter = JoinStringCollectionConverter.Instance,
                    ValidatesOnDataErrors = true
                }
            };

            dataGrid.Columns.Add(column);
        } else if (!TOOL_CONSTANTS.SkillPropertyOrder.ContainsI(e.PropertyName))
            e.Cancel = true;
    }
    #endregion
}