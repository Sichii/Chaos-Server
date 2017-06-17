using System;

namespace Chaos
{
    [Serializable]
    internal sealed class Personal
    {
        internal byte[] Portrait { get; set; }
        internal string Message { get; set; }

        internal Personal(byte[] data, string msg)
        {
            Portrait = data;
            Message = msg;
        }
    }
}
