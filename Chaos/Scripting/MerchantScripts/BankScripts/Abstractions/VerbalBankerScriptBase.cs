using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Scripting.MerchantScripts.Abstractions;
using Chaos.Utilities;
using Microsoft.Extensions.Logging;

namespace Chaos.Scripting.MerchantScripts.BankScripts.Abstractions;

public abstract class VerbalBankerScriptBase : MerchantScriptBase
{
    protected static ICollection<string> AllTerms { get; } = new List<string>
    {
        "all",
        "everything"
    };

    protected static ICollection<string> DepositPhrases { get; } = new List<string>
    {
        "{Name}, your {Amount} {Thing} is now safe. No problem!",
        "{Name}, stashing away your {Amount} {Thing}. You got it.",
        "{Name}, your {Amount} {Thing} will be in good hands.",
        "{Name}, don't you worry. Securing your {Amount} {Thing}.",
        "{Name}, your {Amount} {Thing} will be safe and sound. Trust me.",
        "{Name}, your {Amount} {Thing} is under lock and key. Done deal.",
        "{Name}, your {Amount} {Thing} is stored away. Consider it done.",
        "{Name}, rest easy. I've tucked away your {Amount} {Thing}.",
        "{Name}, your {Amount} {Thing} is safe with me.",
        "{Name}, no worries. We've secured your {Amount} {Thing}."
    };

    protected static ICollection<string> DontHaveThatManyDepositPhrases { get; } = new List<string>
    {
        "{Name}, it appears you don't have {Amount} {Thing}.",
        "{Name}, {Amount} {Thing} seems to be too much for you.",
        "{Name}, you don't have {Amount} {Thing} on hand.",
        "{Name}, you're missing {Amount} {Thing} for deposit.",
        "{Name}, can't find your {Amount} {Thing} for deposit."
    };

    protected static ICollection<string> DontHaveThatManyWithdrawPhrases { get; } = new List<string>
    {
        "{Name}, it appears you don't have {Amount} {Thing} in your account.",
        "{Name}, {Amount} {Thing} seems to exceed your balance.",
        "{Name}, you don't have {Amount} {Thing} to withdraw.",
        "{Name}, you're missing {Amount} {Thing} for withdrawal.",
        "{Name}, can't find your {Amount} {Thing} for withdrawal."
    };

    protected static ICollection<string> HowManyCoinsPhrases { get; } = new List<string>
    {
        "How many coins do I have",
        "How many coins do I have in my bank",
        "How much money do I have",
        "How much money do I have in my bank",
        "How much money I got",
        "How many coins I got"
    };
    protected ILogger Logger { get; }
    protected static ICollection<string> MoneyTerms { get; } = new List<string>
    {
        "coins",
        "money",
        "cash",
        "gold"
    };

    protected static ICollection<string> WithdrawPhrases { get; } = new List<string>
    {
        "{Name}, your {Amount} {Thing} is returned. Here you go.",
        "{Name}, you now have your {Amount} {Thing} back. All set.",
        "{Name}, your {Amount} {Thing} has been retrieved.",
        "{Name}, handing over your {Amount} {Thing}. You got it.",
        "{Name}, as requested, here's your {Amount} {Thing}.",
        "{Name}, delivering your {Amount} {Thing}.",
        "{Name}, just a moment, getting your {Amount} {Thing}.",
        "{Name}, here comes your {Amount} {Thing}.",
        "{Name}, your {Amount} {Thing} is now with you. There you are.",
        "{Name}, as you wished, here's your {Amount} {Thing}."
    };

    /// <inheritdoc />
    protected VerbalBankerScriptBase(Merchant subject, ILogger logger)
        : base(subject) => Logger = logger;

    protected virtual bool IsClosestVerbalBankerTo(Aisling aisling)
    {
        //if we're checking, it means we're in range... it should be impossible to get no results
        var closestVerbalBanker = aisling.MapInstance.GetEntities<Merchant>()
                                         .Where(merchant => merchant.Script.Is<VerbalBankerScriptBase>())
                                         .OrderBy(x => x.DistanceFrom(aisling))
                                         .ThenBy(x => x.Creation)
                                         .First();

        if (closestVerbalBanker.Equals(Subject))
            return true;

        return false;
    }

    protected virtual void ReplyToUnknownInput(Aisling aisling) => Subject.Say(DialogString.UnknownInput);
}