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
    [JsonObject(MemberSerialization.OptIn)]
    internal abstract class PanelObject
    {
        [JsonProperty]
        internal TargetsType TargetType { get; }
        [JsonProperty]
        internal bool UsersOnly { get; }
        [JsonProperty]
        internal int BaseDamage { get; }
        [JsonProperty]
        internal TimeSpan BaseCooldown { get; }
        [JsonProperty]
        internal byte Slot { get; set; }
        [JsonProperty]
        internal ushort Sprite { get; }
        [JsonProperty]
        internal string Name { get; }
        [JsonProperty]
        internal float CooldownReduction { get; set; }
        [JsonProperty]
        internal DateTime LastUse { get; set; }
        [JsonProperty]
        internal Animation Animation { get; }
        [JsonProperty]
        internal BodyAnimation BodyAnimation { get; }
        [JsonProperty]
        internal Effect Effect { get; }
        internal OnUseDelegate Activate { get; }
        internal TimeSpan Cooldown => new TimeSpan(0, 0, 0, 0, (int)(BaseCooldown.TotalMilliseconds * Utility.Clamp<double>(1 - CooldownReduction, 0, 1)));
        internal virtual bool CanUse => LastUse == DateTime.MinValue || BaseCooldown.TotalMilliseconds == 0 || DateTime.UtcNow.Subtract(LastUse) >= Cooldown;

        /// <summary>
        /// Master constructor for PanelObject.
        /// An object representing an Item, Skill, or Spell that you would find on the game panels.
        /// </summary>
        [JsonConstructor]
        internal PanelObject(byte slot, ushort sprite, string name, TimeSpan baseCooldown, Animation effectAnimation , TargetsType targetType = TargetsType.None, bool usersOnly = false, 
            BodyAnimation bodyAnimation = 0, int baseDamage = 0, Effect effect = default(Effect))
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

            effect.Sprite = sprite;
            if (effect.UseParentAnimation)
                effect.Animation = Animation;
            Effect = effect;
        }
    }
}
