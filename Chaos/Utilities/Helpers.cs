using Chaos.Collections.Common;
using Chaos.Common.Definitions;
using Chaos.Extensions.Common;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;

namespace Chaos.Utilities;

public static class Helpers
{
    public static void DefaultChannelMessageHandler(IChannelSubscriber subscriber, string message)
    {
        var aisling = (Aisling)subscriber;
        aisling.SendServerMessage(ServerMessageType.ActiveMessage, message);
        aisling.Client.SendPublicMessage(uint.MaxValue, PublicMessageType.Shout, message);
    }

    public static void HandleApproach(Creature creature1, Creature creature2)
    {
        if (creature1.Equals(creature2))
            return;

        if (creature2.CanObserve(creature1))
            creature2.OnApproached(creature1);

        if (creature1.CanObserve(creature2))
            creature1.OnApproached(creature2);
    }

    public static void HandleDeparture(Creature creature1, Creature creature2)
    {
        if (creature1.Equals(creature2))
            return;

        if (creature2.CanObserve(creature1))
            creature2.OnDeparture(creature1);

        if (creature1.CanObserve(creature2))
            creature1.OnDeparture(creature2);
    }

    public static bool TryGetMessageColor(ArgumentCollection args, [NotNullWhen(true)] out MessageColor? messageColor)
    {
        messageColor = null;

        //attempts to parse a message color through various means
        //first try directly parsing the enum name
        if (!args.TryGetNext(out MessageColor msgColor))
        {
            //next, try parsing a full color prefix or color code
            if (!args.TryGetNext<string>(out var colorPrefix))
                return false;

            switch (colorPrefix.Length)
            {
                case 3 when colorPrefix.StartsWithI("{="):
                    msgColor = (MessageColor)(byte)char.ToLower(colorPrefix[2]);

                    break;
                case 1 when char.IsLetter(colorPrefix[0]):
                    msgColor = (MessageColor)(byte)char.ToLower(colorPrefix[0]);

                    break;
                default:
                    return false;
            }
        }

        messageColor = msgColor;

        return true;
    }
}