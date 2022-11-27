using Chaos.Data;
using Chaos.Extensions.Common;
using Chaos.Factories.Abstractions;
using Chaos.Objects.Menu;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Utilities;
using Microsoft.Extensions.Logging;

namespace Chaos.Scripts.DialogScripts;

public class BuyShopScript : ConfigurableDialogScriptBase
{
    private readonly InputCollector InputCollector;
    private readonly IItemFactory ItemFactory;
    private readonly ILogger<BuyShopScript> Logger;
    private int? Amount;
    private ItemDetails? ItemDetails;
    private Item? ShopItem;
    private int? TotalBuyCost;
    protected HashSet<string> ItemTemplateKeys { get; init; } = null!;

    /// <inheritdoc />
    public BuyShopScript(Dialog subject, IItemFactory itemFactory, ILogger<BuyShopScript> logger)
        : base(subject)
    {
        Logger = logger;
        ItemFactory = itemFactory;
        var requestInputText = DialogString.From(() => $"How many {ShopItem!.DisplayName} would you like to buy?");

        var requestOptionText = DialogString.From(
            () => $"I can sell {ShopItem!.ToAmountString(Amount!.Value)} for {TotalBuyCost} gold. Is that okay?");

        InputCollector = new InputCollectorBuilder()
                         .RequestTextInput(requestInputText)
                         .HandleInput(HandleTextInput)
                         .RequestOptionSelection(requestOptionText, DialogString.Yes, DialogString.No)
                         .HandleInput(HandleOption)
                         .Build();

        foreach (var itemTemplateKey in ItemTemplateKeys)
        {
            var item = ItemFactory.CreateFaux(itemTemplateKey);
            Subject.Items.Add(ItemDetails.Default(item));
        }
    }

    private bool HandleOption(Aisling aisling, Dialog dialog, int? option = null)
    {
        if (option is not 1)
            return false;

        if (!aisling.TryBuyItems(TotalBuyCost!.Value, ShopItem!))
            return false;

        Logger.LogDebug(
            "{PlayerName} bought {AmountOfItemName} from {MerchantName} for {GoldAmount} gold",
            aisling.Name,
            ShopItem!.ToAmountString(Amount!.Value),
            Subject.SourceEntity,
            TotalBuyCost);

        return true;
    }

    private bool HandleTextInput(Aisling aisling, Dialog dialog, int? option = null)
    {
        if (!Subject.MenuArgs.TryGet<int>(1, out var amount) || (amount <= 0))
        {
            dialog.Reply(aisling, DialogString.UnknownInput.Value);

            return false;
        }

        ShopItem!.Count = amount;
        Amount = amount;
        TotalBuyCost = ItemDetails!.AmountOrPrice * Amount;

        return true;
    }

    /// <inheritdoc />
    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (ShopItem == null)
        {
            if (!Subject.MenuArgs.Any())
                return;

            var itemName = Subject.MenuArgs.First();
            ItemDetails = Subject.Items.FirstOrDefault(details => details.Item.DisplayName.EqualsI(itemName));
            ShopItem = ItemDetails?.Item;

            if (ShopItem == null)
                return;
        }

        InputCollector.Collect(source, Subject, optionIndex);
    }
}