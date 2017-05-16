using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insert_Creative_Name
{
    [Flags]
    internal enum EncryptMethod
    {
        None,
        Normal,
        MD5Key,
    }
    [Flags]
    internal enum WaitEventResult
    {
        Signaled = 0,
        Abandoned = 128,
        Timeout = 258,
    }
    [Flags]
    internal enum Direction : byte
    {
        North = 0,
        East = 1,
        South = 2,
        West = 3,
        Invalid = 255,
    }
    [Flags]
    internal enum ActionPane
    {
        Temuair,
        Medenia,
        Miscellaneous,
    }
    [Flags]
    internal enum Stat
    {
        STR = 1,
        DEX = 2,
        INT = 4,
        WIS = 8,
        CON = 16
    }
    [Flags]
    internal enum UserOption
    {
        Request = 0,
        Whisper = 1,
        Group = 2,
        LShout = 3,
        Wisdom = 4,
        Magic = 5,
        Exchange = 6,
        FastMove = 7,
        GuildChat = 8
    }
}
