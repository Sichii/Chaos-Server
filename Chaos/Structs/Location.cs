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
    public struct Location
    {
        [JsonProperty]
        internal ushort MapId;
        [JsonProperty]
        internal ushort X;
        [JsonProperty]
        internal ushort Y;
        internal Point Point => (X, Y);

        /// <summary>
        /// Json & Master constructor for a structure representing a point in the world, which is a point paired with a map ID.
        /// </summary>
        [JsonConstructor]
        private Location(ushort mapId, ushort x, ushort y)
        {
            MapId = mapId;
            X = x;
            Y = y;
        }

        public static implicit operator Location(ValueTuple<int, int, int> tuple) => new Location((ushort)tuple.Item1, (ushort)tuple.Item2, (ushort)tuple.Item3);
        public static implicit operator Location(ValueTuple<int, Point> tuple) => new Location((ushort)tuple.Item1, tuple.Item2.X, tuple.Item2.Y);

        /// <summary>
        /// Returns the equivalent of no location.
        /// </summary>
        internal static Location None => (ushort.MaxValue, Point.None);

        public static bool operator ==(Location loc1, Location loc2) => loc1.Equals(loc2);
        public static bool operator !=(Location loc1, Location loc2) => !loc1.Equals(loc2);
        public override int GetHashCode() => (MapId << 16) + (X << 8) + Y;
        public override string ToString() => $"{MapId} {Point}";

        public override bool Equals(object obj) => (obj as Location?)?.GetHashCode() == GetHashCode();

        /// <summary>
        /// Attempts to parse a location from a string.
        /// </summary>
        public static bool TryParse(string str, out Location location)
        {
            location = None;
            Match m = Regex.Match(str, @"(\d+) \(?(\d+)(?:,| |, )(\d+)\)?");

            return m.Success && ushort.TryParse(m.Groups[1].Value, out location.MapId) && ushort.TryParse(m.Groups[2].Value, out location.X) && ushort.TryParse(m.Groups[3].Value, out location.Y);
        }
    }
}
