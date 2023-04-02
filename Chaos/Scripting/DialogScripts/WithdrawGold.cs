using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Utilities;
using Microsoft.Extensions.Logging;

namespace Chaos.Scripting.DialogScripts;

public class WithdrawGoldScript : DialogScriptBase
{
    private readonly ILogger<WithdrawGoldScript> Logger;

    /// <inheritdoc />
    public WithdrawGoldScript(Dialog subject, ILogger<WithdrawGoldScript> logger)
        : base(subject) =>
        Logger = logger;

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source) => Subject.InjectTextParameters(source.Bank.Gold);

    /// <inheritdoc />
    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (!TryFetchArgs<int>(out var amount) || (amount <= 0))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        var withdrawResult = ComplexActionHelper.WithdrawGold(source, amount);

        switch (withdrawResult)
        {
            case ComplexActionHelper.WithdrawGoldResult.Success:
                Logger.LogDebug(
                    "{@Player} withdrew {GoldAmount} gold from the bank using entity {@Entity}",
                    source,
                    amount,
                    Subject.SourceEntity);

                break;
            case ComplexActionHelper.WithdrawGoldResult.TooMuchGold:
                Subject.Reply(source, "You are carrying too much gold.", "generic_withdrawgold_initial");

                break;
            case ComplexActionHelper.WithdrawGoldResult.NotEnoughGold:
                Subject.Reply(source, "You don't have that much.", "generic_withdrawgold_initial");

                break;
            case ComplexActionHelper.WithdrawGoldResult.BadInput:
                Subject.ReplyToUnknownInput(source);

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}