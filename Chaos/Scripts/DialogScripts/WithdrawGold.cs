using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Utilities;
using Microsoft.Extensions.Logging;

namespace Chaos.Scripts.DialogScripts;

public class WithdrawGold : DialogScriptBase
{
    private readonly ILogger<WithdrawGold> Logger;

    public WithdrawGold(Dialog subject, ILogger<WithdrawGold> logger)
        : base(subject) => Logger = logger;

    public override void OnDisplaying(Aisling source) => RunOnce(
        () =>
        {
            Subject.Text = $"You have {source.Bank.Gold} banked, how much would you like to withdraw?";
        });

    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (!Subject.MenuArgs.TryGet<int>(0, out var amount))
        {
            Subject.Reply(source, DialogString.UnknownInput.Value);

            return;
        }

        var result = ComplexActionHelper.WithdrawGold(source, amount);

        switch (result)
        {
            case ComplexActionHelper.WithdrawGoldResult.Success:
                Logger.LogDebug(
                    "{Player} withdrew {Amount} gold from the bank using entity {Entity}",
                    source,
                    amount,
                    Subject.SourceEntity);

                Subject.NextDialogKey = Subject.Template.TemplateKey;

                break;
            case ComplexActionHelper.WithdrawGoldResult.TooMuchGold:
                Subject.Reply(source, "You are carrying too much gold.");

                break;
            case ComplexActionHelper.WithdrawGoldResult.NotEnoughGold:
                Subject.Reply(source, "You don't have that much.");

                break;
            case ComplexActionHelper.WithdrawGoldResult.BadInput:
                Subject.Reply(source, DialogString.UnknownInput.Value);

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}