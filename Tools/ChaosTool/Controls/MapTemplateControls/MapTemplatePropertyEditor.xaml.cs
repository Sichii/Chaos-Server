using System.Windows;
using System.Windows.Controls;
using Chaos.Schemas.Templates;
using ChaosTool.Definitions;
using ChaosTool.Extensions;
using ChaosTool.Utility;
using ChaosTool.ViewModel;

namespace ChaosTool.Controls.MapTemplateControls;

public sealed partial class MapTemplatePropertyEditor
{
    private MapTemplateViewModel ViewModel
        => DataContext as MapTemplateViewModel
           ?? throw new InvalidOperationException($"DataContext is not of type {nameof(MapTemplateViewModel)}");

    public MapTemplatePropertyEditor() => InitializeComponent();

    #region Tbox Validation
    private void TemplateKeyTbox_OnTextChanged(object sender, TextChangedEventArgs e)
        => Validators.TemplateKeyMatchesFileName(TemplateKeyTbox, PathTbox);
    #endregion

    private void UserControl_Initialized(object sender, EventArgs e)
    {
        TemplateKeyLbl.ToolTip = Helpers.GetPropertyDocs<MapTemplateSchema>(nameof(MapTemplateSchema.TemplateKey));
        ScriptKeysLbl.ToolTip = Helpers.GetPropertyDocs<MapTemplateSchema>(nameof(MapTemplateSchema.ScriptKeys));
        HeightLbl.ToolTip = Helpers.GetPropertyDocs<MapTemplateSchema>(nameof(MapTemplateSchema.Height));
        WidthLbl.ToolTip = Helpers.GetPropertyDocs<MapTemplateSchema>(nameof(MapTemplateSchema.Width));
    }

    #region Buttons
    private void RevertBtn_Click(object sender, RoutedEventArgs e) => ViewModel.RejectChanges();

    private void SaveBtn_Click(object sender, RoutedEventArgs e) => ViewModel.AcceptChanges();

    private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
    {
        var parentList = this.FindVisualParent<MapTemplateListView>();

        parentList?.Items.Remove(ViewModel);

        ViewModel.IsDeleted = true;
        ViewModel.AcceptChanges();
    }
    #endregion

    #region ScriptKeys Controls
    private void AddScriptKeyBtn_Click(object sender, RoutedEventArgs e) => ViewModel.ScriptKeys.Add(new BindableString());

    private void DeleteScriptKeyBtn_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        if (button.DataContext is not BindableString scriptKey)
            return;

        ViewModel.ScriptKeys.Remove(scriptKey);
    }
    #endregion
}