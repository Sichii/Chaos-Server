using Newtonsoft.Json;

namespace Chaos
{
    internal sealed class Personal
    {
        [JsonProperty]
        internal byte[] Portrait { get; set; }
        [JsonProperty]
        internal string Message { get; set; }

        /// <summary>
        /// Object representing the Portrait and Profile Message.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="msg"></param>
        [JsonConstructor]
        internal Personal(byte[] portrait, string message)
        {
            Portrait = portrait;
            Message = message;
        }

    }
}
