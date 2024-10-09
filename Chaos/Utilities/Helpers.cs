using Chaos.Collections.Common;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions.Common;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;

namespace Chaos.Utilities;

public static class Helpers
{
    public static void DefaultChannelMessageHandler(IChannelSubscriber subscriber, string message)
    {
        var aisling = (Aisling)subscriber;
        aisling.SendServerMessage(ServerMessageType.ActiveMessage, message);
        aisling.Client.SendDisplayPublicMessage(uint.MaxValue, PublicMessageType.Shout, message);
    }

    public static bool TryGetMessageColor(ArgumentCollection args, [NotNullWhen(true)] out MessageColor? messageColor)
    {
        messageColor = null;

        //attempts to parse a message color through various means
        //first try directly parsing the enum name
        if (!args.TryGetNext(out messageColor))
        {
            //next, try parsing a full color prefix or color code
            if (!args.TryGetNext<string>(out var colorPrefix))
                return false;

            switch (colorPrefix.Length)
            {
                case 3 when colorPrefix.StartsWithI("{="):
                    messageColor = (MessageColor)(byte)char.ToLower(colorPrefix[2]);

                    break;
                case 1 when char.IsLetter(colorPrefix[0]):
                    messageColor = (MessageColor)(byte)char.ToLower(colorPrefix[0]);

                    break;
                default:
                    return false;
            }
        }

        return messageColor != null;
    }
}