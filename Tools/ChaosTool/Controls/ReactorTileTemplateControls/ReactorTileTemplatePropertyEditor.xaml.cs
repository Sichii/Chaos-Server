using System.Windows;
using System.Windows.Controls;
using Chaos.Schemas.Templates;
using Chaos.Wpf.Observables;
using ChaosTool.Definitions;
using ChaosTool.Extensions;
using ChaosTool.Utility;
using ChaosTool.ViewModel;

namespace ChaosTool.Controls.ReactorTileTemplateControls;

public sealed partial class ReactorTileTemplatePropertyEditor
{
    private ReactorTileTemplateViewModel ViewModel
        => DataContext as ReactorTileTemplateViewModel
           ?? throw new InvalidOperationException($"DataContext is not of type {nameof(ReactorTileTemplateViewModel)}");

    public ReactorTileTemplatePropertyEditor() => InitializeComponent();

    #region Tbox Validation
    private void TemplateKeyTbox_OnTextChanged(object sender, TextChangedEventArgs e)
        => Validators.TemplateKeyMatchesFileName(TemplateKeyTbox, PathTbox);
    #endregion

    private void UserControl_Initialized(object sender, EventArgs e)
    {
        TemplateKeyLbl.ToolTip = Helpers.GetPropertyDocs<ReactorTileTemplateSchema>(nameof(ReactorTileTemplateSchema.TemplateKey));

        ShouldBlockPathfindingLbl.ToolTip
            = Helpers.GetPropertyDocs<ReactorTileTemplateSchema>(nameof(ReactorTileTemplateSchema.ShouldBlockPathfinding));
        ScriptKeysLbl.ToolTip = Helpers.GetPropertyDocs<ReactorTileTemplateSchema>(nameof(ReactorTileTemplateSchema.ScriptKeys));
    }

    #region Buttons
    private void RevertBtn_Click(object sender, RoutedEventArgs e) => ViewModel.RejectChanges();

    private void SaveBtn_Click(object sender, RoutedEventArgs e) => ViewModel.AcceptChanges();

    private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
    {
        var parentList = this.FindVisualParent<ReactorTileTemplateListView>();

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