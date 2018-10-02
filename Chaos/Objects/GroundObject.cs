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
    /// <summary>
    /// Represents an object that can be placed on the ground, generally an item.
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    internal class GroundObject : VisibleObject
    {
        internal Item Item { get; }
        internal uint Amount { get; set; }
        /// <summary>
        /// Json & Master constructor for an object that can be placed on the ground.
        /// </summary>
        [JsonConstructor]
        internal GroundObject(Location location, ushort sprite, uint amount, Item item = null)
          : base(item?.Name ?? "Gold", location, sprite)
        {
            Item = item;
            Amount = amount;
        }
    }
}
