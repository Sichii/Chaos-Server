using System.Windows;
using System.Windows.Controls;
using Chaos.Schemas.Content;
using Chaos.Wpf.Observables;
using ChaosTool.Definitions;
using ChaosTool.Extensions;
using ChaosTool.Utility;
using ChaosTool.ViewModel;

namespace ChaosTool.Controls.WorldMapControls;

public sealed partial class WorldMapPropertyEditor
{
    private WorldMapViewModel ViewModel
        => DataContext as WorldMapViewModel
           ?? throw new InvalidOperationException($"DataContext is not of type {nameof(WorldMapViewModel)}");

    public WorldMapPropertyEditor() => InitializeComponent();
    private void AddNodeKeyBtn_Click(object sender, RoutedEventArgs e) => ViewModel.NodeKeys.Add(new BindableString());

    private void DeleteNodeKeyBtn_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        if (button.DataContext is not BindableString scriptKey)
            return;

        ViewModel.NodeKeys.Remove(scriptKey);
    }

    #region Tbox Validation
    private void NodeKeyTbox_OnTextChanged(object sender, TextChangedEventArgs e)
        => Validators.TemplateKeyMatchesFileName(WorldMapKeyTbox, PathTbox);
    #endregion

    private void UserControl_Initialized(object sender, EventArgs e)
    {
        //tooltips
        WorldMapKeyLbl.ToolTip = Helpers.GetPropertyDocs<WorldMapSchema>(nameof(WorldMapSchema.WorldMapKey));
        FieldIndexLbl.ToolTip = Helpers.GetPropertyDocs<WorldMapSchema>(nameof(WorldMapSchema.FieldIndex));
        NodeKeysLbl.ToolTip = Helpers.GetPropertyDocs<WorldMapSchema>(nameof(WorldMapSchema.NodeKeys));
    }

    #region Buttons
    private void RevertBtn_Click(object sender, RoutedEventArgs e) => ViewModel.RejectChanges();

    private void SaveBtn_Click(object sender, RoutedEventArgs e) => ViewModel.AcceptChanges();

    private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
    {
        var parentList = this.FindVisualParent<WorldMapListView>();

        parentList?.Items.Remove(ViewModel);

        ViewModel.IsDeleted = true;
        ViewModel.AcceptChanges();
    }
    #endregion
}