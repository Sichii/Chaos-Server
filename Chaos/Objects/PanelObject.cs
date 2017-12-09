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
    internal abstract class PanelObject
    {
        [JsonProperty]
        internal byte Slot { get; set; }
        [JsonProperty]
        internal ushort Sprite { get; }
        [JsonProperty]
        internal string Name { get; }
        [JsonProperty]
        internal TimeSpan Cooldown { get; }
        [JsonProperty]
        internal DateTime LastUse { get; set; }
        [JsonProperty]
        internal Animation EffectAnimation { get; set; }
        [JsonProperty]
        internal byte BodyAnimation { get; set; }
        internal OnUseDelegate Activate { get; }
        internal abstract bool CanUse { get; }

        internal PanelObject(byte slot, ushort sprite, string name, TimeSpan cooldown, Animation effectAnimation , byte bodyAnimation = 0)
        {
            Slot = slot;
            Sprite = sprite;
            Name = name;
            Cooldown = cooldown;
            LastUse = DateTime.MinValue;
            EffectAnimation = effectAnimation;
            BodyAnimation = bodyAnimation;
            Activate = Game.CreationEngine.GetEffect(Name);
        }
    }
}
