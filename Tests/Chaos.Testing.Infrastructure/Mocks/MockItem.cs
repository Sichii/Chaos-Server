#region
using Chaos.Common.Abstractions;
using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
#endregion

namespace Chaos.Testing.Infrastructure.Mocks;

public static class MockItem
{
    public static Item Create(
        string name = "TestItem",
        int count = 1,
        bool stackable = false,
        Action<Item>? setup = null)
    {
        var template = new ItemTemplate
        {
            Name = name,
            TemplateKey = name.ToLowerInvariant(),
            ItemSprite = new ItemSprite(1, 1),
            PanelSprite = 1,
            Color = DisplayColor.Default,
            PantsColor = DisplayColor.Default,
            MaxStacks = stackable ? 100 : 1,
            BuyCost = 0,
            SellValue = 0,
            Category = "test",
            Description = null,
            EquipmentType = null,
            Gender = null,
            Class = null,
            AdvClass = null,
            IsDyeable = false,
            IsModifiable = false,
            NoTrade = false,
            AccountBound = false,
            PreventBanking = false,
            Level = 1,
            AbilityLevel = 0,
            MaxDurability = null,
            Modifiers = null,
            Weight = 1,
            Cooldown = null,
            RequiresMaster = false,
            ScriptKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase),
            ScriptVars = new Dictionary<string, IScriptVars>(StringComparer.OrdinalIgnoreCase)
        };

        var item = new Item(template, MockScriptProvider.Instance.Object);
        item.Count = count;

        setup?.Invoke(item);

        return item;
    }
}