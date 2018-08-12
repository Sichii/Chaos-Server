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
using System;

namespace Chaos
{
    [JsonObject(MemberSerialization.OptIn)]
    internal struct Animation : IEquatable<Animation>
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
        /// Master constructor for Animation.
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
        /// Constructor for an animation targeting a point.
        /// </summary>
        internal Animation(Point targetPoint, ushort targetAnimation, ushort speed)
            : this(targetPoint, 0, 0, targetAnimation, 0, speed)
        {

        }

        /// <summary>
        /// Returns a re-targeted animation based on IDs.
        /// </summary>
        internal Animation GetTargetedAnimation(int targetId, int sourceId) =>
            new Animation(TargetPoint, targetId, sourceId, TargetAnimation, SourceAnimation, AnimationSpeed);

        /// <summary>
        /// Returns a re-target animation based on a point.
        /// </summary>
        internal Animation GetTargetedAnimation(Point targetPoint) => 
            new Animation(targetPoint, 0, 0, TargetAnimation, SourceAnimation, AnimationSpeed);

        /// <summary>
        /// Static constructor for no animation.
        /// </summary>
        internal static Animation None => default(Animation);

        public override int GetHashCode() => (SourceId << 16) + (TargetAnimation << 8) + TargetPoint.GetHashCode();
        public override bool Equals(object obj)
        {
            if (!(obj is Animation))
                return false;

            Animation ani = (Animation)obj;
            return GetHashCode() == ani.GetHashCode();
        }


        public bool Equals(Animation other) => GetHashCode() == other.GetHashCode();
    }
}
