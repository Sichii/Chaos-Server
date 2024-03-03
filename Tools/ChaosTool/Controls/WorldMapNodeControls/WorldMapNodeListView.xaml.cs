using System.IO;
using System.Windows;
using System.Windows.Controls;
using Chaos.Extensions.Common;
using ChaosTool.Controls.Abstractions;
using ChaosTool.ViewModel;
using MaterialDesignExtensions.Controls;
using TOOL_CONSTANTS = ChaosTool.Definitions.CONSTANTS;

namespace ChaosTool.Controls.WorldMapNodeControls;

public sealed class WorldMapNodeListView : ViewModelListView
{
    public WorldMapNodeListView() => InitializeComponent();

    /// <inheritdoc />

    #region CTRL+F (Find)
    protected override void FindTbox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        var tbox = (TextBox)sender;

        if (string.IsNullOrWhiteSpace(tbox.Text))
            return;

        var text = tbox.Text;

        /* The idea is to search all properties of each item in TemplatesView
        var searchResult = ListViewItems.Select(listItem => listItem.Object.EnumerateProperties())
                                        .SelectMany(
                                            (rowValue, rowIndex) => rowValue.Select(
                                                (cellValue, columnIndex) => (cellValue, columnIndex, rowIndex)))
                                        .FirstOrDefault(set => set.cellValue?.ContainsI(text) == true);

        if (searchResult.cellValue is null)
            return;

        var selected = ListViewItems[searchResult.rowIndex];
        TemplatesView.SelectedItem = selected;
        */
    }
    #endregion

    #region ListView
    protected override void PopulateListView()
    {
        var mapped = JsonContext.WorldMapNodes
                                .Objects
                                .Select(obj => obj.MapTo<WorldMapNodeViewModel>())
                                .OrderBy(obj => obj.ViewModelIdentifier);

        Items.AddRange(mapped);
    }

    protected override async void AddButton_Click(object sender, RoutedEventArgs e)
    {
        var baseDir = JsonContext.WorldMapNodes.RootDirectory;
        var fullBaseDir = Path.GetFullPath(baseDir);

        var result = await OpenDirectoryDialog.ShowDialogAsync(
            DialogHost,
            new OpenDirectoryDialogArguments
            {
                CurrentDirectory = fullBaseDir,
                CreateNewDirectoryEnabled = true
            });

        if (result is null || result.Canceled)
            return;

        var path = Path.Combine(result.Directory, TOOL_CONSTANTS.TEMP_FILE_NAME);

        var viewModel = new WorldMapNodeViewModel
        {
            OriginalPath = path,
            Path = path
        };

        Items.Add(viewModel);
        ItemsView.SelectedItem = viewModel;
    }
    #endregion
}