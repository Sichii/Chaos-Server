using Chaos.Extensions.Common;
using Chaos.Objects.Menu;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Utilities;
using Microsoft.Extensions.Logging;

namespace Chaos.Scripts.DialogScripts;

public class DepositItemScript : DialogScriptBase
{
    private readonly ILogger<DepositItemScript> Logger;
    private int? Amount { get; set; }
    private Item? Item { get; set; }
    public InputCollector InputCollector { get; }

    public DepositItemScript(Dialog subject, ILogger<DepositItemScript> logger)
        : base(subject)
    {
        Logger = logger;
        var requestInputText = DialogString.From(() => $"How many {Item!.DisplayName} would you like to deposit?");

        InputCollector = new InputCollectorBuilder()
                         .RequestTextInput(requestInputText)
                         .HandleInput(HandleInputText)
                         .Build();
    }

    private bool HandleDepositItemResult(Aisling source, ComplexActionHelper.DepositItemResult result)
    {
        switch (result)
        {
            case ComplexActionHelper.DepositItemResult.Success:
                Logger.LogDebug(
                    "{Player} deposited {Item} in the bank using entity {Entity}",
                    source,
                    Item!.ToAmountString(Amount!.Value),
                    Subject.SourceEntity);

                Subject.NextDialogKey = Subject.Template.TemplateKey;

                return true;
            case ComplexActionHelper.DepositItemResult.DontHaveThatMany:
                Subject.Reply(source, "You don't have that many");

                return false;
            case ComplexActionHelper.DepositItemResult.NotEnoughGold:
                //Subject.Reply(source, $"You don't have enough gold, you need {}");
                //this script doesnt currently take into account deposit fees

                return false;
            case ComplexActionHelper.DepositItemResult.BadInput:
                Subject.Reply(source, DialogString.UnknownInput.Value);

                return false;
            default:
                throw new ArgumentOutOfRangeException(nameof(result), result, null);
        }
    }

    private bool HandleInputText(Aisling source, Dialog dialog, int? option)
    {
        if (!Subject.MenuArgs.TryGet<int>(1, out var amount))
        {
            Subject.Reply(source, DialogString.UnknownInput.Value);

            return false;
        }

        Amount = amount;
        var result = ComplexActionHelper.DepositItem(source, Item!.Slot, Amount.Value);

        return HandleDepositItemResult(source, result);
    }

    public override void OnDisplaying(Aisling source)
    {
        if (Subject.Slots.IsNullOrEmpty())
            Subject.Slots = source.Inventory.Select(x => x.Slot).ToList();
    }

    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (Item == null)
        {
            if (!Subject.MenuArgs.TryGet<byte>(0, out var slot))
            {
                Subject.Reply(source, DialogString.UnknownInput.Value);

                return;
            }

            var item = source.Inventory[slot];

            if (item == null)
            {
                Subject.Reply(source, DialogString.UnknownInput.Value);

                return;
            }

            Item = item;
        }

        if (source.Inventory.CountOf(Item.DisplayName) == 1)
        {
            var result = ComplexActionHelper.DepositItem(source, Item.Slot, 1);
            Amount = 1;
            HandleDepositItemResult(source, result);
        } else
            InputCollector.Collect(source, Subject, optionIndex);
    }
}