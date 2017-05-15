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
    public enum Direction : byte
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
}
