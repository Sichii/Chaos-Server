using System.Windows;
using System.Windows.Controls;
using Chaos.Common.Definitions;
using Chaos.Schemas.Content;
using ChaosTool.Definitions;
using ChaosTool.Extensions;
using ChaosTool.Utility;
using ChaosTool.ViewModel;
using ChaosTool.ViewModel.Observables;

namespace ChaosTool.Controls.LootTableControls;

public sealed partial class LootTablePropertyEditor
{
    private LootTableViewModel ViewModel
        => DataContext as LootTableViewModel
           ?? throw new InvalidOperationException($"DataContext is not of type {nameof(LootTableViewModel)}");

    public LootTablePropertyEditor() => InitializeComponent();

    #region Tbox Validation
    private void TemplateKeyTbox_OnTextChanged(object sender, TextChangedEventArgs e)
        => Validators.TemplateKeyMatchesFileName(KeyTbox, PathTbox);
    #endregion

    private void UserControl_Initialized(object sender, EventArgs e)
    {
        ModeCmbox.ItemsSource = Helpers.GetEnumNames<LootTableMode>();

        KeyLbl.ToolTip = Helpers.GetPropertyDocs<LootTableSchema>(nameof(LootTableSchema.Key));
        LootDropsLbl.ToolTip = Helpers.GetPropertyDocs<LootTableSchema>(nameof(LootTableSchema.LootDrops));
        ModeLbl.ToolTip = Helpers.GetPropertyDocs<LootTableSchema>(nameof(LootTableSchema.Mode));
    }

    #region Buttons
    private void RevertBtn_Click(object sender, RoutedEventArgs e) => ViewModel.RejectChanges();

    private void SaveBtn_Click(object sender, RoutedEventArgs e) => ViewModel.AcceptChanges();

    private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
    {
        var parentList = this.FindVisualParent<LootTableListView>();

        parentList?.Items.Remove(ViewModel);

        ViewModel.IsDeleted = true;
        ViewModel.AcceptChanges();
    }
    #endregion

    #region LootDrops Controls
    private void DeleteLootDropBtn_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        if (button.DataContext is not ObservableLootDrop lootDrop)
            return;

        ViewModel.LootDrops.Remove(lootDrop);
    }

    private void AddLootDropBtn_Click(object sender, RoutedEventArgs e) => ViewModel.LootDrops.Add(new ObservableLootDrop());
    #endregion
}