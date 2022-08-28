using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Entities.Schemas.Templates;
using Chaos.Services.Mappers.Interfaces;
using Chaos.Templates.Abstractions;

namespace Chaos.Templates;

public class ItemTemplate : PanelObjectTemplateBase
{
    public bool AccountBound { get; init; }
    public AdvClass? AdvClass { get; init; }
    public BaseClass? BaseClass { get; init; }
    public DisplayColor Color { get; init; }
    public EquipmentType? EquipmentType { get; init; }
    public Gender? Gender { get; init; }
    public ItemSprite ItemSprite { get; init; }
    public int? MaxDurability { get; init; }
    public int MaxStacks { get; init; }
    public Attributes? Modifiers { get; init; }
    public override ushort PanelSprite => ItemSprite.PanelSprite;
    public override string TemplateKey { get; init; }
    public int Value { get; init; }
    public byte Weight { get; init; }
    public bool Stackable => MaxStacks > 1;

    public ItemTemplate(ItemTemplateSchema schema, ITypeMapper mapper)
        : base(schema, mapper)
    {
        AccountBound = schema.AccountBound;
        BaseClass = schema.BaseClass;
        AdvClass = schema.AdvClass;
        Color = schema.Color;
        EquipmentType = schema.EquipmentType;
        Gender = schema.Gender;
        ItemSprite = new ItemSprite(schema.PanelSprite, schema.DisplaySprite ?? 0);
        MaxDurability = schema.MaxDurability;
        MaxStacks = schema.MaxStacks;

        if (schema.Modifiers != null)
            Modifiers = mapper.Map<Attributes>(schema.Modifiers);

        // ReSharper disable once VirtualMemberCallInConstructor
        TemplateKey = schema.TemplateKey;
        Value = schema.Value;
        Weight = schema.Weight;
    }
}