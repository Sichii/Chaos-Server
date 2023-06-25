using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Chaos.Common.Utilities;
using Chaos.Extensions.Common;
using Chaos.Schemas.Data;
using Chaos.Schemas.Templates;
using ChaosTool.Definitions;
using ChaosTool.Extensions;
using ChaosTool.Model;

namespace ChaosTool.Controls.MerchantTemplateControls;

public sealed partial class MerchantTemplatePropertyEditor
{
    public ObservableCollection<ItemDetailsSchema> ItemsForSaleViewItems { get; }
    public ObservableCollection<BindableString> ItemsToBuyViewItems { get; }
    public ListViewItem<MerchantTemplateSchema, MerchantTemplatePropertyEditor> ListItem { get; }
    public ObservableCollection<BindableString> ScriptKeysViewItems { get; }
    public ObservableCollection<BindableString> SkillsToTeachViewItems { get; }
    public ObservableCollection<BindableString> SpellsToTeachViewItems { get; }
    public TraceWrapper<MerchantTemplateSchema> Wrapper => ListItem.Wrapper;

    public MerchantTemplatePropertyEditor(ListViewItem<MerchantTemplateSchema, MerchantTemplatePropertyEditor> listItem)
    {
        ListItem = listItem;
        ScriptKeysViewItems = new ObservableCollection<BindableString>();
        ItemsForSaleViewItems = new ObservableCollection<ItemDetailsSchema>();
        ItemsToBuyViewItems = new ObservableCollection<BindableString>();
        SkillsToTeachViewItems = new ObservableCollection<BindableString>();
        SpellsToTeachViewItems = new ObservableCollection<BindableString>();

        InitializeComponent();
    }

    private void UserControl_Initialized(object sender, EventArgs e)
    {
        ScriptKeysView.ItemsSource = ScriptKeysViewItems;
        ItemsForSaleView.ItemsSource = ItemsForSaleViewItems;
        ItemsToBuyView.ItemsSource = ItemsToBuyViewItems;
        SkillsToTeachView.ItemsSource = SkillsToTeachViewItems;
        SpellsToTeachView.ItemsSource = SpellsToTeachViewItems;

        PopulateControlsFromItem();
    }

    #region Controls > Template > Controls
    public void CopySelectionsToItem()
    {
        var template = Wrapper.Object;

        Wrapper.Path = PathTbox.Text;
        template.TemplateKey = TemplateKeyTbox.Text;
        template.Name = NameTbox.Text;
        template.Sprite = ParsePrimitive<ushort>(SpriteTbox.Text);
        template.RestockPct = ParsePrimitive<int>(RestockPctTbox.Text);
        template.RestockIntervalHrs = ParsePrimitive<int>(RestockIntervalHrsTbox.Text);
        template.WanderIntervalMs = ParsePrimitive<int>(WanderIntervalMsTbox.Text);

        template.ScriptKeys = ScriptKeysViewItems.ToStrings().ToList();
        template.ItemsForSale = ItemsForSaleViewItems.Select(ShallowCopy<ItemDetailsSchema>.Create).ToList();
        template.ItemsToBuy = ItemsToBuyViewItems.ToStrings().ToList();
        template.SkillsToTeach = SkillsToTeachViewItems.ToStrings().ToList();
        template.SpellsToTeach = SpellsToTeachViewItems.ToStrings().ToList();

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
        SpriteTbox.Text = template.Sprite.ToString();
        RestockPctTbox.Text = template.RestockPct.ToString();
        RestockIntervalHrsTbox.Text = template.RestockIntervalHrs.ToString();
        WanderIntervalMsTbox.Text = template.WanderIntervalMs.ToString();

        ScriptKeysViewItems.Clear();
        ScriptKeysViewItems.AddRange(template.ScriptKeys.ToBindableStrings());

        ItemsForSaleViewItems.Clear();
        ItemsForSaleViewItems.AddRange(template.ItemsForSale.Select(ShallowCopy<ItemDetailsSchema>.Create));

        ItemsToBuyViewItems.Clear();
        ItemsToBuyViewItems.AddRange(template.ItemsToBuy.ToBindableStrings());

        SkillsToTeachViewItems.Clear();
        SkillsToTeachViewItems.AddRange(template.SkillsToTeach.ToBindableStrings());

        SpellsToTeachViewItems.Clear();
        SpellsToTeachViewItems.AddRange(template.SpellsToTeach.ToBindableStrings());
    }
    #endregion

    #region Buttons
    private void RevertBtn_Click(object sender, RoutedEventArgs e) => PopulateControlsFromItem();

    private async void SaveBtn_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var existing = JsonContext.MerchantTemplates.Objects.Where(wrapper => wrapper != Wrapper)
                                      .FirstOrDefault(wrapper => wrapper.Path.EqualsI(PathTbox.Text));

            if (existing is not null)
            {
                Snackbar.MessageQueue?.Enqueue($"Save failed. An item already exists at path \"{existing.Path}\"");

                return;
            }

            existing = JsonContext.MerchantTemplates.Objects.Where(wrapper => wrapper != Wrapper)
                                  .FirstOrDefault(wrapper => wrapper.Object.TemplateKey.EqualsI(TemplateKeyTbox.Text));

            if (existing is not null)
            {
                Snackbar.MessageQueue?.Enqueue(
                    $"Save failed. An item already exists with template key \"{existing.Object.TemplateKey}\" at path \"{existing.Path}\"");

                return;
            }

            existing = JsonContext.MerchantTemplates.Objects.FirstOrDefault(obj => ReferenceEquals(obj, Wrapper));

            if (existing is null)
                JsonContext.MerchantTemplates.Objects.Add(Wrapper);

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

        await JsonContext.MerchantTemplates.SaveItemAsync(Wrapper);
    }
    #endregion

    #region Tbox Validation
    private void TboxNumberValidator(object sender, TextCompositionEventArgs e) => Validators.NumberValidationTextBox(sender, e);

    private void TemplateKeyTbox_OnTextChanged(object sender, TextChangedEventArgs e) =>
        Validators.TemplateKeyMatchesFileName(TemplateKeyTbox, PathTbox);
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

    #region ItemsForSale Controls
    private void DeleteItemForSaleBtn_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        if (button.DataContext is not ItemDetailsSchema itemDetails)
            return;

        ItemsForSaleViewItems.Remove(itemDetails);
    }

    private void AddItemForSaleBtn_Click(object sender, RoutedEventArgs e) => ItemsForSaleViewItems.Add(new ItemDetailsSchema());
    #endregion

    #region ItemsToBuy Controls
    private void DeleteItemToBuyBtn_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        if (button.DataContext is not BindableString scriptKey)
            return;

        ItemsToBuyViewItems.Remove(scriptKey);
    }

    private void AddItemToBuyBtn_Click(object sender, RoutedEventArgs e) => ItemsToBuyViewItems.Add(string.Empty);
    #endregion

    #region SkillsToTeach Controls
    private void DeleteSkillToTeachBtn_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        if (button.DataContext is not BindableString scriptKey)
            return;

        SkillsToTeachViewItems.Remove(scriptKey);
    }

    private void AddSkillToTeachBtn_Click(object sender, RoutedEventArgs e) => SkillsToTeachViewItems.Add(string.Empty);
    #endregion

    #region SpellsToTeach Controls
    private void DeleteSpellToTeachBtn_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        if (button.DataContext is not BindableString scriptKey)
            return;

        SpellsToTeachViewItems.Remove(scriptKey);
    }

    private void AddSpellToTeachBtn_Click(object sender, RoutedEventArgs e) => SpellsToTeachViewItems.Add(string.Empty);
    #endregion
}