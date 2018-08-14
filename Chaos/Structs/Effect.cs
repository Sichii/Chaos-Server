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
    internal struct Effect : IEquatable<Effect>
    {
        internal DateTime StartTime;
        internal EffectsBarColor CurrentColor;

        [JsonProperty]
        internal ushort Sprite; //will be set by the spell itself
        [JsonProperty]
        internal bool UseParentAnimation;
        [JsonProperty]
        internal sbyte StrMod;
        [JsonProperty]
        internal sbyte IntMod;
        [JsonProperty]
        internal sbyte WisMod;
        [JsonProperty]
        internal sbyte ConMod;
        [JsonProperty]
        internal sbyte DexMod;
        [JsonProperty]
        internal int MaxHPMod;
        [JsonProperty]
        internal int MaxMPMod;
        [JsonProperty]
        internal int CurrentHPMod;
        [JsonProperty]
        internal int CurrentMPMod;
        [JsonProperty]
        internal Animation Animation;
        [JsonProperty]
        internal uint AnimationDelay;
        [JsonProperty]
        internal TimeSpan Duration;

        public static bool operator ==(Effect eff1, Effect eff2) => eff1.Equals(eff2);
        public static bool operator !=(Effect eff1, Effect eff2) => !eff1.Equals(eff2);

        /// <summary>
        /// An effect with only an animation.
        /// </summary>
        /// <param name="animation">The animation to display.</param>
        /// <param name="animationDelay">The delay between animations in milliseconds.</param>
        /// <param name="duration">The total duration of the effect in milliseconds.</param>
        internal Effect(uint animationDelay, TimeSpan duration, bool useParentAnimation, Animation animation = default(Animation))
            :this(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, animationDelay, duration, useParentAnimation, animation)
        {
        }

        /// <summary>
        /// Full contructor for an effect.
        /// </summary>
        internal Effect(sbyte strMod, sbyte intMod, sbyte wisMod, sbyte conMod, sbyte dexMod, int maxHPMod, int maxMPMod, int currentHPMod,
            int currentMPMod, uint animationDelay, TimeSpan duration, bool useParentAnimation, Animation animation = default(Animation))
            : this(0, strMod, intMod, wisMod, conMod, dexMod, maxHPMod, maxMPMod, currentHPMod, currentMPMod, animationDelay, duration, useParentAnimation, animation)
        {

        }

        /// <summary>
        /// Master Constructor for Effect.
        /// </summary>
        [JsonConstructor]
        internal Effect(ushort sprite, sbyte strMod, sbyte intMod, sbyte wisMod, sbyte conMod, sbyte dexMod, int maxHPMod, int maxMPMod, int currentHPMod, 
            int currentMPMod, uint animationDelay, TimeSpan duration, bool useParentAnimation, Animation animation = default(Animation))
        {
            StartTime = DateTime.UtcNow;
            UseParentAnimation = useParentAnimation;

            Sprite = sprite;
            StrMod = strMod;
            IntMod = intMod;
            WisMod = wisMod;
            ConMod = conMod;
            DexMod = dexMod;
            MaxHPMod = maxHPMod;
            MaxMPMod = maxMPMod;
            CurrentHPMod = currentHPMod;
            CurrentMPMod = currentMPMod;
            AnimationDelay = animationDelay;
            Duration = duration;
            Animation = animation;

            CurrentColor = EffectsBarColor.None;
            CurrentColor = Color();
        }

        /// <summary>
        /// Returns a re-target effect based on a target and source id.
        /// </summary>
        internal Effect GetTargetedEffect(int targetID, int sourceID)
        {
            return new Effect(Sprite, StrMod, IntMod, WisMod, ConMod, DexMod, MaxHPMod, MaxMPMod, CurrentHPMod, CurrentMPMod, AnimationDelay, 
                Duration, UseParentAnimation, Animation.GetTargetedEffectAnimation(targetID, sourceID));
        }

        /// <summary>
        /// Returns a re-target effect based on a target point.
        /// </summary>
        internal Effect GetTargetedEffect(Point point)
        {
            return new Effect(Sprite, StrMod, IntMod, WisMod, ConMod, DexMod, MaxHPMod, MaxMPMod, CurrentHPMod, CurrentMPMod, AnimationDelay,
                Duration, UseParentAnimation, Animation.GetTargetedEffectAnimation(point));
        }

        /// <summary>
        /// Static contructur for no effect.
        /// </summary>
        internal static Effect None => default(Effect);

        /// <summary>
        /// Returns the remaining duration of the effect in milliseconds.
        /// </summary>
        internal int RemainingDurationMS()
        {
            TimeSpan elapsed = DateTime.UtcNow.Subtract(StartTime);

            if (elapsed > Duration)
                return 0;
            else
                return (int)Duration.Subtract(elapsed).TotalMilliseconds;
        }

        /// <summary>
        /// Returns what color the bar should be based on the remaining time.
        /// </summary>
        /// <returns></returns>
        internal EffectsBarColor Color()
        {
            int ms = RemainingDurationMS();

            if (ms >= 60000) //1min
                return EffectsBarColor.White;
            else if (ms >= 45000)
                return EffectsBarColor.Red;
            else if (ms >= 30000)
                return EffectsBarColor.Orange;
            else if (ms >= 15000)
                return EffectsBarColor.Yellow;
            else if (ms >= 5000)
                return EffectsBarColor.Green;
            else if (ms > 0)
                return EffectsBarColor.Blue;
            else
                return EffectsBarColor.None;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Effect))
                return false;

            Effect eff = (Effect)obj;

            return GetHashCode() == eff.GetHashCode();
        }
        public override int GetHashCode() => (Animation.GetHashCode() + (ushort)(AnimationDelay << 16) + (ushort)(Duration.TotalMilliseconds));

        public bool Equals(Effect other) => GetHashCode() == other.GetHashCode();
    }
}
