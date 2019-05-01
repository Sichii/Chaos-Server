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

using System;
using Newtonsoft.Json;

namespace Chaos
{
    /// <summary>
    /// Represents an object that exists within the in-game panels.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    internal abstract class PanelObject : IEquatable<PanelObject>
    {
        internal TargetsType TargetType { get; }
        internal bool UsersOnly { get; }
        internal int BaseDamage { get; }
        internal TimeSpan BaseCooldown { get; }
        internal ushort Sprite { get; }
        [JsonProperty]
        internal string Name { get; }
        internal Animation Animation { get; }
        internal BodyAnimation BodyAnimation { get; }
        internal Effect Effect { get; }
        internal ActivationDelegate Activate { get; }
        [JsonProperty]
        internal byte Slot { get; set; }
        internal int CooldownReduction { get; set; }
        internal DateTime LastUse { get; set; }
        [JsonProperty]
        internal TimeSpan Elapsed => DateTime.UtcNow.Subtract(LastUse);
        internal TimeSpan Cooldown => new TimeSpan(0, 0, 0, 0, (int)(BaseCooldown.TotalMilliseconds * (Utilities.Clamp<int>(100 - CooldownReduction, 0, 100)/100)));
        internal virtual bool CanUse => LastUse == DateTime.MinValue || BaseCooldown.TotalMilliseconds == 0 || DateTime.UtcNow.Subtract(LastUse) >= Cooldown;

        /// <summary>
        /// Master constructor for an object that exists within the in-game panels.
        /// </summary>
        internal PanelObject(byte slot, ushort sprite, string name, TimeSpan baseCooldown, Animation effectAnimation, TargetsType targetType = TargetsType.None, bool usersOnly = false,
            BodyAnimation bodyAnimation = 0, int baseDamage = 0, Effect effect = default)
        {
            Slot = slot;
            Sprite = sprite;
            Name = name;
            BaseCooldown = baseCooldown;
            LastUse = DateTime.MinValue;
            Animation = effectAnimation;
            TargetType = targetType;
            UsersOnly = usersOnly;
            BodyAnimation = bodyAnimation;
            BaseDamage = baseDamage;
            Activate = Game.CreationEngine.GetEffect(Name);

            effect = effect ?? Effect.None;

            effect.Sprite = sprite;
            if (effect.UseParentAnimation)
                effect.Animation = Animation;

            Effect = effect;
        }

        /// <summary>
        /// Json constructor for a PanelObject. Minimal information is serialized, as we retreive the object from the creation engine, and apply persistent information to it.
        /// </summary>
        [JsonConstructor]
        protected internal PanelObject(byte slot, string name, TimeSpan elapsed)
        {
            Slot = slot;
            Name = name;
            LastUse = DateTime.UtcNow.Subtract(elapsed);
        }

        public override int GetHashCode() => (Name?.GetHashCode() ?? 0 << 16) + Sprite;
        public override bool Equals(object other) => (other is PanelObject tPanelObject) ? Equals(tPanelObject) : false;
        public bool Equals(PanelObject other) => !(other is null) && (GetHashCode() == other.GetHashCode());
        public override string ToString() => $@"SLOT: {Slot} | NAME: {Name}({Sprite})";
    }
}
