using System.Collections.Frozen;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Chaos.Common.Definitions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Geometry;
using Chaos.Schemas.Content;
using Chaos.Schemas.Templates;
using ChaosTool.Model;
using ChaosTool.Model.Tables;
using ChaosTool.ViewModel;

namespace ChaosTool.Controls.IntegrityCheckControls;

public sealed partial class IntegrityCheckControl
{
    public ISet<string> BuyableItemsIndex { get; set; } = null!;
    public IReadOnlyDictionary<string, DialogTemplateSchema> DialogTemplateIndex { get; set; } = null!;
    public ISet<string> InUseMapTemplateIndex { get; set; } = null!;
    public IReadOnlyDictionary<string, ItemTemplateSchema> ItemTemplateIndex { get; set; } = null!;
    public ISet<string> LearnableSkillsIndex { get; set; } = null!;
    public ISet<string> LearnableSpellsIndex { get; set; } = null!;
    public IReadOnlyDictionary<string, LootTableSchema> LootTableIndex { get; set; } = null!;
    public MainWindow MainWindow { get; set; }
    public IReadOnlyDictionary<string, MapInstanceRepository.MapInstanceComposite> MapInstanceIndex { get; set; } = null!;
    public IReadOnlyDictionary<string, MapTemplateSchema> MapTemplateIndex { get; set; } = null!;
    public IReadOnlyDictionary<string, MerchantTemplateSchema> MerchantTemplateIndex { get; set; } = null!;
    public IReadOnlyDictionary<string, MonsterTemplateSchema> MonsterTemplateIndex { get; set; } = null!;
    public IReadOnlyDictionary<string, ReactorTileTemplateSchema> ReactorTileTemplateIndex { get; set; } = null!;
    public ISet<string> SellableItemsIndex { get; set; } = null!;
    public IReadOnlyDictionary<string, SkillTemplateSchema> SkillTemplateIndex { get; set; } = null!;
    public IReadOnlyDictionary<string, SpellTemplateSchema> SpellTemplateIndex { get; set; } = null!;
    public IReadOnlyDictionary<string, WorldMapSchema> WorldMapIndex { get; set; } = null!;
    public IReadOnlyDictionary<string, WorldMapNodeSchema> WorldMapNodeIndex { get; set; } = null!;

    public IntegrityCheckControl()
    {
        MainWindow = (MainWindow)Application.Current.MainWindow!;

        InitializeComponent();
    }

    private async Task DetectDialogTemplateViolationsAsync()
    {
        await Task.Yield();

        var acceptableKeys = new[]
        {
            "top",
            "close"
        };

        foreach (var wrapper in JsonContext.DialogTemplates.Objects)
        {
            var template = wrapper.Object;
            var expectedTemplateKey = GetExpectedKey(wrapper);

            var handler = new RoutedEventHandler(
                (_, _) =>
                {
                    var dialogListView = MainWindow.DialogTemplateListView;
                    MainWindow.DialogsTab.IsSelected = true;

                    var selected = dialogListView.Items
                                                 .OfType<DialogTemplateViewModel>()
                                                 .FirstOrDefault(obs => obs.TemplateKey.EqualsI(template.TemplateKey));

                    if (selected is null)
                        throw new UnreachableException("We derived the selected item from the template, so it should exist.");

                    dialogListView.ItemsView.SelectedItem = selected;
                    dialogListView.ItemsView.ScrollIntoView(selected);
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

            if (template.Type != ChaosDialogType.DialogTextEntry)
            {
                if (template.TextBoxLength.HasValue)
                    await AddViolationAsync("TextBoxLength should only be specified for DialogTextEntry", handler);

                if (!string.IsNullOrEmpty(template.TextBoxPrompt))
                    await AddViolationAsync("TextBoxPrompt should only be specified for DialogTextEntry", handler);
            } else if (!template.TextBoxLength.HasValue && string.IsNullOrEmpty(template.TextBoxPrompt))
                await AddViolationAsync("TextBoxLength AND/OR TextBoxPrompt should be specified for DialogTextEntry", handler);

            if (template.NextDialogKey is null
                && template.Type is not ChaosDialogType.DialogMenu
                                    and not ChaosDialogType.Menu
                                    and not ChaosDialogType.MenuWithArgs
                                    and not ChaosDialogType.CloseDialog
                                    and not ChaosDialogType.Normal)
                await AddViolationAsync("NextDialogKey should be specified for given menu type", handler);
        }

        return;

        bool ValidateDialogKey(string key) => acceptableKeys.ContainsI(key) || DialogTemplateIndex.ContainsKey(key);
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
            DetectLootTableViolationsAsync(),
            DetectMapInstanceViolationsAsync(),
            DetectWorldMapNodeViolationsAsync(),
            DetectWorldMapViolationsAsync());

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
            var expectedTemplateKey = GetExpectedKey(wrapper);

            var handler = new RoutedEventHandler(
                (_, _) =>
                {
                    var itemListView = MainWindow.ItemTemplateListView;
                    MainWindow.ItemsTab.IsSelected = true;

                    var selected = itemListView.Items
                                               .OfType<ItemTemplateViewModel>()
                                               .FirstOrDefault(obs => obs.TemplateKey.EqualsI(template.TemplateKey));

                    if (selected is null)
                        throw new UnreachableException("We derived the selected item from the template, so it should exist.");

                    itemListView.ItemsView.SelectedItem = selected;
                    itemListView.ItemsView.ScrollIntoView(selected);
                });

            if (!template.TemplateKey.EqualsI(expectedTemplateKey))
                await AddViolationAsync($"TemplateKey mismatch: {template.TemplateKey} != {expectedTemplateKey}", handler, true);

            if (template.EquipmentType.HasValue
                && (template.EquipmentType.Value != EquipmentType.NotEquipment)
                && (template.SellValue == 0))
                await AddViolationAsync("Equippable item has sellValue of 0", handler);

            //if the item is bought by a merchant
            if (SellableItemsIndex.Contains(template.TemplateKey))
                if (template.SellValue == 0)
                    await AddViolationAsync("Sellable item has sellValue of 0", handler);

            //if the item is sold by a merchant
            if (BuyableItemsIndex.Contains(template.TemplateKey))
                if (template.BuyCost == 0)
                    await AddViolationAsync("Buyable item has buyCost of 0", handler);

            if (SellableItemsIndex.Contains(template.TemplateKey) && BuyableItemsIndex.Contains(template.TemplateKey))
                if (template.SellValue > template.BuyCost)
                    await AddViolationAsync($"Sellable item has sellValue > buyCost: {template.SellValue} > {template.BuyCost}", handler);
        }
    }

    private async Task DetectLootTableViolationsAsync()
    {
        await Task.Yield();

        foreach (var wrapper in JsonContext.LootTables.Objects)
        {
            var template = wrapper.Object;
            var expectedKey = GetExpectedKey(wrapper);

            var handler = new RoutedEventHandler(
                (_, _) =>
                {
                    var lootTableListView = MainWindow.LootTableListView;
                    MainWindow.LootTablesTab.IsSelected = true;

                    var selected = lootTableListView.Items
                                                    .OfType<LootTableViewModel>()
                                                    .FirstOrDefault(obs => obs.Key.EqualsI(template.Key));

                    if (selected is null)
                        throw new UnreachableException("We derived the selected item from the template, so it should exist.");

                    lootTableListView.ItemsView.SelectedItem = selected;
                    lootTableListView.ItemsView.ScrollIntoView(selected);
                });

            if (!template.Key.EqualsI(expectedKey))
                await AddViolationAsync($"Key mismatch: {template.Key} != {expectedKey}", handler, true);

            foreach (var lootTableItem in template.LootDrops)
                if (!ItemTemplateIndex.ContainsKey(lootTableItem.ItemTemplateKey))
                    await AddViolationAsync($"LootDrop.ItemTemplateKey not found: {lootTableItem.ItemTemplateKey}", handler);
        }
    }

    private async Task DetectMapTemplateViolationsAsync()
    {
        await Task.Yield();

        foreach (var wrapper in JsonContext.MapTemplates.Objects)
        {
            var template = wrapper.Object;
            var expectedTemplateKey = GetExpectedKey(wrapper);

            var handler = new RoutedEventHandler(
                (_, _) =>
                {
                    var mapListView = MainWindow.MapTemplateListView;
                    MainWindow.MapTemplatesTab.IsSelected = true;

                    var selected = mapListView.Items
                                              .OfType<MapTemplateViewModel>()
                                              .FirstOrDefault(obs => obs.TemplateKey.EqualsI(template.TemplateKey));

                    if (selected is null)
                        throw new UnreachableException("We derived the selected item from the template, so it should exist.");

                    mapListView.ItemsView.SelectedItem = selected;
                    mapListView.ItemsView.ScrollIntoView(selected);
                });

            if (!template.TemplateKey.EqualsI(expectedTemplateKey))
                await AddViolationAsync($"TemplateKey mismatch: {template.TemplateKey} != {expectedTemplateKey}", handler, true);

            if (((template.Width == 0) || (template.Height == 0)) && InUseMapTemplateIndex.Contains(template.TemplateKey))
                await AddViolationAsync("In-Use MapTemplate has 0 width or height", handler);
        }
    }

    private async Task DetectMerchantTemplateViolationsAsync()
    {
        await Task.Yield();

        foreach (var wrapper in JsonContext.MerchantTemplates.Objects)
        {
            var template = wrapper.Object;
            var expectedTemplateKey = GetExpectedKey(wrapper);

            var handler = new RoutedEventHandler(
                (_, _) =>
                {
                    var merchantListView = MainWindow.MerchantTemplateListView;
                    MainWindow.MerchantTemplatesTab.IsSelected = true;

                    var selected = merchantListView.Items
                                                   .OfType<MerchantTemplateViewModel>()
                                                   .FirstOrDefault(obs => obs.TemplateKey.EqualsI(template.TemplateKey));

                    if (selected is null)
                        throw new UnreachableException("We derived the selected item from the template, so it should exist.");

                    merchantListView.ItemsView.SelectedItem = selected;
                    merchantListView.ItemsView.ScrollIntoView(selected);
                });

            if (!template.TemplateKey.EqualsI(expectedTemplateKey))
                await AddViolationAsync($"TemplateKey mismatch: {template.TemplateKey} != {expectedTemplateKey}", handler, true);

            foreach (var itemForSale in template.ItemsForSale)
            {
                if (!ItemTemplateIndex.ContainsKey(itemForSale.ItemTemplateKey))
                    await AddViolationAsync($"ItemForSale.ItemTemplateKey not found: {itemForSale.ItemTemplateKey}", handler);

                if (itemForSale.Stock == 0)
                    await AddViolationAsync("ItemForSale.Stock is 0 (use -1 for infinite)", handler);
            }

            foreach (var itemToBuy in template.ItemsToBuy)
                if (!ItemTemplateIndex.ContainsKey(itemToBuy))
                    await AddViolationAsync($"ItemToBuy.ItemTemplateKey not found: {itemToBuy}", handler);

            foreach (var skillToTeach in template.SkillsToTeach)
                if (!SkillTemplateIndex.ContainsKey(skillToTeach))
                    await AddViolationAsync($"SkillToTeach.SkillTemplateKey not found: {skillToTeach}", handler);

            foreach (var spellToTeach in template.SpellsToTeach)
                if (!SpellTemplateIndex.ContainsKey(spellToTeach))
                    await AddViolationAsync($"SpellToTeach.SpellTemplateKey not found: {spellToTeach}", handler);
        }
    }

    private async Task DetectMonsterTemplateViolationsAsync()
    {
        await Task.Yield();

        foreach (var wrapper in JsonContext.MonsterTemplates.Objects)
        {
            var template = wrapper.Object;
            var expectedTemplateKey = GetExpectedKey(wrapper);

            var handler = new RoutedEventHandler(
                (_, _) =>
                {
                    var monsterListView = MainWindow.MonsterTemplateListView;
                    MainWindow.MonsterTemplatesTab.IsSelected = true;

                    var selected = monsterListView.Items
                                                  .OfType<MonsterTemplateViewModel>()
                                                  .FirstOrDefault(obs => obs.TemplateKey.EqualsI(template.TemplateKey));

                    if (selected is null)
                        throw new UnreachableException("We derived the selected item from the template, so it should exist.");

                    monsterListView.ItemsView.SelectedItem = selected;
                    monsterListView.ItemsView.ScrollIntoView(selected);
                });

            if (!template.TemplateKey.EqualsI(expectedTemplateKey))
                await AddViolationAsync($"TemplateKey mismatch: {template.TemplateKey} != {expectedTemplateKey}", handler, true);

            foreach (var spellTemplateKey in template.SpellTemplateKeys)
                if (!SpellTemplateIndex.ContainsKey(spellTemplateKey))
                    await AddViolationAsync($"Spells.SpellTemplateKey not found: {spellTemplateKey}", handler);

            foreach (var skillTemplateKey in template.SkillTemplateKeys)
                if (!SkillTemplateIndex.ContainsKey(skillTemplateKey))
                    await AddViolationAsync($"Skills.SkillTemplateKey not found: {skillTemplateKey}", handler);

            foreach (var lootTableKey in template.LootTableKeys)
                if (!LootTableIndex.ContainsKey(lootTableKey))
                    await AddViolationAsync($"LootTables.LootTableKey not found: {lootTableKey}", handler);
        }
    }

    private async Task DetectReactorTileTemplateViolationsAsync()
    {
        await Task.Yield();

        foreach (var wrapper in JsonContext.ReactorTileTemplates.Objects)
        {
            var template = wrapper.Object;
            var expectedTemplateKey = GetExpectedKey(wrapper);

            var handler = new RoutedEventHandler(
                (_, _) =>
                {
                    var reactorTileListView = MainWindow.ReactorTileTemplateListView;
                    MainWindow.ReactorTileTemplatesTab.IsSelected = true;

                    var selected = reactorTileListView.Items
                                                      .OfType<ReactorTileTemplateViewModel>()
                                                      .FirstOrDefault(obs => obs.TemplateKey.EqualsI(template.TemplateKey));

                    if (selected is null)
                        throw new UnreachableException("We derived the selected item from the template, so it should exist.");

                    reactorTileListView.ItemsView.SelectedItem = selected;
                    reactorTileListView.ItemsView.ScrollIntoView(selected);
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
            var expectedTemplateKey = GetExpectedKey(wrapper);
            var learningRequirements = template.LearningRequirements;

            var handler = new RoutedEventHandler(
                (_, _) =>
                {
                    var skillListView = MainWindow.SkillTemplateListView;
                    MainWindow.SkillsTab.IsSelected = true;

                    var selected = skillListView.Items
                                                .OfType<SkillTemplateViewModel>()
                                                .FirstOrDefault(obs => obs.TemplateKey.EqualsI(template.TemplateKey));

                    if (selected is null)
                        throw new UnreachableException("We derived the selected item from the template, so it should exist.");

                    skillListView.ItemsView.SelectedItem = selected;
                    skillListView.ItemsView.ScrollIntoView(selected);
                });

            if (!template.TemplateKey.EqualsI(expectedTemplateKey))
                await AddViolationAsync($"TemplateKey mismatch: {template.TemplateKey} != {expectedTemplateKey}", handler, true);

            if (learningRequirements is null)
                continue;

            if (!learningRequirements.ItemRequirements.IsNullOrEmpty())
                foreach (var itemRequirement in learningRequirements.ItemRequirements)
                    if (!ItemTemplateIndex.ContainsKey(itemRequirement.ItemTemplateKey))
                        await AddViolationAsync($"ItemRequirement.ItemTemplateKey not found: {itemRequirement.ItemTemplateKey}", handler);

            if (!learningRequirements.PrerequisiteSpells.IsNullOrEmpty())
                foreach (var prerequisiteSpell in learningRequirements.PrerequisiteSpells)
                    if (!SpellTemplateIndex.TryGetValue(prerequisiteSpell.TemplateKey, out var spellTemplate))
                        await AddViolationAsync($"PrerequisiteSpell not found: {prerequisiteSpell.TemplateKey}", handler);
                    else if (prerequisiteSpell.Level.HasValue)
                    {
                        if (!spellTemplate.LevelsUp)
                            await AddViolationAsync(
                                $"Required level specified for non-leveling spell: {prerequisiteSpell.TemplateKey}",
                                handler);

                        if (prerequisiteSpell.Level > spellTemplate.MaxLevel)
                            await AddViolationAsync($"Required level > max level: {prerequisiteSpell.TemplateKey}", handler);
                    }

            if (!learningRequirements.PrerequisiteSkills.IsNullOrEmpty())
                foreach (var prerequisiteSkill in learningRequirements.PrerequisiteSkills)
                    if (!SkillTemplateIndex.TryGetValue(prerequisiteSkill.TemplateKey, out var skillTemplate))
                        await AddViolationAsync($"PrerequisiteSkill not found: {prerequisiteSkill.TemplateKey}", handler);
                    else if (prerequisiteSkill.Level.HasValue)
                    {
                        if (!skillTemplate.LevelsUp)
                            await AddViolationAsync(
                                $"Required level specified for non-leveling skill: {prerequisiteSkill.TemplateKey}",
                                handler);

                        if (prerequisiteSkill.Level > skillTemplate.MaxLevel)
                            await AddViolationAsync($"Required level > max level: {prerequisiteSkill.TemplateKey}", handler);
                    }
        }
    }

    private async Task DetectSpellTemplateViolationsAsync()
    {
        await Task.Yield();

        foreach (var wrapper in JsonContext.SpellTemplates.Objects)
        {
            var template = wrapper.Object;
            var expectedTemplateKey = GetExpectedKey(wrapper);
            var learningRequirements = template.LearningRequirements;

            var handler = new RoutedEventHandler(
                (_, _) =>
                {
                    var spellListView = MainWindow.SpellTemplateListView;
                    MainWindow.SpellsTab.IsSelected = true;

                    var selected = spellListView.Items
                                                .OfType<SpellTemplateViewModel>()
                                                .FirstOrDefault(obs => obs.TemplateKey.EqualsI(template.TemplateKey));

                    if (selected is null)
                        throw new UnreachableException("We derived the selected item from the template, so it should exist.");

                    spellListView.ItemsView.SelectedItem = selected;
                    spellListView.ItemsView.ScrollIntoView(selected);
                });

            if (!template.TemplateKey.EqualsI(expectedTemplateKey))
                await AddViolationAsync($"TemplateKey mismatch: {template.TemplateKey} != {expectedTemplateKey}", handler, true);

            if (learningRequirements is null)
                continue;

            if (!learningRequirements.ItemRequirements.IsNullOrEmpty())
                foreach (var itemRequirement in learningRequirements.ItemRequirements)
                    if (!ItemTemplateIndex.ContainsKey(itemRequirement.ItemTemplateKey))
                        await AddViolationAsync($"ItemRequirement.ItemTemplateKey not found: {itemRequirement.ItemTemplateKey}", handler);

            if (!learningRequirements.PrerequisiteSpells.IsNullOrEmpty())
                foreach (var prerequisiteSpell in learningRequirements.PrerequisiteSpells)
                    if (!SpellTemplateIndex.ContainsKey(prerequisiteSpell.TemplateKey))
                        await AddViolationAsync($"PrerequisiteSpellTemplateKey not found: {prerequisiteSpell.TemplateKey}", handler);

            if (!learningRequirements.PrerequisiteSkills.IsNullOrEmpty())
                foreach (var prerequisiteSkill in learningRequirements.PrerequisiteSkills)
                    if (!SkillTemplateIndex.ContainsKey(prerequisiteSkill.TemplateKey))
                        await AddViolationAsync($"PrerequisiteSkillTemplateKey not found: {prerequisiteSkill.TemplateKey}", handler);
        }
    }

    private async Task DetectWorldMapNodeViolationsAsync()
    {
        await Task.Yield();

        var clientRect = new Rectangle(
            0,
            0,
            640,
            480);

        foreach (var wrapper in JsonContext.WorldMapNodes.Objects)
        {
            var node = wrapper.Object;
            var expectedNodeKey = GetExpectedKey(wrapper);

            var handler = new RoutedEventHandler(
                (_, _) =>
                {
                    var nodeListView = MainWindow.WorldMapNodeListView;
                    MainWindow.WorldMapNodesTab.IsSelected = true;

                    var selected = nodeListView.Items
                                               .OfType<WorldMapNodeViewModel>()
                                               .FirstOrDefault(obs => obs.NodeKey.EqualsI(node.NodeKey));

                    if (selected is null)
                        throw new UnreachableException("We derived the selected item from the template, so it should exist.");

                    nodeListView.ItemsView.SelectedItem = selected;
                    nodeListView.ItemsView.ScrollIntoView(selected);
                });

            if (!node.NodeKey.EqualsI(expectedNodeKey))
                await AddViolationAsync($"NodeKey mismatch: {node.NodeKey} != {expectedNodeKey}", handler, true);

            if (!clientRect.Contains(node.ScreenPosition))
                await AddViolationAsync("ScreenPosition out of bounds", handler);

            if (!MapInstanceIndex.TryGetValue(node.Destination.Map, out var destinationMapInstance))
                await AddViolationAsync($"Destination map not found: {node.Destination.Map}", handler);

            if (destinationMapInstance is not null
                && MapTemplateIndex.TryGetValue(destinationMapInstance.Instance.TemplateKey, out var destinationMapTemplate))
            {
                var bounds = new Rectangle(
                    0,
                    0,
                    destinationMapTemplate.Width,
                    destinationMapTemplate.Height);

                if (!bounds.Contains(node.Destination))
                    await AddViolationAsync("Destination out of bounds", handler);
            }
        }
    }

    private async Task DetectWorldMapViolationsAsync()
    {
        await Task.Yield();

        foreach (var wrapper in JsonContext.WorldMaps.Objects)
        {
            var worldMap = wrapper.Object;
            var expectdWorldMapKey = GetExpectedKey(wrapper);

            var handler = new RoutedEventHandler(
                (_, _) =>
                {
                    var worldMapListView = MainWindow.WorldMapListView;
                    MainWindow.WorldMapsTab.IsSelected = true;

                    var selected = worldMapListView.Items
                                                   .OfType<WorldMapViewModel>()
                                                   .FirstOrDefault(obs => obs.WorldMapKey.EqualsI(worldMap.WorldMapKey));

                    if (selected is null)
                        throw new UnreachableException("We derived the selected item from the template, so it should exist.");

                    worldMapListView.ItemsView.SelectedItem = selected;
                    worldMapListView.ItemsView.ScrollIntoView(selected);
                });

            if (!worldMap.WorldMapKey.EqualsI(expectdWorldMapKey))
                await AddViolationAsync($"WorldMapKey mismatch: {worldMap.WorldMapKey} != {expectdWorldMapKey}", handler, true);

            foreach (var nodeKey in worldMap.NodeKeys)
                if (!WorldMapNodeIndex.ContainsKey(nodeKey))
                    await AddViolationAsync($"NodeKey not found: {nodeKey}", handler);
        }
    }

    #region Utility
    private async Task AddViolationAsync(string violation, RoutedEventHandler handler, bool insertToHead = false)
        => await Dispatcher.InvokeAsync(
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

    private string GetExpectedKey<T>(TraceWrapper<T> wrapper) => Path.GetFileNameWithoutExtension(wrapper.Path);

    private async void IntegrityCheckBtn_OnClick(object sender, RoutedEventArgs e)
    {
        await JsonContext.LoadingTask;

        ReBuildIndexes();

        await DetectIntegrityViolationsAsync()
            .ConfigureAwait(false);
    }

    private void ReBuildIndexes()
    {
        MapInstanceIndex = JsonContext.MapInstances.ToFrozenDictionary(mi => mi.Instance.InstanceId, StringComparer.OrdinalIgnoreCase);
        MapTemplateIndex = JsonContext.MapTemplates.ToFrozenDictionary(mt => mt.TemplateKey, StringComparer.OrdinalIgnoreCase);
        MerchantTemplateIndex = JsonContext.MerchantTemplates.ToFrozenDictionary(mt => mt.TemplateKey, StringComparer.OrdinalIgnoreCase);
        MonsterTemplateIndex = JsonContext.MonsterTemplates.ToFrozenDictionary(mt => mt.TemplateKey, StringComparer.OrdinalIgnoreCase);
        ItemTemplateIndex = JsonContext.ItemTemplates.ToFrozenDictionary(it => it.TemplateKey, StringComparer.OrdinalIgnoreCase);
        SkillTemplateIndex = JsonContext.SkillTemplates.ToFrozenDictionary(st => st.TemplateKey, StringComparer.OrdinalIgnoreCase);
        SpellTemplateIndex = JsonContext.SpellTemplates.ToFrozenDictionary(st => st.TemplateKey, StringComparer.OrdinalIgnoreCase);
        DialogTemplateIndex = JsonContext.DialogTemplates.ToFrozenDictionary(dt => dt.TemplateKey, StringComparer.OrdinalIgnoreCase);
        LootTableIndex = JsonContext.LootTables.ToFrozenDictionary(lt => lt.Key, StringComparer.OrdinalIgnoreCase);
        WorldMapNodeIndex = JsonContext.WorldMapNodes.ToFrozenDictionary(wmn => wmn.NodeKey, StringComparer.OrdinalIgnoreCase);
        WorldMapIndex = JsonContext.WorldMaps.ToFrozenDictionary(wm => wm.WorldMapKey, StringComparer.OrdinalIgnoreCase);

        ReactorTileTemplateIndex = JsonContext.ReactorTileTemplates.ToFrozenDictionary(
            rt => rt.TemplateKey,
            StringComparer.OrdinalIgnoreCase);

        BuyableItemsIndex = JsonContext.MerchantTemplates
                                       .SelectMany(mt => mt.ItemsForSale.Select(i => i.ItemTemplateKey))
                                       .ToFrozenSet(StringComparer.OrdinalIgnoreCase);

        SellableItemsIndex = JsonContext.MerchantTemplates
                                        .SelectMany(mt => mt.ItemsToBuy)
                                        .ToFrozenSet(StringComparer.OrdinalIgnoreCase);

        LearnableSkillsIndex = JsonContext.MerchantTemplates
                                          .SelectMany(mt => mt.SkillsToTeach)
                                          .ToFrozenSet(StringComparer.OrdinalIgnoreCase);

        LearnableSpellsIndex = JsonContext.MerchantTemplates
                                          .SelectMany(mt => mt.SpellsToTeach)
                                          .ToFrozenSet(StringComparer.OrdinalIgnoreCase);

        InUseMapTemplateIndex = JsonContext.MapInstances
                                           .Select(mi => mi.Instance.TemplateKey)
                                           .ToFrozenSet(StringComparer.OrdinalIgnoreCase);
    }
    #endregion

    #region MapInstances
    private async Task DetectMapInstanceViolationsAsync()
    {
        await Task.Yield();

        foreach (var wrapper in JsonContext.MapInstances.Objects)
        {
            var composite = wrapper.Object;
            var mapInstance = composite.Instance;
            var merchantSpawns = composite.Merchants;
            var monsterSpawns = composite.Monsters;
            var reactors = composite.Reactors;
            var expectedInstanceId = GetExpectedKey(wrapper);

            var instancePath = Path.Combine(wrapper.Path, "instance.json");
            var merchantSpawnsPath = Path.Combine(wrapper.Path, "merchants.json");
            var monsterSpawnsPath = Path.Combine(wrapper.Path, "monsters.json");
            var reactorsPath = Path.Combine(wrapper.Path, "reactors.json");

            var handler = new RoutedEventHandler(
                (_, _) =>
                {
                    var info = new ProcessStartInfo(instancePath)
                    {
                        UseShellExecute = true
                    };

                    Process.Start(info);
                });

            if (!mapInstance.InstanceId.EqualsI(expectedInstanceId))
                await AddViolationAsync($"InstanceId mismatch: {mapInstance.InstanceId} != {expectedInstanceId}", handler, true);

            await DetectMapInstance_InstanceViolationsAsync(instancePath, mapInstance);
            await DetectMapInstance_MerchantSpawnViolationsAsync(merchantSpawnsPath, composite, merchantSpawns);
            await DetectMapInstance_MonsterSpawnViolationsAsync(monsterSpawnsPath, composite, monsterSpawns);
            await DetectMapInstance_ReactorViolationsAsync(reactorsPath, composite, reactors);
        }
    }

    private async Task DetectMapInstance_ReactorViolationsAsync(
        string path,
        MapInstanceRepository.MapInstanceComposite composite,
        List<ReactorTileSchema> reactors)
    {
        var handler = new RoutedEventHandler(
            (_, _) =>
            {
                var info = new ProcessStartInfo(path)
                {
                    UseShellExecute = true
                };

                Process.Start(info);
            });

        if (!MapTemplateIndex.TryGetValue(composite.Instance.TemplateKey, out var template))
            return;

        var templateBounds = new Rectangle(
            0,
            0,
            template.Width,
            template.Height);

        foreach (var reactor in reactors)
        {
            if (!templateBounds.Contains(reactor.Source))
                await AddViolationAsync($"Reactor out of bounds: {reactor.Source}", handler);

            if (reactor.OwnerMonsterTemplateKey is not null && !MonsterTemplateIndex.ContainsKey(reactor.OwnerMonsterTemplateKey))
                await AddViolationAsync($"OwnerMonsterTemplateKey not found: {reactor.OwnerMonsterTemplateKey}", handler);
        }
    }

    private async Task DetectMapInstance_InstanceViolationsAsync(string path, MapInstanceSchema mapInstance)
    {
        var handler = new RoutedEventHandler(
            (_, _) =>
            {
                var info = new ProcessStartInfo(path)
                {
                    UseShellExecute = true
                };

                Process.Start(info);
            });

        if (!MapTemplateIndex.ContainsKey(mapInstance.TemplateKey))
            await AddViolationAsync($"TemplateKey not found: {mapInstance.TemplateKey}", handler, true);

        if (mapInstance is { MinimumLevel: not null, MaximumLevel: not null } && (mapInstance.MinimumLevel > mapInstance.MaximumLevel))
            await AddViolationAsync("MinimumLevel > MaximumLevel", handler);

        var shardingOptions = mapInstance.ShardingOptions;

        if (shardingOptions is null)
            return;

        if (shardingOptions.ShardingType == ShardingType.None)
            await AddViolationAsync("ShardingType is None, remove sharding options", handler);
        else
        {
            if (shardingOptions.Limit <= 0)
                await AddViolationAsync("Invalid sharding limit", handler);

            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (shardingOptions.ExitLocation is null)
                await AddViolationAsync("Invalid sharding exit location", handler);
            else
            {
                if (!MapInstanceIndex.TryGetValue(shardingOptions.ExitLocation.Map, out var mi))
                    await AddViolationAsync($"Exit location mapInstance not found: {shardingOptions.ExitLocation.Map}", handler);
                else if (!MapTemplateIndex.TryGetValue(mi.Instance.TemplateKey, out var mt))

                    // ReSharper disable once RedundantJumpStatement
                    return;
                else if (!new Rectangle(
                             0,
                             0,
                             mt.Width,
                             mt.Height).Contains(shardingOptions.ExitLocation))
                    await AddViolationAsync($"Exit location out of bounds: {shardingOptions.ExitLocation}", handler);
            }
        }
    }

    private async Task DetectMapInstance_MerchantSpawnViolationsAsync(
        string path,
        MapInstanceRepository.MapInstanceComposite composite,
        IEnumerable<MerchantSpawnSchema> merchantSpawns)
    {
        var handler = new RoutedEventHandler(
            (_, _) =>
            {
                var info = new ProcessStartInfo(path)
                {
                    UseShellExecute = true
                };

                Process.Start(info);
            });

        foreach (var merchantSpawn in merchantSpawns)
        {
            if (!MerchantTemplateIndex.ContainsKey(merchantSpawn.MerchantTemplateKey))
                await AddViolationAsync($"MerchantTemplateKey not found: {merchantSpawn.MerchantTemplateKey}", handler);

            if (MapTemplateIndex.TryGetValue(composite.Instance.TemplateKey, out var template))
            {
                var templateBounds = new Rectangle(
                    0,
                    0,
                    template.Width,
                    template.Height);

                if (!templateBounds.Contains(merchantSpawn.SpawnPoint))
                    await AddViolationAsync("Merchant spawn point out of bounds", handler);

                if (merchantSpawn.PathingBounds is not null && !templateBounds.Contains(merchantSpawn.PathingBounds))
                    await AddViolationAsync("Merchant pathing bounds out of bounds", handler);

                if (merchantSpawn.BlackList.Any(pt => !templateBounds.Contains(pt)))
                    await AddViolationAsync("Merchant blacklisted point out of bounds", handler);
            }
        }
    }

    private async Task DetectMapInstance_MonsterSpawnViolationsAsync(
        string path,
        MapInstanceRepository.MapInstanceComposite composite,
        IEnumerable<MonsterSpawnSchema> monsterSpawns)
    {
        var handler = new RoutedEventHandler(
            (_, _) =>
            {
                var info = new ProcessStartInfo(path)
                {
                    UseShellExecute = true
                };

                Process.Start(info);
            });

        foreach (var monsterSpawn in monsterSpawns)
        {
            if (!MonsterTemplateIndex.ContainsKey(monsterSpawn.MonsterTemplateKey))
                await AddViolationAsync($"MonsterTemplateKey not found: {monsterSpawn.MonsterTemplateKey}", handler);

            if (MapTemplateIndex.TryGetValue(composite.Instance.TemplateKey, out var template))
            {
                var templateBounds = new Rectangle(
                    0,
                    0,
                    template.Width,
                    template.Height);

                if (monsterSpawn.SpawnArea is not null && !templateBounds.Contains(monsterSpawn.SpawnArea))
                    await AddViolationAsync("Monster spawn area out of bounds", handler);

                if (monsterSpawn.IntervalSecs <= 0)
                    await AddViolationAsync($"Invalid monster spawn interval: {monsterSpawn.IntervalSecs}", handler);

                if (monsterSpawn.MaxAmount <= 0)
                    await AddViolationAsync($"Invalid monster spawn max amount: {monsterSpawn.MaxAmount}", handler);

                if (monsterSpawn.MaxPerSpawn <= 0)
                    await AddViolationAsync($"Invalid monster spawn max per spawn: {monsterSpawn.MaxPerSpawn}", handler);

                if (monsterSpawn.BlackList.Any(pt => !templateBounds.Contains(pt)))
                    await AddViolationAsync("Monster blacklisted point out of bounds", handler);

                foreach (var extraLootTableKey in monsterSpawn.ExtraLootTableKeys)
                    if (!LootTableIndex.ContainsKey(extraLootTableKey))
                        await AddViolationAsync($"ExtraLootTableKey not found: {extraLootTableKey}", handler);
            }
        }
    }
    #endregion
}