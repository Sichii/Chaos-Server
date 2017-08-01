namespace Chaos
{
    internal abstract class MapObject
    {
        internal Point Point => new Point(X, Y);
        internal Location Location => new Location(MapId, Point);
        protected ushort X;
        protected ushort Y;
        protected ushort MapId;

        protected internal MapObject(ushort mapId, ushort x, ushort y)
        {
            MapId = mapId;
            X = x;
            Y = y;
        }

        protected internal MapObject(Location location)
        {
            MapId = location.MapId;
            X = location.X;
            Y = location.Y;
        }

        protected internal MapObject(ushort mapId, Point point)
        {
            MapId = mapId;
            X = point.X;
            Y = point.Y;
        }
    }
}
