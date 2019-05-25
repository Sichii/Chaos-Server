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
    internal sealed class Effect : IEquatable<Effect>
    {
        internal static readonly Type TypeRef = typeof(Effect);

        private readonly DateTime StartTime;
        internal EffectsBarColor Color { get; private set; }

        [JsonProperty]
        internal ushort Sprite { get; set; }
        [JsonProperty]
        internal bool UseParentAnimation { get; }
        [JsonProperty]
        internal sbyte StrMod { get; }
        [JsonProperty]
        internal sbyte IntMod { get; }
        [JsonProperty]
        internal sbyte WisMod { get; }
        [JsonProperty]
        internal sbyte ConMod { get; }
        [JsonProperty]
        internal sbyte DexMod { get; }
        [JsonProperty]
        internal int MaxHPMod { get; }
        [JsonProperty]
        internal int MaxMPMod { get; }
        [JsonProperty]
        internal int CurrentHPMod { get; }
        [JsonProperty]
        internal int CurrentMPMod { get; }
        [JsonProperty]
        internal uint AnimationDelay { get; }
        [JsonProperty]
        internal Animation Animation { get; set; }
        [JsonProperty]
        internal TimeSpan Duration { get; private set; }


        public static bool operator ==(Effect eff1, Effect eff2) => eff1.Equals(eff2);
        public static bool operator !=(Effect eff1, Effect eff2) => !eff1.Equals(eff2);

        /// <summary>
        /// Constructor for an effect with only an animation.
        /// </summary>
        internal Effect(uint animationDelay, TimeSpan duration, bool useParentAnimation, Animation animation = default)
            : this(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, animationDelay, duration, useParentAnimation, animation ?? Animation.None)
        {
        }

        /// <summary>
        /// Master constructor for a structure representing an in-game persistent effect.
        /// </summary>
        internal Effect(sbyte strMod, sbyte intMod, sbyte wisMod, sbyte conMod, sbyte dexMod, int maxHPMod, int maxMPMod, int currentHPMod,
            int currentMPMod, uint animationDelay, TimeSpan duration, bool useParentAnimation, Animation animation = default)
            : this(0, strMod, intMod, wisMod, conMod, dexMod, maxHPMod, maxMPMod, currentHPMod, currentMPMod, animationDelay, duration, useParentAnimation, animation)
        {

        }

        /// <summary>
        /// Json constructor for a structure representing an in-game persistent effect.
        /// </summary>
        [JsonConstructor]
        private Effect(ushort sprite, sbyte strMod, sbyte intMod, sbyte wisMod, sbyte conMod, sbyte dexMod, int maxHPMod, int maxMPMod, int currentHPMod,
            int currentMPMod, uint animationDelay, TimeSpan duration, bool useParentAnimation, Animation animation = default)
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

            Color = EffectsBarColor.None;
        }

        /// <summary>
        /// Returns a re-target effect based on a target and source id.
        /// </summary>
        internal Effect GetTargetedEffect(int targetID, int sourceID) =>
            new Effect(Sprite, StrMod, IntMod, WisMod, ConMod, DexMod, MaxHPMod, MaxMPMod, CurrentHPMod, CurrentMPMod, AnimationDelay,
                Duration, UseParentAnimation, Animation.GetTargetedEffectAnimation(targetID, sourceID));

        /// <summary>
        /// Returns a re-target effect based on a target point.
        /// </summary>
        internal Effect GetTargetedEffect(Point point) =>
            new Effect(Sprite, StrMod, IntMod, WisMod, ConMod, DexMod, MaxHPMod, MaxMPMod, CurrentHPMod, CurrentMPMod, AnimationDelay,
                Duration, UseParentAnimation, Animation.GetTargetedEffectAnimation(point));

        /// <summary>
        /// Static contructur for no effect.
        /// </summary>
        internal static Effect None => new Effect(0, TimeSpan.Zero, false);

        /// <summary>
        /// Returns the remaining duration of the effect in milliseconds.
        /// </summary>
        internal int RemainingDurationMS() => Math.Max(0, (int)Duration.Subtract(DateTime.UtcNow.Subtract(StartTime)).TotalMilliseconds);

        /// <summary>
        /// Returns what color the bar should be based on the remaining time.
        /// </summary>
        /// <returns></returns>
        private EffectsBarColor GetColor()
        {
            int ms = RemainingDurationMS();

            return ms >= 60000 ? EffectsBarColor.White
                : ms >= 45000 ? EffectsBarColor.Red
                : ms >= 30000 ? EffectsBarColor.Orange
                : ms >= 15000 ? EffectsBarColor.Yellow
                : ms >= 5000 ? EffectsBarColor.Green
                : ms > 0 ? EffectsBarColor.Blue
                : EffectsBarColor.None;
        }

        internal bool ShouldSendColor()
        {
            EffectsBarColor currentColor = GetColor();

            if(Color != currentColor)
            {
                Color = currentColor;
                return true;
            }

            return false;
        }

        internal void EndEffect() => Duration = TimeSpan.Zero;

        public override int GetHashCode() => (Animation.GetHashCode() << 24) + (ushort)(AnimationDelay << 16) + (ushort)Duration.TotalMilliseconds;
        public override bool Equals(object obj) => (obj is Effect tEffect) ? Equals(tEffect) : false;
        public bool Equals(Effect other) => !(other is null) && GetHashCode() == other.GetHashCode();
    }
}
