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
    [JsonObject(MemberSerialization.OptOut)]
    internal sealed class Skill : PanelObject
    {
        [JsonProperty]
        internal bool IsBasic { get; }
        [JsonProperty]
        internal byte Animation { get; }

        internal Skill(byte slot, string name, ushort sprite, TimeSpan cooldown)
            :base(slot, sprite, name, cooldown)
        {
        }

        [JsonConstructor]
        internal Skill(byte slot, ushort sprite, string name, TimeSpan cooldown, bool isBasic, byte animation)
            :base(slot, sprite, name, cooldown)
        {
            IsBasic = isBasic;
            Animation = animation;
        }
        
    }
}
