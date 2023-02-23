using Chaos.Data;
using Chaos.Extensions.Common;
using Chaos.Objects.Menu;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.TypeMapper.Abstractions;
using Chaos.Utilities;
using Microsoft.Extensions.Logging;

namespace Chaos.Scripting.DialogScripts;

public class BuyShopScript : ConfigurableDialogScriptBase
{
    private readonly InputCollector InputCollector;
    private readonly ICloningService<Item> ItemCloner;
    private readonly IItemFactory ItemFactory;
    private readonly ILogger<BuyShopScript> Logger;
    private int? Amount;
    private ItemDetails? ItemDetails;
    private int? TotalBuyCost;
    protected HashSet<string> ItemTemplateKeys { get; init; } = null!;
    private Item? FauxItem => ItemDetails?.Item;

    /// <inheritdoc />
    public BuyShopScript(
        Dialog subject,
        IItemFactory itemFactory,
        ILogger<BuyShopScript> logger,
        ICloningService<Item> itemCloner
    )
        : base(subject)
    {
        Logger = logger;
        ItemCloner = itemCloner;
        ItemFactory = itemFactory;

        var requestInputText = DialogString.From(() => $"How many {FauxItem!.DisplayName} would you like to buy?");

        var requestOptionText = DialogString.From(
            () => $"I can sell {FauxItem!.ToAmountString(Amount!.Value)} for {TotalBuyCost} gold. Is that okay?");

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

        var result = ComplexActionHelper.BuyItem(
            aisling,
            FauxItem!,
            ItemFactory,
            ItemCloner,
            Amount!.Value,
            ItemDetails!.AmountOrPrice);

        switch (result)
        {
            case ComplexActionHelper.BuyItemResult.Success:
                Logger.LogDebug(
                    "{@Player} bought {ItemCount} {@Item} from {@Merchant} for {GoldAmount} gold",
                    aisling,
                    Amount!.Value,
                    FauxItem,
                    Subject.SourceEntity,
                    TotalBuyCost);

                return true;
            case ComplexActionHelper.BuyItemResult.CantCarry:
                Subject.Reply(aisling, "You can't carry that many");

                return false;
            case ComplexActionHelper.BuyItemResult.NotEnoughGold:
                Subject.Reply(aisling, $"You don't have enough gold, you need {TotalBuyCost}");

                return false;
            case ComplexActionHelper.BuyItemResult.BadInput:
                Subject.Reply(aisling, DialogString.UnknownInput.Value);

                return false;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private bool HandleTextInput(Aisling aisling, Dialog dialog, int? optionIndex = null)
    {
        if (!Subject.MenuArgs.TryGet<int>(1, out var amount) || (amount <= 0))
        {
            dialog.Reply(aisling, DialogString.UnknownInput.Value);

            return false;
        }

        FauxItem!.Count = amount;
        Amount = amount;
        TotalBuyCost = ItemDetails!.AmountOrPrice * Amount;

        return true;
    }

    /// <inheritdoc />
    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (ItemDetails == null)
        {
            if (!Subject.MenuArgs.TryGet<string>(0, out var itemName))
            {
                Subject.Reply(source, DialogString.UnknownInput.Value);

                return;
            }

            ItemDetails = Subject.Items.FirstOrDefault(details => details.Item.DisplayName.EqualsI(itemName));

            if (ItemDetails == null)
            {
                Subject.Reply(source, DialogString.UnknownInput.Value);

                return;
            }
        }

        InputCollector.Collect(source, Subject, optionIndex);
    }
}