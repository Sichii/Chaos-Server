using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Utilities;
using Microsoft.Extensions.Logging;

namespace Chaos.Scripting.DialogScripts.BankScripts;

public class DepositGoldScript : DialogScriptBase
{
    private readonly ILogger<DepositGoldScript> Logger;

    /// <inheritdoc />
    public DepositGoldScript(Dialog subject, ILogger<DepositGoldScript> logger)
        : base(subject)
        => Logger = logger;

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source) => Subject.InjectTextParameters(source.Gold);

    /// <inheritdoc />
    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (!TryFetchArgs<int>(out var amount) || (amount <= 0))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        var depositResult = ComplexActionHelper.DepositGold(source, amount);

        switch (depositResult)
        {
            case ComplexActionHelper.DepositGoldResult.Success:
                Logger.WithTopics(Topics.Entities.Aisling, Topics.Entities.Gold, Topics.Actions.Deposit)
                      .WithProperty(Subject)
                      .WithProperty(Subject.DialogSource)
                      .WithProperty(source)
                      .LogInformation("Aisling {@AislingName} deposited {Amount} gold in the bank", source.Name, amount);

                break;
            case ComplexActionHelper.DepositGoldResult.DontHaveThatMany:
                Subject.Reply(source, "You don't have enough gold.", "generic_depositgold_initial");

                break;
            case ComplexActionHelper.DepositGoldResult.BadInput:
                Subject.ReplyToUnknownInput(source);

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}