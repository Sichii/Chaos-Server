#region
using Chaos.Collections.Common;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;
#endregion

namespace Chaos.Utilities;

public static class Helpers
{
    /// <summary>
    ///     Chunks a message so that it fits within the maximum message length. Ignores color codes as they do not contribute
    ///     to line length
    /// </summary>
    public static List<string> ChunkMessage(string message)
    {
        var chunks = new List<string>();
        var length = message.Length;
        var messageSpan = message.AsSpan();
        Span<char> buffer = stackalloc char[length];
        var charCount = 0;
        var bufferIndex = 0;

        for (var i = 0; i < length; i++)
        {
            if ((i + 3) < length)
            {
                var nextThree = messageSpan.Slice(i, 3);

                if (RegexCache.MessageColorRegex.IsMatch(nextThree))
                {
                    nextThree.CopyTo(buffer.Slice(bufferIndex, 3));
                    i += 2;
                    bufferIndex += 3;

                    continue;
                }
            }

            buffer[bufferIndex++] = messageSpan[i];
            charCount++;

            if (charCount >= CONSTANTS.MAX_MESSAGE_LINE_LENGTH)
            {
                chunks.Add(new string(buffer[..bufferIndex]));
                bufferIndex = 0;
                charCount = 0;
            }
        }

        if (bufferIndex > 0)
            chunks.Add(new string(buffer[..bufferIndex]));

        return chunks;
    }

    public static void DefaultChannelMessageHandler(IChannelSubscriber subscriber, string message)
    {
        var aisling = (Aisling)subscriber;

        aisling.Client.SendDisplayPublicMessage(uint.MaxValue, PublicMessageType.Shout, message);
        var orangeBarChunk = ChunkMessage(message)[0];
        orangeBarChunk = orangeBarChunk[..^3] + "...";
        aisling.SendServerMessage(ServerMessageType.OrangeBar1, orangeBarChunk);
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