using Chaos.Extensions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Utilities;
using Microsoft.Extensions.Logging;

namespace Chaos.Scripting.DialogScripts.BankScripts;

public class DepositItemScript : DialogScriptBase
{
    private readonly ILogger<DepositItemScript> Logger;

    /// <inheritdoc />
    public DepositItemScript(Dialog subject, ILogger<DepositItemScript> logger)
        : base(subject) =>
        Logger = logger;

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "generic_deposititem_initial":
            {
                OnDisplayingInitial(source);

                break;
            }
            case "generic_deposititem_amountrequest":
            {
                OnDisplayingAmountRequest(source);

                break;
            }
        }
    }

    private void OnDisplayingAmountRequest(Aisling source)
    {
        if (!TryFetchArgs<byte>(out var slot) || !source.Inventory.TryGetObject(slot, out var item))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        var total = source.Inventory.CountOf(item.DisplayName);

        Subject.InjectTextParameters(item.DisplayName, total);
    }

    private void OnDisplayingInitial(Aisling source) => Subject.Slots = source.Inventory.Select(obj => obj.Slot).ToList();

    /// <inheritdoc />
    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "generic_deposititem_amountrequest":
            {
                OnNextAmountRequest(source);

                break;
            }
        }
    }

    private void OnNextAmountRequest(Aisling source)
    {
        if (!TryFetchArgs<byte, int>(out var slot, out var amount)
            || (amount <= 0)
            || !source.Inventory.TryGetObject(slot, out var item))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        var depositItemResult = ComplexActionHelper.DepositItem(source, slot, amount);

        switch (depositItemResult)
        {
            case ComplexActionHelper.DepositItemResult.Success:
                Logger.WithProperty(Subject)
                      .WithProperty(Subject.DialogSource)
                      .WithProperty(source)
                      .WithProperty(item)
                      .LogInformation(
                          "Aisling {@AislingName} deposited {Amount} {@ItemName} in the bank",
                          source.Name,
                          amount,
                          item.DisplayName);

                return;
            case ComplexActionHelper.DepositItemResult.DontHaveThatMany:
                Subject.Reply(source, "You don't have that many", "generic_deposititem_initial");

                return;
            case ComplexActionHelper.DepositItemResult.NotEnoughGold:
                //Subject.Reply(source, $"You don't have enough gold, you need {}");
                //this script doesnt currently take into account deposit fees

                return;
            case ComplexActionHelper.DepositItemResult.BadInput:
                Subject.ReplyToUnknownInput(source);

                return;
            default:
                throw new ArgumentOutOfRangeException(nameof(depositItemResult), depositItemResult, null);
        }
    }
}