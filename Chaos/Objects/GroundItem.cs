using Newtonsoft.Json;

namespace Chaos.Objects
{
    [JsonObject(MemberSerialization.OptOut)]
    internal sealed class GroundItem : VisibleObject
    {
        internal Item Item { get; set; }

        [JsonConstructor]
        internal GroundItem(ushort sprite, Point point, Map map, Item item = null)
          : base(string.Empty, sprite, point, map)
        {
            Item = item;
        }
    }
}
