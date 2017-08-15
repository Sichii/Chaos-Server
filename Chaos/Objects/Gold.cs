namespace Chaos
{
    internal sealed class Gold : GroundItem
    {
        internal uint Amount { get; set; }

        internal Gold(byte sprite, Point point, Map map, uint amount)
          : base(sprite, point, map)
        {
            Amount = amount;
        }
    }
}
