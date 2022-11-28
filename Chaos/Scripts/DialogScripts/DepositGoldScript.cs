using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Utilities;
using Microsoft.Extensions.Logging;

namespace Chaos.Scripts.DialogScripts;

public class DepositGoldScript : DialogScriptBase
{
    private readonly ILogger<DepositGoldScript> Logger;

    public DepositGoldScript(Dialog subject, ILogger<DepositGoldScript> logger)
        : base(subject) => Logger = logger;

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source) => RunOnce(
        () =>
        {
            Subject.Text = $"You are holding {source.Gold} gold, how much would you like to deposit?";
        });

    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (!Subject.MenuArgs.TryGet<int>(0, out var amount))
        {
            Subject.Reply(source, DialogString.UnknownInput.Value);

            return;
        }

        var result = ComplexActionHelper.DepositGold(source, amount);

        switch (result)
        {
            case ComplexActionHelper.DepositGoldResult.Success:
                Logger.LogDebug(
                    "{Player} deposited {Amount} gold in the bank using entity {Entity}",
                    source,
                    amount,
                    Subject.SourceEntity);

                Subject.NextDialogKey = Subject.Template.TemplateKey;

                break;
            case ComplexActionHelper.DepositGoldResult.NotEnoughGold:
                Subject.Reply(source, "You don't have enough gold.");

                break;
            case ComplexActionHelper.DepositGoldResult.BadInput:
                Subject.Reply(source, DialogString.UnknownInput.Value);

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}