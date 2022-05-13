using AutoMapper;
using Chaos.Factories.Interfaces;
using Chaos.Objects.Panel;

namespace Chaos.Utilities;

public class ItemUtility
{
    private readonly IItemFactory ItemFactory;
    private readonly IMapper Mapper;
    public static ItemUtility Instance { get; private set; } = null!;

    public ItemUtility(IMapper mapper, IItemFactory itemFactory)
    {
        Mapper = mapper;
        ItemFactory = itemFactory;

        if (Instance == null!)
            Instance = this;
    }

    public Item Clone(Item item)
    {
        //create a new item with the same template and scripts
        var newItem = ItemFactory.CreateItem(item.Template.TemplateKey, item.ScriptKeys);
        //maps item properties (excluding unique id, script, and template)
        newItem = Mapper.Map(item, newItem);

        return newItem;
    }
}