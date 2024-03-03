using System.Windows;
using System.Windows.Controls;
using Chaos.Schemas.Content;
using ChaosTool.Definitions;
using ChaosTool.Extensions;
using ChaosTool.Utility;
using ChaosTool.ViewModel;

namespace ChaosTool.Controls.WorldMapNodeControls;

public partial class WorldMapNodePropertyEditor
{
    private WorldMapNodeViewModel ViewModel
        => DataContext as WorldMapNodeViewModel
           ?? throw new InvalidOperationException($"DataContext is not of type {nameof(WorldMapNodeViewModel)}");

    public WorldMapNodePropertyEditor() => InitializeComponent();

    #region Tbox Validation
    private void NodeKeyTbox_OnTextChanged(object sender, TextChangedEventArgs e)
        => Validators.TemplateKeyMatchesFileName(NodeKeyTbox, PathTbox);
    #endregion

    private void UserControl_Initialized(object sender, EventArgs e)
    {
        //tooltips
        NodeKeyLbl.ToolTip = Helpers.GetPropertyDocs<WorldMapNodeSchema>(nameof(WorldMapNodeSchema.NodeKey));
        DestMapInstanceKeyLbl.ToolTip = Helpers.GetPropertyDocs<WorldMapNodeSchema>(nameof(WorldMapNodeSchema.Destination));
        DestXLbl.ToolTip = Helpers.GetPropertyDocs<WorldMapNodeSchema>(nameof(WorldMapNodeSchema.Destination));
        DestYLbl.ToolTip = Helpers.GetPropertyDocs<WorldMapNodeSchema>(nameof(WorldMapNodeSchema.Destination));
        ScreenXLbl.ToolTip = Helpers.GetPropertyDocs<WorldMapNodeSchema>(nameof(WorldMapNodeSchema.ScreenPosition));
        ScreenYLbl.ToolTip = Helpers.GetPropertyDocs<WorldMapNodeSchema>(nameof(WorldMapNodeSchema.ScreenPosition));
        TextLbl.ToolTip = Helpers.GetPropertyDocs<WorldMapNodeSchema>(nameof(WorldMapNodeSchema.Text));
    }

    #region Buttons
    private void RevertBtn_Click(object sender, RoutedEventArgs e) => ViewModel.RejectChanges();

    private void SaveBtn_Click(object sender, RoutedEventArgs e) => ViewModel.AcceptChanges();

    private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
    {
        var parentList = this.FindVisualParent<WorldMapNodeListView>();

        parentList?.Items.Remove(ViewModel);

        ViewModel.IsDeleted = true;
        ViewModel.AcceptChanges();
    }
    #endregion
}