using Newtonsoft.Json;
using System;

namespace Chaos
{
    [JsonObject(MemberSerialization.OptOut)]
    internal struct ItemSprite
    {
        internal ushort InventorySprite;
        internal ushort OffsetSprite;
        internal ushort DisplaySprite;

        [JsonConstructor]
        internal ItemSprite(ushort inventorySprite, ushort displaySprite)
        {
            InventorySprite = inventorySprite;
            OffsetSprite = (ushort)(inventorySprite + CONSTANTS.ITEM_SPRITE_OFFSET);
            DisplaySprite = displaySprite;
        }
    }
}
