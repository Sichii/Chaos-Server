using System.Windows;
using System.Windows.Controls;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions.Common;
using Chaos.Schemas.Data;
using Chaos.Schemas.Templates;
using Chaos.Wpf.Observables;
using ChaosTool.Definitions;
using ChaosTool.Extensions;
using ChaosTool.Utility;
using ChaosTool.ViewModel;

namespace ChaosTool.Controls.ItemTemplateControls;

/// <summary>
///     Interaction logic for ItemTemplatePropertyEditor.xaml
/// </summary>
public sealed partial class ItemTemplatePropertyEditor
{
    private ItemTemplateViewModel ViewModel
        => DataContext as ItemTemplateViewModel
           ?? throw new InvalidOperationException($"DataContext is not of type {nameof(ItemTemplateViewModel)}");

    public ItemTemplatePropertyEditor() => InitializeComponent();

    #region Tbox Validation
    private void TemplateKeyTbox_OnTextChanged(object sender, TextChangedEventArgs e)
        => Validators.TemplateKeyMatchesFileName(TemplateKeyTbox, PathTbox);
    #endregion

    private void UserControl_Initialized(object sender, EventArgs e)
    {
        ColorCmbox.ItemsSource = EnumExtensions.GetEnumNames<DisplayColor>();
        EquipmentTypeCmbox.ItemsSource = EnumExtensions.GetEnumNames<EquipmentType?>();
        GenderCmbox.ItemsSource = EnumExtensions.GetEnumNames<Gender?>();

        PantsColorCmbox.ItemsSource = EnumExtensions.GetEnumNames<DisplayColor?>()
                                                    .Take(17); //0-15 + null
        ClassCmbox.ItemsSource = EnumExtensions.GetEnumNames<BaseClass?>();
        AdvClassCmbox.ItemsSource = EnumExtensions.GetEnumNames<AdvClass?>();

        TemplateKeyLbl.ToolTip = Helpers.GetPropertyDocs<ItemTemplateSchema>(nameof(ItemTemplateSchema.TemplateKey));
        NameLbl.ToolTip = Helpers.GetPropertyDocs<ItemTemplateSchema>(nameof(ItemTemplateSchema.Name));
        PanelSpriteLbl.ToolTip = Helpers.GetPropertyDocs<ItemTemplateSchema>(nameof(ItemTemplateSchema.PanelSprite));
        DisplaySpriteLbl.ToolTip = Helpers.GetPropertyDocs<ItemTemplateSchema>(nameof(ItemTemplateSchema.DisplaySprite));
        ColorLbl.ToolTip = Helpers.GetPropertyDocs<ItemTemplateSchema>(nameof(ItemTemplateSchema.Color));
        PantsColorLbl.ToolTip = Helpers.GetPropertyDocs<ItemTemplateSchema>(nameof(ItemTemplateSchema.PantsColor));
        MaxStacksLbl.ToolTip = Helpers.GetPropertyDocs<ItemTemplateSchema>(nameof(ItemTemplateSchema.MaxStacks));
        AccountBoundLbl.ToolTip = Helpers.GetPropertyDocs<ItemTemplateSchema>(nameof(ItemTemplateSchema.AccountBound));
        BuyCostLbl.ToolTip = Helpers.GetPropertyDocs<ItemTemplateSchema>(nameof(ItemTemplateSchema.BuyCost));
        SellValueLbl.ToolTip = Helpers.GetPropertyDocs<ItemTemplateSchema>(nameof(ItemTemplateSchema.SellValue));
        WeightLbl.ToolTip = Helpers.GetPropertyDocs<ItemTemplateSchema>(nameof(ItemTemplateSchema.Weight));
        MaxDurabilityLbl.ToolTip = Helpers.GetPropertyDocs<ItemTemplateSchema>(nameof(ItemTemplateSchema.MaxDurability));
        ClassLbl.ToolTip = Helpers.GetPropertyDocs<ItemTemplateSchema>(nameof(ItemTemplateSchema.Class));
        AdvClassLbl.ToolTip = Helpers.GetPropertyDocs<ItemTemplateSchema>(nameof(ItemTemplateSchema.AdvClass));
        LevelLbl.ToolTip = Helpers.GetPropertyDocs<ItemTemplateSchema>(nameof(ItemTemplateSchema.Level));
        RequiresMasterLbl.ToolTip = Helpers.GetPropertyDocs<ItemTemplateSchema>(nameof(ItemTemplateSchema.RequiresMaster));

        AtkSpeedPctLbl.ToolTip = Helpers.GetPropertyDocs<AttributesSchema>(nameof(AttributesSchema.AtkSpeedPct));
        AcLbl.ToolTip = Helpers.GetPropertyDocs<AttributesSchema>(nameof(AttributesSchema.Ac));
        MagicResistanceLbl.ToolTip = Helpers.GetPropertyDocs<AttributesSchema>(nameof(AttributesSchema.MagicResistance));
        HitLbl.ToolTip = Helpers.GetPropertyDocs<AttributesSchema>(nameof(AttributesSchema.Hit));
        DmgLbl.ToolTip = Helpers.GetPropertyDocs<AttributesSchema>(nameof(AttributesSchema.Dmg));
        FlatSkillDamageLbl.ToolTip = Helpers.GetPropertyDocs<AttributesSchema>(nameof(AttributesSchema.FlatSkillDamage));
        FlatSpellDamageLbl.ToolTip = Helpers.GetPropertyDocs<AttributesSchema>(nameof(AttributesSchema.FlatSpellDamage));
        SkillDamagePctLbl.ToolTip = Helpers.GetPropertyDocs<AttributesSchema>(nameof(AttributesSchema.SkillDamagePct));
        SpellDamagePctLbl.ToolTip = Helpers.GetPropertyDocs<AttributesSchema>(nameof(AttributesSchema.SpellDamagePct));
        StrLbl.ToolTip = Helpers.GetPropertyDocs<AttributesSchema>(nameof(AttributesSchema.Str));
        IntLbl.ToolTip = Helpers.GetPropertyDocs<AttributesSchema>(nameof(AttributesSchema.Int));
        WisLbl.ToolTip = Helpers.GetPropertyDocs<AttributesSchema>(nameof(AttributesSchema.Wis));
        ConLbl.ToolTip = Helpers.GetPropertyDocs<AttributesSchema>(nameof(AttributesSchema.Con));
        DexLbl.ToolTip = Helpers.GetPropertyDocs<AttributesSchema>(nameof(AttributesSchema.Dex));

        CooldownMsLbl.ToolTip = Helpers.GetPropertyDocs<ItemTemplateSchema>(nameof(ItemTemplateSchema.CooldownMs));
        EquipmentTypeLbl.ToolTip = Helpers.GetPropertyDocs<ItemTemplateSchema>(nameof(ItemTemplateSchema.EquipmentType));
        GenderLbl.ToolTip = Helpers.GetPropertyDocs<ItemTemplateSchema>(nameof(ItemTemplateSchema.Gender));
        IsDyeableLbl.ToolTip = Helpers.GetPropertyDocs<ItemTemplateSchema>(nameof(ItemTemplateSchema.IsDyeable));
        IsModifiableLbl.ToolTip = Helpers.GetPropertyDocs<ItemTemplateSchema>(nameof(ItemTemplateSchema.IsModifiable));
        CategoryLbl.ToolTip = Helpers.GetPropertyDocs<ItemTemplateSchema>(nameof(ItemTemplateSchema.Category));
        DescriptionLbl.ToolTip = Helpers.GetPropertyDocs<ItemTemplateSchema>(nameof(ItemTemplateSchema.Description));
        ScriptKeysLbl.ToolTip = Helpers.GetPropertyDocs<ItemTemplateSchema>(nameof(ItemTemplateSchema.ScriptKeys));
    }

    #region Buttons
    private void RevertBtn_Click(object sender, RoutedEventArgs e) => ViewModel.RejectChanges();

    private void SaveBtn_Click(object sender, RoutedEventArgs e) => ViewModel.AcceptChanges();

    private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
    {
        var parentList = this.FindVisualParent<ItemTemplateListView>();

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