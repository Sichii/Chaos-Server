using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Chaos.Extensions.Common;
using ChaosTool.Model;

namespace ChaosTool.Controls.IntegrityCheckControls;

public sealed partial class IntegrityCheckControl
{
    public MainWindow MainWindow { get; set; }

    public IntegrityCheckControl()
    {
        MainWindow = (MainWindow)Application.Current.MainWindow!;

        InitializeComponent();
    }

    private async Task AddViolationAsync(string violation, RoutedEventHandler handler, bool insertToHead = false) =>
        await Dispatcher.InvokeAsync(
            () =>
            {
                var button = new Button
                {
                    Content = violation,
                    Style = Application.Current.Resources["MaterialDesignFlatAccentBgButton"] as Style,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(
                        10,
                        3,
                        10,
                        3)
                };

                button.Click += handler;

                if (insertToHead)
                    IntegrityViolationsControl.Items.Insert(0, button);
                else
                    IntegrityViolationsControl.Items.Add(button);
            });

    /*
    private void DetectMapInstanceViolationsAsync()
    {
        foreach (var wrapper in JsonContext.MapInstances.Objects)
        {
            var mapInstance = wrapper.Object.Instance;
            var merchantSpawns = wrapper.Object.Merchants;
            var monsterSpawns = wrapper.Object.Monsters;
            var reactors = wrapper.Object.Reactors;
        }
    }*/

    private async Task DetectDialogTemplateViolationsAsync()
    {
        await Task.Yield();

        var acceptableKeys = new[] { "top", "close" };

        bool ValidateDialogKey(string key) => acceptableKeys.ContainsI(key)
                                              || JsonContext.DialogTemplates.Any(t => t.TemplateKey.EqualsI(key));

        foreach (var wrapper in JsonContext.DialogTemplates.Objects)
        {
            var template = wrapper.Object;
            var expectedTemplateKey = GetExpectedTemplateKey(wrapper);

            var handler = new RoutedEventHandler(
                (_, _) =>
                {
                    var dialogEditor = MainWindow.DialogTemplateEditor;
                    MainWindow.DialogsTab.IsSelected = true;

                    var selected = dialogEditor.ListViewItems.FirstOrDefault(obs => obs.Object == template);

                    if (selected is null)
                        throw new UnreachableException("We derived the selected item from the template, so it should exist.");

                    dialogEditor.TemplatesView.SelectedItem = selected;
                    dialogEditor.TemplatesView.ScrollIntoView(selected);
                });

            if (!template.TemplateKey.EqualsI(expectedTemplateKey))
                await AddViolationAsync($"TemplateKey mismatch: {template.TemplateKey} != {expectedTemplateKey}", handler, true);

            foreach (var option in template.Options)
            {
                var key = option.DialogKey;

                if (!ValidateDialogKey(key))
                    await AddViolationAsync($"Option.DialogKey not found: {key}", handler);
            }

            if ((template.NextDialogKey != null) && !ValidateDialogKey(template.NextDialogKey))
                await AddViolationAsync($"NextDialogKey not found: {template.NextDialogKey}", handler);

            if ((template.PrevDialogKey != null) && !ValidateDialogKey(template.PrevDialogKey))
                await AddViolationAsync($"PrevDialogKey not found: {template.PrevDialogKey}", handler);
        }
    }

    private async Task DetectIntegrityViolationsAsync()
    {
        IntegrityViolationsControl.Items.Clear();

        await Task.WhenAll(
            DetectDialogTemplateViolationsAsync(),
            DetectItemTemplateViolationsAsync(),
            DetectSkillTemplateViolationsAsync(),
            DetectSpellTemplateViolationsAsync(),
            DetectMapTemplateViolationsAsync(),
            DetectReactorTileTemplateViolationsAsync(),
            DetectMonsterTemplateViolationsAsync(),
            DetectMerchantTemplateViolationsAsync(),
            DetectLootTableViolationsAsync());

        if (IntegrityViolationsControl.Items.IsEmpty)
            await Dispatcher.InvokeAsync(
                () =>
                {
                    var label = new Label
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(
                            10,
                            200,
                            10,
                            200),
                        Content = "No integrity violations detected"
                    };

                    IntegrityViolationsControl.Items.Add(label);
                });
    }

    private async Task DetectItemTemplateViolationsAsync()
    {
        await Task.Yield();

        foreach (var wrapper in JsonContext.ItemTemplates.Objects)
        {
            var template = wrapper.Object;
            var expectedTemplateKey = GetExpectedTemplateKey(wrapper);

            var handler = new RoutedEventHandler(
                (_, _) =>
                {
                    var itemEditor = MainWindow.ItemTemplateEditor;
                    MainWindow.ItemsTab.IsSelected = true;

                    var selected = itemEditor.ListViewItems.FirstOrDefault(obs => obs.Object == template);

                    if (selected is null)
                        throw new UnreachableException("We derived the selected item from the template, so it should exist.");

                    itemEditor.TemplatesView.SelectedItem = selected;
                    itemEditor.TemplatesView.ScrollIntoView(selected);
                });

            if (!template.TemplateKey.EqualsI(expectedTemplateKey))
                await AddViolationAsync($"TemplateKey mismatch: {template.TemplateKey} != {expectedTemplateKey}", handler, true);
        }
    }

    private async Task DetectLootTableViolationsAsync()
    {
        await Task.Yield();

        foreach (var wrapper in JsonContext.LootTables.Objects)
        {
            var template = wrapper.Object;
            var expectedTemplateKey = GetExpectedTemplateKey(wrapper);

            var handler = new RoutedEventHandler(
                (_, _) =>
                {
                    var lootTableEditor = MainWindow.LootTableEditor;
                    MainWindow.LootTablesTab.IsSelected = true;

                    var selected = lootTableEditor.ListViewItems.FirstOrDefault(obs => obs.Object == template);

                    if (selected is null)
                        throw new UnreachableException("We derived the selected item from the template, so it should exist.");

                    lootTableEditor.TemplatesView.SelectedItem = selected;
                    lootTableEditor.TemplatesView.ScrollIntoView(selected);
                });

            if (!template.Key.EqualsI(expectedTemplateKey))
                await AddViolationAsync($"Key mismatch: {template.Key} != {expectedTemplateKey}", handler, true);

            foreach (var lootTableItem in template.LootDrops)
                if (!JsonContext.ItemTemplates.Any(obs => obs.TemplateKey.EqualsI(lootTableItem.ItemTemplateKey)))
                    await AddViolationAsync($"LootDrop.ItemTemplateKey not found: {lootTableItem.ItemTemplateKey}", handler);
        }
    }

    private async Task DetectMapTemplateViolationsAsync()
    {
        await Task.Yield();

        foreach (var wrapper in JsonContext.MapTemplates.Objects)
        {
            var template = wrapper.Object;
            var expectedTemplateKey = GetExpectedTemplateKey(wrapper);

            var handler = new RoutedEventHandler(
                (_, _) =>
                {
                    var mapEditor = MainWindow.MapTemplateEditor;
                    MainWindow.MapTemplatesTab.IsSelected = true;

                    var selected = mapEditor.ListViewItems.FirstOrDefault(obs => obs.Object == template);

                    if (selected is null)
                        throw new UnreachableException("We derived the selected item from the template, so it should exist.");

                    mapEditor.TemplatesView.SelectedItem = selected;
                    mapEditor.TemplatesView.ScrollIntoView(selected);
                });

            if (!template.TemplateKey.EqualsI(expectedTemplateKey))
                await AddViolationAsync($"TemplateKey mismatch: {template.TemplateKey} != {expectedTemplateKey}", handler, true);
        }
    }

    private async Task DetectMerchantTemplateViolationsAsync()
    {
        await Task.Yield();

        foreach (var wrapper in JsonContext.MerchantTemplates.Objects)
        {
            var template = wrapper.Object;
            var expectedTemplateKey = GetExpectedTemplateKey(wrapper);

            var handler = new RoutedEventHandler(
                (_, _) =>
                {
                    var merchantEditor = MainWindow.MerchantTemplateEditor;
                    MainWindow.MerchantTemplatesTab.IsSelected = true;

                    var selected = merchantEditor.ListViewItems.FirstOrDefault(obs => obs.Object == template);

                    if (selected is null)
                        throw new UnreachableException("We derived the selected item from the template, so it should exist.");

                    merchantEditor.TemplatesView.SelectedItem = selected;
                    merchantEditor.TemplatesView.ScrollIntoView(selected);
                });

            if (!template.TemplateKey.EqualsI(expectedTemplateKey))
                await AddViolationAsync($"TemplateKey mismatch: {template.TemplateKey} != {expectedTemplateKey}", handler, true);

            foreach (var itemForSale in template.ItemsForSale)
                if (!JsonContext.ItemTemplates.Any(obs => obs.TemplateKey.EqualsI(itemForSale.ItemTemplateKey)))
                    await AddViolationAsync($"ItemForSale.ItemTemplateKey not found: {itemForSale.ItemTemplateKey}", handler);

            foreach (var itemToBuy in template.ItemsToBuy)
                if (!JsonContext.ItemTemplates.Any(obs => obs.TemplateKey.EqualsI(itemToBuy)))
                    await AddViolationAsync($"ItemToBuy.ItemTemplateKey not found: {itemToBuy}", handler);

            foreach (var skillToTeach in template.SkillsToTeach)
                if (!JsonContext.SkillTemplates.Any(obs => obs.TemplateKey.EqualsI(skillToTeach)))
                    await AddViolationAsync($"SkillToTeach.SkillTemplateKey not found: {skillToTeach}", handler);

            foreach (var spellToTeach in template.SpellsToTeach)
                if (!JsonContext.SpellTemplates.Any(obs => obs.TemplateKey.EqualsI(spellToTeach)))
                    await AddViolationAsync($"SpellToTeach.SpellTemplateKey not found: {spellToTeach}", handler);
        }
    }

    private async Task DetectMonsterTemplateViolationsAsync()
    {
        await Task.Yield();

        foreach (var wrapper in JsonContext.MonsterTemplates.Objects)
        {
            var template = wrapper.Object;
            var expectedTemplateKey = GetExpectedTemplateKey(wrapper);

            var handler = new RoutedEventHandler(
                (_, _) =>
                {
                    var monsterEditor = MainWindow.MonsterTemplateEditor;
                    MainWindow.MonsterTemplatesTab.IsSelected = true;

                    var selected = monsterEditor.ListViewItems.FirstOrDefault(obs => obs.Object == template);

                    if (selected is null)
                        throw new UnreachableException("We derived the selected item from the template, so it should exist.");

                    monsterEditor.TemplatesView.SelectedItem = selected;
                    monsterEditor.TemplatesView.ScrollIntoView(selected);
                });

            if (!template.TemplateKey.EqualsI(expectedTemplateKey))
                await AddViolationAsync($"TemplateKey mismatch: {template.TemplateKey} != {expectedTemplateKey}", handler, true);

            foreach (var spellTemplateKey in template.SpellTemplateKeys)
                if (!JsonContext.SpellTemplates.Any(obs => obs.TemplateKey.EqualsI(spellTemplateKey)))
                    await AddViolationAsync($"Spells.SpellTemplateKey not found: {spellTemplateKey}", handler);

            foreach (var skillTemplateKey in template.SkillTemplateKeys)
                if (!JsonContext.SkillTemplates.Any(obs => obs.TemplateKey.EqualsI(skillTemplateKey)))
                    await AddViolationAsync($"Skills.SkillTemplateKey not found: {skillTemplateKey}", handler);

            foreach (var lootTableKey in template.LootTableKeys)
                if (!JsonContext.LootTables.Any(obs => obs.Key.EqualsI(lootTableKey)))
                    await AddViolationAsync($"LootTables.LootTableKey not found: {lootTableKey}", handler);
        }
    }

    private async Task DetectReactorTileTemplateViolationsAsync()
    {
        await Task.Yield();

        foreach (var wrapper in JsonContext.ReactorTileTemplates.Objects)
        {
            var template = wrapper.Object;
            var expectedTemplateKey = GetExpectedTemplateKey(wrapper);

            var handler = new RoutedEventHandler(
                (_, _) =>
                {
                    var reactorTileEditor = MainWindow.ReactorTileTemplateEditor;
                    MainWindow.ReactorTileTemplatesTab.IsSelected = true;

                    var selected = reactorTileEditor.ListViewItems.FirstOrDefault(obs => obs.Object == template);

                    if (selected is null)
                        throw new UnreachableException("We derived the selected item from the template, so it should exist.");

                    reactorTileEditor.TemplatesView.SelectedItem = selected;
                    reactorTileEditor.TemplatesView.ScrollIntoView(selected);
                });

            if (!template.TemplateKey.EqualsI(expectedTemplateKey))
                await AddViolationAsync($"TemplateKey mismatch: {template.TemplateKey} != {expectedTemplateKey}", handler, true);
        }
    }

    private async Task DetectSkillTemplateViolationsAsync()
    {
        await Task.Yield();

        foreach (var wrapper in JsonContext.SkillTemplates.Objects)
        {
            var template = wrapper.Object;
            var expectedTemplateKey = GetExpectedTemplateKey(wrapper);
            var learningRequirements = template.LearningRequirements;

            var handler = new RoutedEventHandler(
                (_, _) =>
                {
                    var skillEditor = MainWindow.SkillTemplateEditor;
                    MainWindow.SkillsTab.IsSelected = true;

                    var selected = skillEditor.ListViewItems.FirstOrDefault(obs => obs.Object == template);

                    if (selected is null)
                        throw new UnreachableException("We derived the selected item from the template, so it should exist.");

                    skillEditor.TemplatesView.SelectedItem = selected;
                    skillEditor.TemplatesView.ScrollIntoView(selected);
                });

            if (!template.TemplateKey.EqualsI(expectedTemplateKey))
                await AddViolationAsync($"TemplateKey mismatch: {template.TemplateKey} != {expectedTemplateKey}", handler, true);

            if (learningRequirements is null)
                continue;

            if (!learningRequirements.ItemRequirements.IsNullOrEmpty())
                foreach (var itemRequirement in learningRequirements.ItemRequirements)
                    if (!JsonContext.ItemTemplates.Any(obs => obs.TemplateKey.EqualsI(itemRequirement.ItemTemplateKey)))
                        await AddViolationAsync($"ItemRequirement.ItemTemplateKey not found: {itemRequirement.ItemTemplateKey}", handler);

            if (!learningRequirements.PrerequisiteSpellTemplateKeys.IsNullOrEmpty())
                foreach (var prerequisiteSpellTemplateKey in learningRequirements.PrerequisiteSpellTemplateKeys)
                    if (!JsonContext.SkillTemplates.Any(obs => obs.TemplateKey.EqualsI(prerequisiteSpellTemplateKey)))
                        await AddViolationAsync($"PrerequisiteSpellTemplateKey not found: {prerequisiteSpellTemplateKey}", handler);

            if (!learningRequirements.PrerequisiteSkillTemplateKeys.IsNullOrEmpty())
                foreach (var prerequisiteSkillTemplateKey in learningRequirements.PrerequisiteSkillTemplateKeys)
                    if (!JsonContext.SkillTemplates.Any(obs => obs.TemplateKey.EqualsI(prerequisiteSkillTemplateKey)))
                        await AddViolationAsync($"PrerequisiteSkillTemplateKey not found: {prerequisiteSkillTemplateKey}", handler);
        }
    }

    private async Task DetectSpellTemplateViolationsAsync()
    {
        await Task.Yield();

        foreach (var wrapper in JsonContext.SpellTemplates.Objects)
        {
            var template = wrapper.Object;
            var expectedTemplateKey = GetExpectedTemplateKey(wrapper);
            var learningRequirements = template.LearningRequirements;

            var handler = new RoutedEventHandler(
                (_, _) =>
                {
                    var spellEditor = MainWindow.SpellTemplateEditor;
                    MainWindow.SpellsTab.IsSelected = true;

                    var selected = spellEditor.ListViewItems.FirstOrDefault(obs => obs.Object == template);

                    if (selected is null)
                        throw new UnreachableException("We derived the selected item from the template, so it should exist.");

                    spellEditor.TemplatesView.SelectedItem = selected;
                    spellEditor.TemplatesView.ScrollIntoView(selected);
                });

            if (!template.TemplateKey.EqualsI(expectedTemplateKey))
                await AddViolationAsync($"TemplateKey mismatch: {template.TemplateKey} != {expectedTemplateKey}", handler, true);

            if (learningRequirements is null)
                continue;

            if (!learningRequirements.ItemRequirements.IsNullOrEmpty())
                foreach (var itemRequirement in learningRequirements.ItemRequirements)
                    if (!JsonContext.ItemTemplates.Any(obs => obs.TemplateKey.EqualsI(itemRequirement.ItemTemplateKey)))
                        await AddViolationAsync($"ItemRequirement.ItemTemplateKey not found: {itemRequirement.ItemTemplateKey}", handler);

            if (!learningRequirements.PrerequisiteSpellTemplateKeys.IsNullOrEmpty())
                foreach (var prerequisiteSpellTemplateKey in learningRequirements.PrerequisiteSpellTemplateKeys)
                    if (!JsonContext.SpellTemplates.Any(obs => obs.TemplateKey.EqualsI(prerequisiteSpellTemplateKey)))
                        await AddViolationAsync($"PrerequisiteSpellTemplateKey not found: {prerequisiteSpellTemplateKey}", handler);

            if (!learningRequirements.PrerequisiteSkillTemplateKeys.IsNullOrEmpty())
                foreach (var prerequisiteSkillTemplateKey in learningRequirements.PrerequisiteSkillTemplateKeys)
                    if (!JsonContext.SkillTemplates.Any(obs => obs.TemplateKey.EqualsI(prerequisiteSkillTemplateKey)))
                        await AddViolationAsync($"PrerequisiteSkillTemplateKey not found: {prerequisiteSkillTemplateKey}", handler);
        }
    }

    private string GetExpectedTemplateKey<T>(TraceWrapper<T> wrapper) => Path.GetFileNameWithoutExtension(wrapper.Path);

    private async void IntegrityCheckBtn_OnClick(object sender, RoutedEventArgs e)
    {
        await JsonContext.LoadingTask;

        await DetectIntegrityViolationsAsync().ConfigureAwait(false);
    }
}