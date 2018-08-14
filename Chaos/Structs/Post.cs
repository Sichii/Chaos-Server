// ****************************************************************************
// This file belongs to the Chaos-Server project.
// 
// This project is free and open-source, provided that any alterations or
// modifications to any portions of this project adhere to the
// Affero General Public License (Version 3).
// 
// A copy of the AGPLv3 can be found in the project directory.
// You may also find a copy at <https://www.gnu.org/licenses/agpl-3.0.html>
// ****************************************************************************

using Newtonsoft.Json;

namespace Chaos
{
    [JsonObject(MemberSerialization.OptIn)]
    internal struct Post
    {
        [JsonProperty]
        internal ushort PostId;
        [JsonProperty]
        internal string Subject;
        [JsonProperty]
        internal string Body;

        internal Post(ushort postId, string subject, string body)
        {
            PostId = postId;
            Subject = subject;
            Body = body;
        }
    }
}
