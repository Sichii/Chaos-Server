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
using System.Text.RegularExpressions;

namespace Chaos
{
    [JsonObject(MemberSerialization.OptIn)]
    public struct Location : IEquatable<Location>
    {
        [JsonProperty]
        public ushort MapID { get; }
        [JsonProperty]
        public Point Point { get; }

        /// <summary>
        /// Json & Master constructor for a structure representing a point in the world, which is a point paired with a map ID.
        /// </summary>
        [JsonConstructor]
        private Location(ushort mapID, Point point)
        {
            MapID = mapID;
            Point = point;
        }

        /// <summary>
        /// Returns the equivalent of no location.
        /// </summary>
        internal static Location None => (ushort.MaxValue, Point.None);

        public static implicit operator Location(ValueTuple<int, int, int> tuple) => new Location((ushort)tuple.Item1, ((ushort)tuple.Item2, (ushort)tuple.Item3));
        public static implicit operator Location(ValueTuple<int, Point> tuple) => new Location((ushort)tuple.Item1, tuple.Item2);
        public static bool operator ==(Location left, Location right) => left.Equals(right);
        public static bool operator !=(Location left, Location right) => !(left == right);

        /// <summary>
        /// Attempts to parse a location from a string.
        /// </summary>
        public static bool TryParse(string str, out Location location)
        {
            location = None;
            Match m = Regex.Match(str, @"(\d+)(?::| )\(?(\d+)(?:,| |, )(\d+)\)?");

            location = (m.Success && ushort.TryParse(m.Groups[1].Value, out ushort mapID) && ushort.TryParse(m.Groups[2].Value, out ushort x) && ushort.TryParse(m.Groups[3].Value, out ushort y)) ? (mapID, x, y) 
                : None;

            return location != None;
        }

        public override int GetHashCode() => (MapID << 16) + Point.GetHashCode();
        public override bool Equals(object obj) => (obj is Location tLocation) ? Equals(tLocation) : false;
        public bool Equals(Location other) => GetHashCode() == other.GetHashCode();
        public override string ToString() => $"{MapID}:{Point}";
    }
}
