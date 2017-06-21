using System;
using System.Collections.Generic;

namespace Chaos
{
    [Serializable]
    internal sealed class UserOptions
    {
        internal bool Whisper { get; set; }
        internal bool Group { get; set; }
        internal bool Shout { get; set; }
        internal bool Wisdom { get; set; }
        internal bool Magic { get; set; }
        internal bool Exchange { get; set; }
        internal bool FastMove { get; set; }
        internal bool GuildChat { get; set; }

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

        public string ToString(UserOption opt)
        {
            string format = "{0, 17}, :{0, 3}";
            switch(opt)
            {
                case UserOption.Request:
                    return ToString();
                case UserOption.Whisper:
                    return string.Format(format, "Listen to whisper", Whisper ? "ON" : "OFF");
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

        public override string ToString()
        {
            List<string> req = new List<string>();
            for (int i = 1; i <= 8; i++)
                req.Add(ToString((UserOption)i));

            return string.Join("\t", req);
        }
    }
}
