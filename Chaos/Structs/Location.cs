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
using System.Text.RegularExpressions;

namespace Chaos
{
    public struct Location
    {
        [JsonProperty]
        internal ushort MapId;
        [JsonProperty]
        internal ushort X;
        [JsonProperty]
        internal ushort Y;
        internal Point Point => new Point(X, Y);

        [JsonConstructor]
        internal Location(ushort mapId, ushort x, ushort y)
        {
            MapId = mapId;
            X = x;
            Y = y;
        }

        internal Location(ushort id, Point point)
        {
            MapId = id;
            X = point.X;
            Y = point.Y;
        }

        public static bool operator ==(Location loc1, Location loc2) => loc1.Equals(loc2);
        public static bool operator !=(Location loc1, Location loc2) => !loc1.Equals(loc2);
        public override int GetHashCode() => (MapId << 16) + (X << 8) + Y;
        public override string ToString() => $"{MapId} {X},{Y}";

        public override bool Equals(object obj) => (obj as Location?)?.GetHashCode() == GetHashCode();

        public static bool TryParse(string str, out Location loc)
        {
            ushort mapId = 0;
            ushort x = 0;
            ushort y = 0;
            Match m = Regex.Match(str, @"([0-9]+) ([0-9]+) ([0-9]+)");
            
            if(m.Success && 
                ushort.TryParse(m.Groups[1].Value, out mapId) && 
                ushort.TryParse(m.Groups[2].Value, out x) &&
                ushort.TryParse(m.Groups[3].Value, out y))
            {
                loc = new Location(mapId, x, y);
                return true;
            }

            loc = new Location();
            return false;
        }
    }
}
