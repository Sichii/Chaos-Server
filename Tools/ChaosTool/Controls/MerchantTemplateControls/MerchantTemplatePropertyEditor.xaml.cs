using System.Windows;
using System.Windows.Controls;
using Chaos.Schemas.Templates;
using Chaos.Wpf.Observables;
using ChaosTool.Definitions;
using ChaosTool.Extensions;
using ChaosTool.Utility;
using ChaosTool.ViewModel;
using ChaosTool.ViewModel.Observables;

namespace ChaosTool.Controls.MerchantTemplateControls;

public sealed partial class MerchantTemplatePropertyEditor
{
    private MerchantTemplateViewModel ViewModel
        => DataContext as MerchantTemplateViewModel
           ?? throw new InvalidOperationException($"DataContext is not of type {nameof(MerchantTemplateViewModel)}");

    public MerchantTemplatePropertyEditor() => InitializeComponent();

    #region Tbox Validation
    private void TemplateKeyTbox_OnTextChanged(object sender, TextChangedEventArgs e)
        => Validators.TemplateKeyMatchesFileName(TemplateKeyTbox, PathTbox);
    #endregion

    private void UserControl_Initialized(object sender, EventArgs e)
    {
        TemplateKeyLbl.ToolTip = Helpers.GetPropertyDocs<MerchantTemplateSchema>(nameof(MerchantTemplateSchema.TemplateKey));
        NameLbl.ToolTip = Helpers.GetPropertyDocs<MerchantTemplateSchema>(nameof(MerchantTemplateSchema.Name));
        SpriteLbl.ToolTip = Helpers.GetPropertyDocs<MerchantTemplateSchema>(nameof(MerchantTemplateSchema.Sprite));
        RestockPctLbl.ToolTip = Helpers.GetPropertyDocs<MerchantTemplateSchema>(nameof(MerchantTemplateSchema.RestockPct));
        RestockIntervalHrsLbl.ToolTip = Helpers.GetPropertyDocs<MerchantTemplateSchema>(nameof(MerchantTemplateSchema.RestockIntervalHrs));
        WanderIntervalMsLbl.ToolTip = Helpers.GetPropertyDocs<MerchantTemplateSchema>(nameof(MerchantTemplateSchema.WanderIntervalMs));
        ScriptKeysLbl.ToolTip = Helpers.GetPropertyDocs<MerchantTemplateSchema>(nameof(MerchantTemplateSchema.ScriptKeys));
        ItemsForSaleLbl.ToolTip = Helpers.GetPropertyDocs<MerchantTemplateSchema>(nameof(MerchantTemplateSchema.ItemsForSale));
        ItemsToBuyLbl.ToolTip = Helpers.GetPropertyDocs<MerchantTemplateSchema>(nameof(MerchantTemplateSchema.ItemsToBuy));
        SkillsToTeachLbl.ToolTip = Helpers.GetPropertyDocs<MerchantTemplateSchema>(nameof(MerchantTemplateSchema.SkillsToTeach));
        SpellsToTeachLbl.ToolTip = Helpers.GetPropertyDocs<MerchantTemplateSchema>(nameof(MerchantTemplateSchema.SpellsToTeach));
    }

    #region Buttons
    private void RevertBtn_Click(object sender, RoutedEventArgs e) => ViewModel.RejectChanges();

    private void SaveBtn_Click(object sender, RoutedEventArgs e) => ViewModel.AcceptChanges();

    private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
    {
        var parentList = this.FindVisualParent<MerchantTemplateListView>();

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

    #region ItemsForSale Controls
    private void DeleteItemForSaleBtn_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        if (button.DataContext is not ObservableItemDetails itemDetails)
            return;

        ViewModel.ItemsForSale.Remove(itemDetails);
    }

    private void AddItemForSaleBtn_Click(object sender, RoutedEventArgs e) => ViewModel.ItemsForSale.Add(new ObservableItemDetails());
    #endregion

    #region ItemsToBuy Controls
    private void DeleteItemToBuyBtn_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        if (button.DataContext is not BindableString scriptKey)
            return;

        ViewModel.ItemsToBuy.Remove(scriptKey);
    }

    private void AddItemToBuyBtn_Click(object sender, RoutedEventArgs e) => ViewModel.ItemsToBuy.Add(string.Empty);
    #endregion

    #region SkillsToTeach Controls
    private void DeleteSkillToTeachBtn_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        if (button.DataContext is not BindableString scriptKey)
            return;

        ViewModel.SkillsToTeach.Remove(scriptKey);
    }

    private void AddSkillToTeachBtn_Click(object sender, RoutedEventArgs e) => ViewModel.SkillsToTeach.Add(string.Empty);
    #endregion

    #region SpellsToTeach Controls
    private void DeleteSpellToTeachBtn_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        if (button.DataContext is not BindableString scriptKey)
            return;

        ViewModel.SpellsToTeach.Remove(scriptKey);
    }

    private void AddSpellToTeachBtn_Click(object sender, RoutedEventArgs e) => ViewModel.SpellsToTeach.Add(string.Empty);
    #endregion
}