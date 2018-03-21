using Newtonsoft.Json;
using System;

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

        [JsonConstructor]
        internal ItemSprite(ushort inventorySprite, ushort displaySprite)
        {
            InventorySprite = inventorySprite;
            OffsetSprite = (ushort)(inventorySprite + CONSTANTS.ITEM_SPRITE_OFFSET);
            DisplaySprite = displaySprite;
        }
    }
}
