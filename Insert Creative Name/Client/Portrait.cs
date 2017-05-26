using System;

namespace Insert_Creative_Name
{
    [Serializable]
    internal sealed class Portrait
    {
        internal byte[] Data { get; }

        internal Portrait(byte[] data)
        {
            Data = data;
        }
    }
}
