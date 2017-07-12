using System;

namespace Chaos
{
    internal sealed class Personal
    {
        internal byte[] Portrait { get; set; }
        internal string Message { get; set; }

        /// <summary>
        /// Object representing the Portrait and Profile Message.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="msg"></param>
        internal Personal(byte[] data, string msg)
        {
            Portrait = data;
            Message = msg;
        }
    }
}
