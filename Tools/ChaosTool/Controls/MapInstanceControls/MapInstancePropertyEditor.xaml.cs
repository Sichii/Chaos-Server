using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Chaos.Extensions.Common;
using ChaosTool.Definitions;
using ChaosTool.Extensions;
using ChaosTool.Model;
using ChaosTool.Model.Tables;

namespace ChaosTool.Controls.MapInstanceControls;

public sealed partial class MapInstancePropertyEditor
{
    public ListViewItem<MapInstanceRepository.MapInstanceComposite, MapInstancePropertyEditor> ListItem { get; }
    public ObservableCollection<BindableString> ScriptKeysViewItems { get; }
    public TraceWrapper<MapInstanceRepository.MapInstanceComposite> Wrapper => ListItem.Wrapper;

    public MapInstancePropertyEditor(ListViewItem<MapInstanceRepository.MapInstanceComposite, MapInstancePropertyEditor> listItem)
    {
        ListItem = listItem;
        ScriptKeysViewItems = new ObservableCollection<BindableString>();

        InitializeComponent();
    }

    #region Tbox Validation
    //private void TboxNumberValidator(object sender, TextCompositionEventArgs e) => Validators.NumberValidationTextBox(sender, e);

    private void TemplateKeyTbox_OnTextChanged(object sender, TextChangedEventArgs e)
        => Validators.TemplateKeyMatchesFileName(TemplateKeyTbox, PathTbox);
    #endregion

    private void UserControl_Initialized(object sender, EventArgs e)
    {
        ScriptKeysView.ItemsSource = ScriptKeysViewItems;

        PopulateControlsFromItem();
    }

    #region Controls > Template > Controls
    public void CopySelectionsToItem()
    {
        var composite = Wrapper.Object;

        Wrapper.Path = PathTbox.Text;
        composite.Instance.TemplateKey = TemplateKeyTbox.Text;

        composite.Instance.ScriptKeys = ScriptKeysViewItems.ToStrings()
                                                           .ToList();

        ListItem.Name = composite.Instance.TemplateKey;
    }

    public void PopulateControlsFromItem()
    {
        var template = Wrapper.Object;

        PathTbox.Text = Wrapper.Path;

        TemplateKeyTbox.IsEnabled = false;
        TemplateKeyTbox.Text = template.Instance.TemplateKey;
        TemplateKeyTbox.IsEnabled = true;

        ScriptKeysViewItems.Clear();
        ScriptKeysViewItems.AddRange(template.Instance.ScriptKeys.ToBindableStrings());
    }
    #endregion

    #region Buttons
    private void RevertBtn_Click(object sender, RoutedEventArgs e) => PopulateControlsFromItem();

    private async void SaveBtn_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var existing = JsonContext.MapInstances
                                      .Objects
                                      .Where(wrapper => wrapper != Wrapper)
                                      .FirstOrDefault(wrapper => wrapper.Path.EqualsI(PathTbox.Text));

            if (existing is not null)
            {
                Snackbar.MessageQueue?.Enqueue($"Save failed. An item already exists at path \"{existing.Path}\"");

                return;
            }

            existing = JsonContext.MapInstances
                                  .Objects
                                  .Where(wrapper => wrapper != Wrapper)
                                  .FirstOrDefault(wrapper => wrapper.Object.Instance.TemplateKey.EqualsI(TemplateKeyTbox.Text));

            if (existing is not null)
            {
                Snackbar.MessageQueue?.Enqueue(
                    $"Save failed. An item already exists with template key \"{existing.Object.Instance.TemplateKey}\" at path \"{existing
                        .Path}\"");

                return;
            }

            existing = JsonContext.MapInstances.Objects.FirstOrDefault(obj => ReferenceEquals(obj, Wrapper));

            if (existing is null)
                JsonContext.MapInstances.Objects.Add(Wrapper);

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

        await JsonContext.MapInstances.SaveItemAsync(Wrapper);
    }
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
}