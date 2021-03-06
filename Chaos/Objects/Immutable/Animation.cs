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
    internal sealed class Animation : IEquatable<Animation>
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
        /// Json & Master constructor for a structure representing a point on a map.
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
        /// Constructor for an animations targeting an object.
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
        /// Returns a re-targeted animation based on IDs. Removes sourceAnimation.
        /// </summary>
        internal Animation GetTargetedEffectAnimation(int targetId, int sourceId) =>
            new Animation(TargetPoint, targetId, sourceId, TargetAnimation, 0, AnimationSpeed);

        /// <summary>
        /// Returns a re-target animation based on a point. Removes sourceAnimation.
        /// </summary>
        internal Animation GetTargetedEffectAnimation(Point targetPoint) =>
            new Animation(targetPoint, 0, 0, TargetAnimation, 0, AnimationSpeed);

        /// <summary>
        /// Returns a re-targeted animation based on IDs. (Does not remove sourceAnimation)
        /// </summary>
        internal Animation GetTargetedAnimation(int targetId, int sourceId) =>
            new Animation(TargetPoint, targetId, sourceId, TargetAnimation, SourceAnimation, AnimationSpeed);

        /// <summary>
        /// Returns a re-target animation based on a point. (Does not remove sourceAnimation)
        /// </summary>
        internal Animation GetTargetedAnimation(Point targetPoint) => 
            new Animation(targetPoint, 0, 0, TargetAnimation, SourceAnimation, AnimationSpeed);

        /// <summary>
        /// Static constructor for no animation.
        /// </summary>
        internal static Animation None => new Animation(0, 0, 0);

        public override int GetHashCode() => (SourceId << 16) + (TargetAnimation << 8) + TargetPoint.GetHashCode();
        public override bool Equals(object obj) => (obj is Animation ani) ? Equals(ani) : false;
        public bool Equals(Animation other) => !(other is null) && GetHashCode() == other.GetHashCode();
        public override string ToString() => (TargetPoint != Point.None) ? $@"TARGET_POINT: {TargetPoint} | TARGET_ANIMATION: {TargetAnimation}" : $@"SOURCE_ID: {SourceId} | SOURCE_ANIMATION: {SourceAnimation} | TARGET_ID: {TargetId} | TARGET_ANIMATION: {TargetAnimation}";
    }
}
