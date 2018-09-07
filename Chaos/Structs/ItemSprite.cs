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
    internal struct ItemSprite
    {
        [JsonProperty]
        internal ushort InventorySprite;
        [JsonProperty]
        internal ushort DisplaySprite;
        internal ushort OffsetSprite;

        /// <summary>
        /// Json & Master constructor for a structure representing an in-game item's sprite pair, which is the sprite shown in the panel, and the sprite shown when equipped.
        /// </summary>
        /// <param name="inventorySprite">The sprite number for when it is shown in the panel.</param>
        /// <param name="displaySprite">The sprite number for when it is equipped.</param>
        [JsonConstructor]
        internal ItemSprite(ushort inventorySprite, ushort displaySprite = 0)
        {
            InventorySprite = inventorySprite;
            DisplaySprite = displaySprite;
            OffsetSprite = (ushort)(inventorySprite + CONSTANTS.ITEM_SPRITE_OFFSET);
        }
    }
}
