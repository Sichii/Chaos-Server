using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using Chaos.Extensions.Common;
using ChaosTool.Controls.Abstractions;
using ChaosTool.ViewModel;
using TextBox = System.Windows.Controls.TextBox;
using TOOL_CONSTANTS = ChaosTool.Definitions.CONSTANTS;

namespace ChaosTool.Controls.MerchantTemplateControls;

public sealed class MerchantTemplateListView : ViewModelListView
{
    public MerchantTemplateListView() => InitializeComponent();

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
        var mapped = JsonContext.MerchantTemplates
                                .Objects
                                .Select(obj => obj.MapTo<MerchantTemplateViewModel>())
                                .OrderBy(obj => obj.ViewModelIdentifier);

        Items.AddRange(mapped);
    }

    protected override void AddButton_Click(object sender, RoutedEventArgs e)
    {
        var baseDir = JsonContext.MerchantTemplates.RootDirectory;
        var fullBaseDir = Path.GetFullPath(baseDir);

        using var folderBrowserDialog = new FolderBrowserDialog();
        folderBrowserDialog.InitialDirectory = fullBaseDir;
        folderBrowserDialog.ShowNewFolderButton = true;

        if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
        {
            var path = Path.Combine(folderBrowserDialog.SelectedPath, TOOL_CONSTANTS.TEMP_FILE_NAME);

            var viewModel = new MerchantTemplateViewModel
            {
                OriginalPath = path,
                Path = path
            };

            Items.Add(viewModel);
            ItemsView.SelectedItem = viewModel;
        }
    }
    #endregion
}