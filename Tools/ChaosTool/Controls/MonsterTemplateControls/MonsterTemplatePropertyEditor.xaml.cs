using System.Windows;
using System.Windows.Controls;
using Chaos.Common.Definitions;
using Chaos.Extensions.Common;
using Chaos.Schemas.Data;
using Chaos.Schemas.Templates;
using Chaos.Wpf.Observables;
using ChaosTool.Definitions;
using ChaosTool.Extensions;
using ChaosTool.Utility;
using ChaosTool.ViewModel;

namespace ChaosTool.Controls.MonsterTemplateControls;

public sealed partial class MonsterTemplatePropertyEditor
{
    private MonsterTemplateViewModel ViewModel
        => DataContext as MonsterTemplateViewModel
           ?? throw new InvalidOperationException($"DataContext is not of type {nameof(MonsterTemplateViewModel)}");

    public MonsterTemplatePropertyEditor() => InitializeComponent();

    #region Tbox Validation
    private void TemplateKeyTbox_OnTextChanged(object sender, TextChangedEventArgs e)
        => Validators.TemplateKeyMatchesFileName(TemplateKeyTbox, PathTbox);
    #endregion

    private void UserControl_Initialized(object sender, EventArgs e)
    {
        TypeCmbox.ItemsSource = EnumExtensions.GetEnumNames<CreatureType>();

        TemplateKeyLbl.ToolTip = Helpers.GetPropertyDocs<MonsterTemplateSchema>(nameof(MonsterTemplateSchema.TemplateKey));
        NameLbl.ToolTip = Helpers.GetPropertyDocs<MonsterTemplateSchema>(nameof(MonsterTemplateSchema.Name));
        TypeLbl.ToolTip = Helpers.GetPropertyDocs<MonsterTemplateSchema>(nameof(MonsterTemplateSchema.Type));
        AggroRangeLbl.ToolTip = Helpers.GetPropertyDocs<MonsterTemplateSchema>(nameof(MonsterTemplateSchema.AggroRange));
        ExpRewardLbl.ToolTip = Helpers.GetPropertyDocs<MonsterTemplateSchema>(nameof(MonsterTemplateSchema.ExpReward));
        MinGoldDropLbl.ToolTip = Helpers.GetPropertyDocs<MonsterTemplateSchema>(nameof(MonsterTemplateSchema.MinGoldDrop));
        MaxGoldDropLbl.ToolTip = Helpers.GetPropertyDocs<MonsterTemplateSchema>(nameof(MonsterTemplateSchema.MaxGoldDrop));
        AssailIntervalMsLbl.ToolTip = Helpers.GetPropertyDocs<MonsterTemplateSchema>(nameof(MonsterTemplateSchema.AssailIntervalMs));
        SkillIntervalMsLbl.ToolTip = Helpers.GetPropertyDocs<MonsterTemplateSchema>(nameof(MonsterTemplateSchema.SkillIntervalMs));
        SpellIntervalMsLbl.ToolTip = Helpers.GetPropertyDocs<MonsterTemplateSchema>(nameof(MonsterTemplateSchema.SpellIntervalMs));
        MoveIntervalMsLbl.ToolTip = Helpers.GetPropertyDocs<MonsterTemplateSchema>(nameof(MonsterTemplateSchema.MoveIntervalMs));
        WanderIntervalMsLbl.ToolTip = Helpers.GetPropertyDocs<MonsterTemplateSchema>(nameof(MonsterTemplateSchema.WanderIntervalMs));

        AbilityLbl.ToolTip = Helpers.GetPropertyDocs<StatSheetSchema>(nameof(MonsterTemplateSchema.StatSheet.AbilityLevel));
        LevelLbl.ToolTip = Helpers.GetPropertyDocs<StatSheetSchema>(nameof(MonsterTemplateSchema.StatSheet.Level));
        AtkSpeedPctLbl.ToolTip = Helpers.GetPropertyDocs<StatSheetSchema>(nameof(MonsterTemplateSchema.StatSheet.AtkSpeedPct));
        AcLbl.ToolTip = Helpers.GetPropertyDocs<StatSheetSchema>(nameof(MonsterTemplateSchema.StatSheet.Ac));

        MagicResistanceLbl.ToolTip = Helpers.GetPropertyDocs<StatSheetSchema>(nameof(MonsterTemplateSchema.StatSheet.MagicResistance));
        HitLbl.ToolTip = Helpers.GetPropertyDocs<StatSheetSchema>(nameof(MonsterTemplateSchema.StatSheet.Hit));
        DmgLbl.ToolTip = Helpers.GetPropertyDocs<StatSheetSchema>(nameof(MonsterTemplateSchema.StatSheet.Dmg));

        FlatSkillDamageLbl.ToolTip = Helpers.GetPropertyDocs<StatSheetSchema>(nameof(MonsterTemplateSchema.StatSheet.FlatSkillDamage));

        FlatSpellDamageLbl.ToolTip = Helpers.GetPropertyDocs<StatSheetSchema>(nameof(MonsterTemplateSchema.StatSheet.FlatSpellDamage));
        SkillDamagePctLbl.ToolTip = Helpers.GetPropertyDocs<StatSheetSchema>(nameof(MonsterTemplateSchema.StatSheet.SkillDamagePct));
        SpellDamagePctLbl.ToolTip = Helpers.GetPropertyDocs<StatSheetSchema>(nameof(MonsterTemplateSchema.StatSheet.SpellDamagePct));
        MaximumHpLbl.ToolTip = Helpers.GetPropertyDocs<StatSheetSchema>(nameof(MonsterTemplateSchema.StatSheet.MaximumHp));
        MaximumMpLbl.ToolTip = Helpers.GetPropertyDocs<StatSheetSchema>(nameof(MonsterTemplateSchema.StatSheet.MaximumMp));
        StrLbl.ToolTip = Helpers.GetPropertyDocs<StatSheetSchema>(nameof(MonsterTemplateSchema.StatSheet.Str));
        IntLbl.ToolTip = Helpers.GetPropertyDocs<StatSheetSchema>(nameof(MonsterTemplateSchema.StatSheet.Int));
        WisLbl.ToolTip = Helpers.GetPropertyDocs<StatSheetSchema>(nameof(MonsterTemplateSchema.StatSheet.Wis));
        ConLbl.ToolTip = Helpers.GetPropertyDocs<StatSheetSchema>(nameof(MonsterTemplateSchema.StatSheet.Con));
        DexLbl.ToolTip = Helpers.GetPropertyDocs<StatSheetSchema>(nameof(MonsterTemplateSchema.StatSheet.Dex));

        LootTableKeysLbl.ToolTip = Helpers.GetPropertyDocs<MonsterTemplateSchema>(nameof(MonsterTemplateSchema.LootTableKeys));
        ScriptKeysLbl.ToolTip = Helpers.GetPropertyDocs<MonsterTemplateSchema>(nameof(MonsterTemplateSchema.ScriptKeys));
        SkillTemplateKeysLbl.ToolTip = Helpers.GetPropertyDocs<MonsterTemplateSchema>(nameof(MonsterTemplateSchema.SkillTemplateKeys));
        SpellTemplateKeysLbl.ToolTip = Helpers.GetPropertyDocs<MonsterTemplateSchema>(nameof(MonsterTemplateSchema.SpellTemplateKeys));
    }

    #region Buttons
    private void RevertBtn_Click(object sender, RoutedEventArgs e) => ViewModel.RejectChanges();

    private void SaveBtn_Click(object sender, RoutedEventArgs e) => ViewModel.AcceptChanges();

    private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
    {
        var parentList = this.FindVisualParent<MonsterTemplateListView>();

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

    #region SpellTemplateKeys Controls
    private void AddSpellTemplateKeyBtn_OnClick(object sender, RoutedEventArgs e) => ViewModel.SpellTemplateKeys.Add(string.Empty);

    private void DeleteSpellTemplateKeyBtn_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        if (button.DataContext is not BindableString scriptKey)
            return;

        ViewModel.SpellTemplateKeys.Remove(scriptKey);
    }
    #endregion

    #region SkillTemplateKeys Controls
    private void DeleteSkillTemplateKeyBtn_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        if (button.DataContext is not BindableString scriptKey)
            return;

        ViewModel.SkillTemplateKeys.Remove(scriptKey);
    }

    private void AddSkillTemplateKeyBtn_OnClick(object sender, RoutedEventArgs e) => ViewModel.SkillTemplateKeys.Add(string.Empty);
    #endregion

    #region LootTableKeysControls
    private void DeleteLootTableKeyBtn_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        if (button.DataContext is not BindableString scriptKey)
            return;

        ViewModel.LootTableKeys.Remove(scriptKey);
    }

    private void AddLootTableKeyBtn_OnClick(object sender, RoutedEventArgs e) => ViewModel.LootTableKeys.Add(string.Empty);
    #endregion
}