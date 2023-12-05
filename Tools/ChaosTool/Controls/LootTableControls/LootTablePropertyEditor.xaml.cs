using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Extensions.Common;
using Chaos.Schemas.Content;
using Chaos.Schemas.Data;
using ChaosTool.Definitions;
using ChaosTool.Model;

namespace ChaosTool.Controls.LootTableControls;

public sealed partial class LootTablePropertyEditor
{
    public ListViewItem<LootTableSchema, LootTablePropertyEditor> ListItem { get; }
    public ObservableCollection<LootDropSchema> LootDropsViewItems { get; }
    public TraceWrapper<LootTableSchema> Wrapper => ListItem.Wrapper;

    public LootTablePropertyEditor(ListViewItem<LootTableSchema, LootTablePropertyEditor> listItem)
    {
        ListItem = listItem;
        LootDropsViewItems = new ObservableCollection<LootDropSchema>();

        InitializeComponent();
    }

    #region Tbox Validation
    private void TemplateKeyTbox_OnTextChanged(object sender, TextChangedEventArgs e)
        => Validators.TemplateKeyMatchesFileName(KeyTbox, PathTbox);
    #endregion

    private void UserControl_Initialized(object sender, EventArgs e)
    {
        LootDropsView.ItemsSource = LootDropsViewItems;
        ModeCmbox.ItemsSource = GetEnumNames<LootTableMode>();

        PopulateControlsFromItem();
    }

    #region Controls > Template > Controls
    public void CopySelectionsToItem()
    {
        var template = Wrapper.Object;

        Wrapper.Path = PathTbox.Text;
        template.Key = KeyTbox.Text;
        template.Mode = ParsePrimitive<LootTableMode>(ModeCmbox.Text);

        template.LootDrops = LootDropsViewItems.Select(ShallowCopy<LootDropSchema>.Create)
                                               .ToList();

        ListItem.Name = template.Key;
    }

    public void PopulateControlsFromItem()
    {
        var template = Wrapper.Object;

        PathTbox.Text = Wrapper.Path;

        KeyTbox.IsEnabled = false;
        KeyTbox.Text = template.Key;
        KeyTbox.IsEnabled = true;

        ModeCmbox.SelectedItem = SelectPrimitive(template.Mode, ModeCmbox.ItemsSource);

        LootDropsViewItems.Clear();
        LootDropsViewItems.AddRange(template.LootDrops.Select(ShallowCopy<LootDropSchema>.Create));
    }
    #endregion

    #region Buttons
    private void RevertBtn_Click(object sender, RoutedEventArgs e) => PopulateControlsFromItem();

    private async void SaveBtn_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var existing = JsonContext.LootTables
                                      .Objects
                                      .Where(wrapper => wrapper != Wrapper)
                                      .FirstOrDefault(wrapper => wrapper.Path.EqualsI(PathTbox.Text));

            if (existing is not null)
            {
                Snackbar.MessageQueue?.Enqueue($"Save failed. An item already exists at path \"{existing.Path}\"");

                return;
            }

            existing = JsonContext.LootTables
                                  .Objects
                                  .Where(wrapper => wrapper != Wrapper)
                                  .FirstOrDefault(wrapper => wrapper.Object.Key.EqualsI(KeyTbox.Text));

            if (existing is not null)
            {
                Snackbar.MessageQueue?.Enqueue(
                    $"Save failed. An item already exists with key \"{existing.Object.Key}\" at path \"{existing.Path}\"");

                return;
            }

            existing = JsonContext.LootTables.Objects.FirstOrDefault(obj => ReferenceEquals(obj, Wrapper));

            if (existing is null)
                JsonContext.LootTables.Objects.Add(Wrapper);

            if (!ValidatePreSave(Wrapper, PathTbox, KeyTbox))
            {
                Snackbar.MessageQueue?.Enqueue("Filename does not match the key");

                return;
            }

            CopySelectionsToItem();
            PopulateControlsFromItem();
        } catch (Exception ex)
        {
            Snackbar.MessageQueue?.Enqueue(ex.ToString());
        }

        await JsonContext.LootTables.SaveItemAsync(Wrapper);
    }
    #endregion

    #region LootDrops Controls
    private void DeleteLootDropBtn_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        if (button.DataContext is not LootDropSchema lootDrop)
            return;

        LootDropsViewItems.Remove(lootDrop);
    }

    private void AddLootDropBtn_Click(object sender, RoutedEventArgs e) => LootDropsViewItems.Add(new LootDropSchema());
    #endregion
}