using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Chaos
{
    internal sealed class UserOptions
    {
        [JsonProperty]
        internal bool Whisper { get; set; }
        [JsonProperty]
        internal bool Group { get; set; }
        [JsonProperty]
        internal bool Shout { get; set; }
        [JsonProperty]
        internal bool Wisdom { get; set; }
        [JsonProperty]
        internal bool Magic { get; set; }
        [JsonProperty]
        internal bool Exchange { get; set; }
        [JsonProperty]
        internal bool FastMove { get; set; }
        [JsonProperty]
        internal bool GuildChat { get; set; }

        /// <summary>
        /// Object representing the user's options page.
        /// </summary>
        internal UserOptions()
        {
            Whisper = true;
            Group = true;
            Shout = true;
            Wisdom = true;
            Magic = true;
            Exchange = false;
            FastMove = false;
            GuildChat = true;
        }

        [JsonConstructor]
        internal UserOptions(bool whisper, bool group, bool shout, bool wisdom, bool magic, bool exchange, bool fastmove, bool guildchat)
        {
            Whisper = whisper;
            Group = group;
            Shout = shout;
            Wisdom = wisdom;
            Magic = magic;
            Exchange = exchange;
            FastMove = fastmove;
            GuildChat = guildchat;
        }

        /// <summary>
        /// Toggles the given UserOption.
        /// </summary>
        /// <param name="opt">Option to toggle.</param>
        internal void Toggle(UserOption opt)
        {
            switch(opt)
            {
                case UserOption.Whisper:
                    Whisper = !Whisper;
                    break;
                case UserOption.Group:
                    Group = !Group;
                    break;
                case UserOption.Shout:
                    Shout = !Shout;
                    break;
                case UserOption.Wisdom:
                    Wisdom = !Wisdom;
                    break;
                case UserOption.Magic:
                    Magic = !Magic;
                    break;
                case UserOption.Exchange:
                    Exchange = !Exchange;
                    break;
                case UserOption.FastMove:
                    FastMove = !FastMove;
                    break;
                case UserOption.GuildChat:
                    GuildChat = !GuildChat;
                    break;
            }
        }

        /// <summary>
        /// Returns string representation of the given UserOption ready for ingame use.
        /// </summary>
        /// <param name="opt">UserOption to convert.</param>
        public string ToString(UserOption opt)
        {
            string format = "{0,-17}:{1,3}";
            switch(opt)
            {
                case UserOption.Request:
                    return ToString();
                case UserOption.Whisper:
                    return string.Format(format, "Listen to whisper", (Whisper ? "ON" : "OFF"));
                case UserOption.Group:
                    return string.Format(format, "Join a group", Group ? "ON" : "OFF");
                case UserOption.Shout:
                    return string.Format(format, "Listen to shout", Shout ? "ON" : "OFF");
                case UserOption.Wisdom:
                    return string.Format(format, "Believe in wisdom", Wisdom ? "ON" : "OFF");
                case UserOption.Magic:
                    return string.Format(format, "Believe in magic", Magic ? "ON" : "OFF");
                case UserOption.Exchange:
                    return string.Format(format, "Exchange", Exchange ? "ON" : "OFF");
                case UserOption.FastMove:
                    return string.Format(format, "Fast Move", FastMove ? "ON" : "OFF");
                case UserOption.GuildChat:
                    return string.Format(format, "Guild Chat", GuildChat ? "ON" : "OFF");
            }

            return string.Empty;
        }

        /// <summary>
        /// Returns string representation of the entire UserOptions page, ready for ingame use.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            List<string> req = new List<string>();
            for (int i = 1; i <= 8; i++)
                req.Add(ToString((UserOption)i));

            return $"\t{string.Join("\t", req)}";
        }
    }
}
