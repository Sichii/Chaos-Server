using Chaos.Data;
using Chaos.Objects.Menu;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Utilities;
using Microsoft.Extensions.Logging;

namespace Chaos.Scripting.DialogScripts;

public class WithdrawItemScript : DialogScriptBase
{
    private readonly ILogger<WithdrawItemScript> Logger;

    /// <inheritdoc />
    public WithdrawItemScript(Dialog subject, ILogger<WithdrawItemScript> logger)
        : base(subject) =>
        Logger = logger;

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "generic_withdrawitem_initial":
            {
                OnDisplayingInitial(source);

                break;
            }
            case "generic_withdrawitem_amountrequest":
            {
                OnDisplayingAmountRequest(source);

                break;
            }
        }
    }

    private void OnDisplayingAmountRequest(Aisling source)
    {
        if (!TryFetchArgs<string>(out var itemName) || !TryGetItem(source, itemName, out var item))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        Subject.InjectTextParameters(item.DisplayName, item.Count);
    }

    private void OnDisplayingInitial(Aisling source) => Subject.Items.AddRange(source.Bank.Select(ItemDetails.WithdrawItem));

    /// <inheritdoc />
    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "generic_withdrawitem_amountrequest":
            {
                OnNextAmountRequest(source);

                break;
            }
        }
    }

    private void OnNextAmountRequest(Aisling source)
    {
        if (!TryFetchArgs<string, int>(out var itemName, out var amount)
            || (amount <= 0)
            || !TryGetItem(source, itemName, out var item))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        var withdrawResult = ComplexActionHelper.WithdrawItem(source, item.DisplayName, amount);

        switch (withdrawResult)
        {
            case ComplexActionHelper.WithdrawItemResult.Success:
                Logger.LogDebug(
                    "{@Player} withdrew {ItemCount} {@Item} from the bank using entity {@Entity}",
                    source,
                    amount,
                    item,
                    Subject.SourceEntity);

                return;
            case ComplexActionHelper.WithdrawItemResult.CantCarry:
                Subject.Reply(source, "You can't carry that");

                return;
            case ComplexActionHelper.WithdrawItemResult.DontHaveThatMany:
                Subject.Reply(source, "You don't have that many");

                return;
            case ComplexActionHelper.WithdrawItemResult.BadInput:
                Subject.Reply(source, DialogString.UnknownInput.Value);

                return;
            default:
                throw new ArgumentOutOfRangeException(nameof(withdrawResult), withdrawResult, null);
        }
    }

    private bool TryGetItem(Aisling source, string itemName, [MaybeNullWhen(false)] out Item item)
    {
        item = source.Bank.FirstOrDefault(obj => obj.DisplayName.Equals(itemName, StringComparison.OrdinalIgnoreCase));

        return item != null;
    }
}