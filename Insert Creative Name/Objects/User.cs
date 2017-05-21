namespace Insert_Creative_Name.Objects
{
    internal sealed class User : Creature
    {
        internal DisplayData DisplayData { get; set; }

        internal User(uint id, string name, Point point, Map map, Direction direction)
          : base(id, name, 0, 4, point, map, direction)
        {
            DisplayData = new DisplayData();
        }

        internal User(uint id, string name, Point point, Map map, DisplayData displayData, Direction direction)
            :base(id, name, 0, 4, point, map, direction)
        {
            DisplayData = displayData;
        }
    }
}
