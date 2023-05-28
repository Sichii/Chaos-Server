using Chaos.Extensions.Common;
using Chaos.Models.Abstractions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Utilities;
using Microsoft.Extensions.Logging;

namespace Chaos.Scripting.DialogScripts.ShopScripts;

public class SellShopScript : DialogScriptBase
{
    private readonly ILogger<SellShopScript> Logger;
    private readonly ISellShopSource SellShopSource;

    /// <inheritdoc />
    public SellShopScript(Dialog subject, ILogger<SellShopScript> logger)
        : base(subject)
    {
        Logger = logger;
        SellShopSource = (ISellShopSource)subject.DialogSource;
    }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "generic_sellshop_initial":
            {
                OnDisplayingInitial(source);

                break;
            }
            case "generic_sellshop_amountrequest":
            {
                OnDisplayingAmountRequest(source);

                break;
            }
            case "generic_sellshop_confirmation":
            {
                OnDisplayingConfirmation(source);

                break;
            }
            case "generic_sellshop_accepted":
            {
                OnDisplayingAccepted(source);

                break;
            }
        }
    }

    private void OnDisplayingAccepted(Aisling source)
    {
        if (!TryFetchArgs<byte, int>(out var slot, out var amount)
            || (amount <= 0)
            || !source.Inventory.TryGetObject(slot, out var item)
            || !SellShopSource.IsBuying(item))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        var totalSellValue = amount * item.Template.SellValue;

        var sellItemResult = ComplexActionHelper.SellItem(
            source,
            slot,
            amount,
            item.Template.SellValue);

        switch (sellItemResult)
        {
            case ComplexActionHelper.SellItemResult.Success:
                Logger.WithProperties(source, item, SellShopSource)
                      .LogDebug(
                          "Aisling {@AislingName} sold {ItemAmount} {@ItemName} to merchant {@MerchantName} for {GoldAmount} gold",
                          source.Name,
                          amount,
                          item.DisplayName,
                          SellShopSource.Name,
                          totalSellValue);

                return;
            case ComplexActionHelper.SellItemResult.DontHaveThatMany:
                Subject.Reply(source, "You don't have that many.", "generic_sellshop_initial");

                return;
            case ComplexActionHelper.SellItemResult.TooMuchGold:
                Subject.Reply(source, "You are carrying too much gold.", "generic_sellshop_initial");

                return;
            case ComplexActionHelper.SellItemResult.BadInput:
                Subject.ReplyToUnknownInput(source);

                return;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void OnDisplayingAmountRequest(Aisling source)
    {
        if (!TryFetchArgs<byte>(out var slot) || !source.Inventory.TryGetObject(slot, out var item) || !SellShopSource.IsBuying(item))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        var total = source.Inventory.CountOf(item.DisplayName);

        Subject.InjectTextParameters(item.DisplayName, total);
    }

    private void OnDisplayingConfirmation(Aisling source)
    {
        if (!TryFetchArgs<byte, int>(out var slot, out var amount)
            || (amount <= 0)
            || !source.Inventory.TryGetObject(slot, out var item)
            || !SellShopSource.IsBuying(item))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        Subject.InjectTextParameters(amount, item.DisplayName, amount * item.Template.SellValue);
    }

    private void OnDisplayingInitial(Aisling source) =>
        Subject.Slots = source.Inventory
                              .Where(item => SellShopSource.ItemsToBuy.Contains(item.Template.TemplateKey))
                              .Select(item => item.Slot)
                              .ToList();
}