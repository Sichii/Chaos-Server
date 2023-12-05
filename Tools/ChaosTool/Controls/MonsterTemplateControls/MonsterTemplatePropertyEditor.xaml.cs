using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Chaos.Common.Definitions;
using Chaos.Extensions.Common;
using Chaos.Schemas.Templates;
using ChaosTool.Definitions;
using ChaosTool.Extensions;
using ChaosTool.Model;

namespace ChaosTool.Controls.MonsterTemplateControls;

public sealed partial class MonsterTemplatePropertyEditor
{
    public ListViewItem<MonsterTemplateSchema, MonsterTemplatePropertyEditor> ListItem { get; }
    public ObservableCollection<BindableString> LootTableKeysViewItems { get; }
    public ObservableCollection<BindableString> ScriptKeysViewItems { get; }
    public ObservableCollection<BindableString> SkillTemplateKeysViewItems { get; }
    public ObservableCollection<BindableString> SpellTemplateKeysViewItems { get; }
    public TraceWrapper<MonsterTemplateSchema> Wrapper => ListItem.Wrapper;

    public MonsterTemplatePropertyEditor(ListViewItem<MonsterTemplateSchema, MonsterTemplatePropertyEditor> listItem)
    {
        ListItem = listItem;
        ScriptKeysViewItems = new ObservableCollection<BindableString>();
        SpellTemplateKeysViewItems = new ObservableCollection<BindableString>();
        SkillTemplateKeysViewItems = new ObservableCollection<BindableString>();
        LootTableKeysViewItems = new ObservableCollection<BindableString>();

        InitializeComponent();
    }

    private void UserControl_Initialized(object sender, EventArgs e)
    {
        TypeCmbox.ItemsSource = GetEnumNames<CreatureType>();
        ScriptKeysView.ItemsSource = ScriptKeysViewItems;
        SpellTemplateKeysView.ItemsSource = SpellTemplateKeysViewItems;
        SkillTemplateKeysView.ItemsSource = SkillTemplateKeysViewItems;
        LootTableKeysView.ItemsSource = LootTableKeysViewItems;

        PopulateControlsFromItem();
    }

    #region Controls > Template > Controls
    public void CopySelectionsToItem()
    {
        var template = Wrapper.Object;
        var statSheet = template.StatSheet;

        Wrapper.Path = PathTbox.Text;
        template.TemplateKey = TemplateKeyTbox.Text;
        template.Name = NameTbox.Text;
        template.Type = ParsePrimitive<CreatureType>(TypeCmbox.Text);
        template.AggroRange = ParsePrimitive<int>(AggroRangeTbox.Text);
        template.ExpReward = ParsePrimitive<int>(ExpRewardTbox.Text);
        template.MinGoldDrop = ParsePrimitive<int>(MinGoldDropTbox.Text);
        template.MaxGoldDrop = ParsePrimitive<int>(MaxGoldDropTbox.Text);
        template.AssailIntervalMs = ParsePrimitive<int>(AssailIntervalMsTbox.Text);
        template.SkillIntervalMs = ParsePrimitive<int>(SkillIntervalMsTbox.Text);
        template.SpellIntervalMs = ParsePrimitive<int>(SpellIntervalMsTbox.Text);
        template.MoveIntervalMs = ParsePrimitive<int>(MoveIntervalMsTbox.Text);
        template.WanderIntervalMs = ParsePrimitive<int>(WanderIntervalMsTbox.Text);

        statSheet.Ability = ParsePrimitive<int>(AbilityTbox.Text);
        statSheet.Level = ParsePrimitive<int>(LevelTbox.Text);
        statSheet.AtkSpeedPct = ParsePrimitive<int>(AtkSpeedPctTbox.Text);
        statSheet.Ac = ParsePrimitive<int>(AcTbox.Text);
        statSheet.MagicResistance = ParsePrimitive<int>(MagicResistanceTbox.Text);
        statSheet.Hit = ParsePrimitive<int>(HitTbox.Text);
        statSheet.Dmg = ParsePrimitive<int>(DmgTbox.Text);
        statSheet.FlatSkillDamage = ParsePrimitive<int>(FlatSkillDamageTbox.Text);
        statSheet.FlatSpellDamage = ParsePrimitive<int>(FlatSpellDamageTbox.Text);
        statSheet.SkillDamagePct = ParsePrimitive<int>(SkillDamagePctTbox.Text);
        statSheet.SpellDamagePct = ParsePrimitive<int>(SpellDamagePctTbox.Text);
        statSheet.MaximumHp = ParsePrimitive<int>(MaximumHpTbox.Text);
        statSheet.MaximumMp = ParsePrimitive<int>(MaximumMpTbox.Text);
        statSheet.Str = ParsePrimitive<int>(StrTbox.Text);
        statSheet.Int = ParsePrimitive<int>(IntTbox.Text);
        statSheet.Wis = ParsePrimitive<int>(WisTbox.Text);
        statSheet.Con = ParsePrimitive<int>(ConTbox.Text);
        statSheet.Dex = ParsePrimitive<int>(DexTbox.Text);

        template.ScriptKeys = ScriptKeysViewItems.ToStrings()
                                                 .ToList();

        template.SpellTemplateKeys = SpellTemplateKeysViewItems.ToStrings()
                                                               .ToList();

        template.SkillTemplateKeys = SkillTemplateKeysViewItems.ToStrings()
                                                               .ToList();

        template.LootTableKeys = LootTableKeysViewItems.ToStrings()
                                                       .ToList();

        ListItem.Name = template.TemplateKey;
    }

    public void PopulateControlsFromItem()
    {
        var template = Wrapper.Object;

        PathTbox.Text = Wrapper.Path;

        TemplateKeyTbox.IsEnabled = false;
        TemplateKeyTbox.Text = template.TemplateKey;
        TemplateKeyTbox.IsEnabled = true;

        NameTbox.Text = template.Name;
        TypeCmbox.SelectedItem = SelectPrimitive(template.Type, TypeCmbox.Items);
        AggroRangeTbox.Text = template.AggroRange.ToString();
        ExpRewardTbox.Text = template.ExpReward.ToString();
        MinGoldDropTbox.Text = template.MinGoldDrop.ToString();
        MaxGoldDropTbox.Text = template.MaxGoldDrop.ToString();
        AssailIntervalMsTbox.Text = template.AssailIntervalMs.ToString();
        SkillIntervalMsTbox.Text = template.SkillIntervalMs.ToString();
        SpellIntervalMsTbox.Text = template.SpellIntervalMs.ToString();
        MoveIntervalMsTbox.Text = template.MoveIntervalMs.ToString();
        WanderIntervalMsTbox.Text = template.WanderIntervalMs.ToString();
        AbilityTbox.Text = template.StatSheet.Ability.ToString();
        LevelTbox.Text = template.StatSheet.Level.ToString();
        AtkSpeedPctTbox.Text = template.StatSheet.AtkSpeedPct.ToString();
        AcTbox.Text = template.StatSheet.Ac.ToString();
        MagicResistanceTbox.Text = template.StatSheet.MagicResistance.ToString();
        HitTbox.Text = template.StatSheet.Hit.ToString();
        DmgTbox.Text = template.StatSheet.Dmg.ToString();
        FlatSkillDamageTbox.Text = template.StatSheet.FlatSkillDamage.ToString();
        FlatSpellDamageTbox.Text = template.StatSheet.FlatSpellDamage.ToString();
        SkillDamagePctTbox.Text = template.StatSheet.SkillDamagePct.ToString();
        SpellDamagePctTbox.Text = template.StatSheet.SpellDamagePct.ToString();
        MaximumHpTbox.Text = template.StatSheet.MaximumHp.ToString();
        MaximumMpTbox.Text = template.StatSheet.MaximumMp.ToString();
        StrTbox.Text = template.StatSheet.Str.ToString();
        IntTbox.Text = template.StatSheet.Int.ToString();
        WisTbox.Text = template.StatSheet.Wis.ToString();
        ConTbox.Text = template.StatSheet.Con.ToString();
        DexTbox.Text = template.StatSheet.Dex.ToString();

        ScriptKeysViewItems.Clear();
        ScriptKeysViewItems.AddRange(template.ScriptKeys.ToBindableStrings());

        SpellTemplateKeysViewItems.Clear();
        SpellTemplateKeysViewItems.AddRange(template.SpellTemplateKeys.ToBindableStrings());

        SkillTemplateKeysViewItems.Clear();
        SkillTemplateKeysViewItems.AddRange(template.SkillTemplateKeys.ToBindableStrings());

        LootTableKeysViewItems.Clear();
        LootTableKeysViewItems.AddRange(template.LootTableKeys.ToBindableStrings());
    }
    #endregion

    #region Buttons
    private void RevertBtn_Click(object sender, RoutedEventArgs e) => PopulateControlsFromItem();

    private async void SaveBtn_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var existing = JsonContext.MonsterTemplates
                                      .Objects
                                      .Where(wrapper => wrapper != Wrapper)
                                      .FirstOrDefault(wrapper => wrapper.Path.EqualsI(PathTbox.Text));

            if (existing is not null)
            {
                Snackbar.MessageQueue?.Enqueue($"Save failed. An item already exists at path \"{existing.Path}\"");

                return;
            }

            existing = JsonContext.MonsterTemplates
                                  .Objects
                                  .Where(wrapper => wrapper != Wrapper)
                                  .FirstOrDefault(wrapper => wrapper.Object.TemplateKey.EqualsI(TemplateKeyTbox.Text));

            if (existing is not null)
            {
                Snackbar.MessageQueue?.Enqueue(
                    $"Save failed. An item already exists with template key \"{existing.Object.TemplateKey}\" at path \"{existing.Path}\"");

                return;
            }

            existing = JsonContext.MonsterTemplates.Objects.FirstOrDefault(obj => ReferenceEquals(obj, Wrapper));

            if (existing is null)
                JsonContext.MonsterTemplates.Objects.Add(Wrapper);

            if (!ValidatePreSave(Wrapper, PathTbox, TemplateKeyTbox))
            {
                Snackbar.MessageQueue?.Enqueue("Filename does not match the template key");

                return;
            }

            CopySelectionsToItem();
            PopulateControlsFromItem();
        } catch (Exception ex)
        {
            Snackbar.MessageQueue?.Enqueue(ex.ToString());
        }

        await JsonContext.MonsterTemplates.SaveItemAsync(Wrapper);
    }
    #endregion

    #region Tbox Validation
    private void TboxNumberValidator(object sender, TextCompositionEventArgs e) => Validators.NumberValidationTextBox(sender, e);

    private void TemplateKeyTbox_OnTextChanged(object sender, TextChangedEventArgs e)
        => Validators.TemplateKeyMatchesFileName(TemplateKeyTbox, PathTbox);
    #endregion

    #region ScriptKeys Controls
    private void AddScriptKeyBtn_Click(object sender, RoutedEventArgs e) => ScriptKeysViewItems.Add(string.Empty);

    private void DeleteScriptKeyBtn_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        if (button.DataContext is not BindableString scriptKey)
            return;

        ScriptKeysViewItems.Remove(scriptKey);
    }
    #endregion

    #region SpellTemplateKeys Controls
    private void AddSpellTemplateKeyBtn_OnClick(object sender, RoutedEventArgs e) => SpellTemplateKeysViewItems.Add(string.Empty);

    private void DeleteSpellTemplateKeyBtn_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        if (button.DataContext is not BindableString scriptKey)
            return;

        SpellTemplateKeysViewItems.Remove(scriptKey);
    }
    #endregion

    #region SkillTemplateKeys Controls
    private void DeleteSkillTemplateKeyBtn_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        if (button.DataContext is not BindableString scriptKey)
            return;

        SkillTemplateKeysViewItems.Remove(scriptKey);
    }

    private void AddSkillTemplateKeyBtn_OnClick(object sender, RoutedEventArgs e) => SkillTemplateKeysViewItems.Add(string.Empty);
    #endregion

    #region LootTableKeysControls
    private void DeleteLootTableKeyBtn_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        if (button.DataContext is not BindableString scriptKey)
            return;

        LootTableKeysViewItems.Remove(scriptKey);
    }

    private void AddLootTableKeyBtn_OnClick(object sender, RoutedEventArgs e) => LootTableKeysViewItems.Add(string.Empty);
    #endregion
}