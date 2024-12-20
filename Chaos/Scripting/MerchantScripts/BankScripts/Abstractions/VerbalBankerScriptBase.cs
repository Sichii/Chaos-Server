#region
using Chaos.Common.Utilities;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Scripting.MerchantScripts.Abstractions;
using Chaos.Utilities;
using Humanizer;
#endregion

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
        "{Name}, your {AmountOfThing} is now safe. No problem!",
        "{Name}, stashing away your {AmountOfThing}. You got it.",
        "{Name}, your {AmountOfThing} will be in good hands.",
        "{Name}, don't you worry. Securing your {AmountOfThing}.",
        "{Name}, your {AmountOfThing} will be safe and sound. Trust me.",
        "{Name}, your {AmountOfThing} is under lock and key. Done deal.",
        "{Name}, your {AmountOfThing} is stored away. Consider it done.",
        "{Name}, rest easy. I've tucked away your {AmountOfThing}.",
        "{Name}, your {AmountOfThing} is safe with me.",
        "{Name}, no worries. We've secured your {AmountOfThing}."
    };

    protected static ICollection<string> DontHaveThatManyDepositPhrases { get; } = new List<string>
    {
        "{Name}, it appears you don't have {AmountOfThing}.",
        "{Name}, {AmountOfThing} seems to be too much for you.",
        "{Name}, you don't have {AmountOfThing} on hand.",
        "{Name}, you're missing {AmountOfThing} for deposit.",
        "{Name}, can't find your {AmountOfThing} for deposit."
    };

    protected static ICollection<string> DontHaveThatManyWithdrawPhrases { get; } = new List<string>
    {
        "{Name}, it appears you don't have {AmountOfThing} in your account.".Humanize(),
        "{Name}, {AmountOfThing} seems to exceed your balance.",
        "{Name}, you don't have {AmountOfThing} to withdraw.",
        "{Name}, you're missing {AmountOfThing} for withdrawal.",
        "{Name}, can't find your {AmountOfThing} for withdrawal."
    };

    protected static ICollection<string> HowMuchGoldPhrases { get; } = new List<string>
    {
        "How much gold do I have",
        "How much gold do I have in my bank",
        "How much money do I have",
        "How much money do I have in my bank",
        "How much money I got",
        "How much gold I got"
    };

    protected static ICollection<string> ItemDamagedDepositPhrases { get; } = new List<string>
    {
        "{Name}, I can't be made responsible for your damaged {AmountOfThing}.",
        "{Name}, your {AmountOfThing} is damaged. Deposit denied.",
        "{Name}, your {AmountOfThing} isn't in depositable condition.",
        "{Name}, fix your {AmountOfThing} before depositing it.",
        "{Name}, I can't take your {AmountOfThing}. Ask a smith to fix it."
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
        "{Name}, your {AmountOfThing} is returned. Here you go.",
        "{Name}, you now have your {AmountOfThing} back. All set.",
        "{Name}, your {AmountOfThing} has been retrieved.",
        "{Name}, handing over your {AmountOfThing}. You got it.",
        "{Name}, as requested, here's your {AmountOfThing}.",
        "{Name}, delivering your {AmountOfThing}.",
        "{Name}, just a moment, getting your {AmountOfThing}.",
        "{Name}, here comes your {AmountOfThing}.",
        "{Name}, your {AmountOfThing} is now with you. There you are.",
        "{Name}, as you wished, here's your {AmountOfThing}."
    };

    /// <inheritdoc />
    protected VerbalBankerScriptBase(Merchant subject, ILogger logger)
        : base(subject)
        => Logger = logger;

    protected virtual string Humanize(
        string template,
        string name,
        int amount,
        string thing)
    {
        if (amount > 1)
            template.ReplaceI("{AmountOfThing} is", "{AmountOfThing} are");
        else
            template.ReplaceI("{AmountOfThing} are", "{AmountOfThing} is");

        return template.Inject(name, thing.ToQuantity(amount));
    }

    protected virtual bool IsClosestVerbalBankerTo(Aisling aisling)
    {
        //if we're checking, it means we're in range... it should be impossible to get no results
        var closestVerbalBanker = aisling.MapInstance
                                         .GetEntities<Merchant>()
                                         .Where(merchant => merchant.Script.Is<VerbalBankerScriptBase>())
                                         .OrderBy(x => x.ManhattanDistanceFrom(aisling))
                                         .ThenBy(x => x.Creation)
                                         .First();

        if (closestVerbalBanker.Equals(Subject))
            return true;

        return false;
    }

    protected virtual void RandomizedReply(
        Merchant source,
        ICollection<string> phrases,
        string name,
        int amount,
        string thing,
        int? goldAmount = null)
    {
        var phrase = phrases.PickRandom();

        phrase = Humanize(
            phrase,
            name,
            amount,
            thing);

        if (goldAmount.HasValue)
            phrase = phrase.Inject(goldAmount.Value);

        source.Say(phrase);
    }

    protected virtual void ReplyToUnknownInput(Aisling aisling) => Subject.Say(DialogString.UnknownInput);
}