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
    [JsonObject(MemberSerialization.OptOut)]
    internal struct Animation
    {
        internal int TargetId { get; }
        internal int SourceId { get; }
        internal ushort TargetAnimation { get; }
        internal ushort SourceAnimation { get; }
        internal ushort AnimationSpeed { get; }

        internal Animation(int targetId, int sourceId, ushort targetAnimation, ushort sourceAnimation, ushort speed)
        {
            TargetId = targetId;
            SourceId = sourceId;
            TargetAnimation = targetAnimation;
            SourceAnimation = sourceAnimation;
            AnimationSpeed = speed;
        }

        internal Animation(ushort targetAnimation, ushort sourceAnimation, ushort speed)
            : this(0, 0, targetAnimation, sourceAnimation, speed)
        {

        }

        internal Animation(Animation animation, int targetId, int sourceId)
            : this(targetId, sourceId, animation.TargetAnimation, animation.SourceAnimation, animation.AnimationSpeed)
        {

        }

        internal static Animation None => default(Animation);
    }
}
