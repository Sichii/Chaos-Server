using Chaos.Data;
using Chaos.Extensions.Common;
using Chaos.Objects.Menu;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Utilities;
using Microsoft.Extensions.Logging;

namespace Chaos.Scripts.DialogScripts;

public class WithdrawItemScript : DialogScriptBase
{
    private readonly InputCollector InputCollector;
    private readonly ILogger<WithdrawItemScript> Logger;
    private int? Amount;
    private Item? Item;

    /// <inheritdoc />
    public WithdrawItemScript(Dialog subject, ILogger<WithdrawItemScript> logger)
        : base(subject)
    {
        Logger = logger;

        var requestInputText = DialogString.From(() => $"How many {Item!.DisplayName} would you like to withdraw?");

        InputCollector = new InputCollectorBuilder()
                         .RequestTextInput(requestInputText)
                         .HandleInput(HandleTextInput)
                         .Build();
    }

    private bool HandleTextInput(Aisling source, Dialog dialog, int? optionIndex = null)
    {
        if (!Subject.MenuArgs.TryGet<int>(1, out var amount))
        {
            dialog.Reply(source, DialogString.UnknownInput.Value);

            return false;
        }

        Amount = amount;

        var result = ComplexActionHelper.WithdrawItem(source, Item!.DisplayName, amount);

        return HandleWithdrawItemResult(source, result);
    }

    private bool HandleWithdrawItemResult(Aisling source, ComplexActionHelper.WithdrawItemResult result)
    {
        switch (result)
        {
            case ComplexActionHelper.WithdrawItemResult.Success:
                Logger.LogDebug(
                    "{Player} withdrew {Item} from the bank using entity {Entity}",
                    source,
                    Item!.ToAmountString(Amount!.Value),
                    Subject.SourceEntity);

                Subject.NextDialogKey = Subject.Template.TemplateKey;

                return true;
            case ComplexActionHelper.WithdrawItemResult.CantCarry:
                Subject.Reply(source, "You can't carry that");

                return false;
            case ComplexActionHelper.WithdrawItemResult.DontHaveThatMany:
                Subject.Reply(source, "You don't have that many");

                return false;
            case ComplexActionHelper.WithdrawItemResult.BadInput:
                Subject.Reply(source, DialogString.UnknownInput.Value);

                return false;
            default:
                throw new ArgumentOutOfRangeException(nameof(result), result, null);
        }
    }

    public override void OnDisplaying(Aisling source)
    {
        if (!Subject.Items.Any())
            Subject.Items.AddRange(
                source.Bank.Select(
                    x => new ItemDetails
                    {
                        Item = x,
                        AmountOrPrice = x.Count
                    }));
    }

    /// <inheritdoc />
    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (Item == null)
        {
            if (!Subject.MenuArgs.TryGet<string>(0, out var itemName))
            {
                Subject.Reply(source, DialogString.UnknownInput.Value);

                return;
            }

            Item = Subject.Items.Select(i => i.Item).FirstOrDefault(i => i.DisplayName.EqualsI(itemName));

            if (Item == null)
            {
                Subject.Reply(source, DialogString.UnknownInput.Value);

                return;
            }
        }

        if (source.Bank.CountOf(Item!.DisplayName) == 1)
        {
            var result = ComplexActionHelper.WithdrawItem(source, Item.DisplayName, 1);
            HandleWithdrawItemResult(source, result);

            return;
        }

        InputCollector.Collect(source, Subject, optionIndex);
    }
}