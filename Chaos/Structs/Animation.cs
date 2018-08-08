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
    internal struct Animation
    {
        [JsonProperty]
        internal Point TargetPoint { get; }
        [JsonProperty]
        internal int TargetId { get; }
        [JsonProperty]
        internal int SourceId { get; }
        [JsonProperty]
        internal ushort TargetAnimation { get; }
        [JsonProperty]
        internal ushort SourceAnimation { get; }
        [JsonProperty]
        internal ushort AnimationSpeed { get; }

        public static bool operator ==(Animation ani1, Animation ani2) => ani1.Equals(ani2);
        public static bool operator !=(Animation ani1, Animation ani2) => !ani1.Equals(ani2);

        /// <summary>
        /// Master constructor, do not use.
        /// </summary>
        [JsonConstructor]
        internal Animation(Point targetPoint, int targetId, int sourceId, ushort targetAnimation, ushort sourceAnimation, ushort animationSpeed)
        {
            TargetPoint = targetPoint;
            TargetId = targetId;
            SourceId = sourceId;
            TargetAnimation = targetAnimation;
            SourceAnimation = sourceAnimation;
            AnimationSpeed = animationSpeed;
        }

        /// <summary>
        /// Constructor for animations targeting a specific object.
        /// </summary>
        internal Animation(ushort targetAnimation, ushort sourceAnimation, ushort speed)
            : this(Point.None, 0, 0, targetAnimation, sourceAnimation, speed)
        {

        }

        /// <summary>
        /// Constructor for a full animation targeting a specific object, taking a partial animation.
        /// </summary>
        internal Animation(Animation animation, int targetId, int sourceId)
            : this(animation.TargetPoint, targetId, sourceId, animation.TargetAnimation, animation.SourceAnimation, animation.AnimationSpeed)
        {

        }

        /// <summary>
        /// Constructor for an animation targeting a point.
        /// </summary>
        internal Animation(Point targetPoint, ushort targetAnimation, ushort speed)
            : this(targetPoint, 0, 0, targetAnimation, 0, speed)
        {

        }


        internal static Animation None => default(Animation);

        public override bool Equals(object obj)
        {
            if (!(obj is Animation))
                return false;

            Animation ani = (Animation)obj;
            return GetHashCode() == ani.GetHashCode();
        }
        public override int GetHashCode() => ((ushort)(TargetId + TargetAnimation) << 16) + ((ushort)(SourceId + SourceAnimation)) + (AnimationSpeed << 16) + (TargetPoint.GetHashCode());
    }
}
