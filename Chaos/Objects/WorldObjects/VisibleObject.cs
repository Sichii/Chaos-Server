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

namespace Chaos
{
    /// <summary>
    /// Represents an object that is visible.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    internal abstract class VisibleObject : MapObject
    {
        internal new static readonly Type TypeRef = typeof(VisibleObject);

        [JsonProperty]
        internal ushort Sprite { get; }

        /// <summary>
        /// Json & Master constructor for an object that is visible.
        /// </summary>
        [JsonConstructor]
        protected VisibleObject(string name, Location location, ushort sprite)
          : base(name, location)
        {
            Sprite = sprite;
        }

        /// <summary>
        /// Checks if a point is within 13 spaces of the object.
        /// </summary>
        internal bool WithinRange(Point p) => Point.Distance(p) < 13;

        /// <summary>
        /// Checks if another VisibleObject is within 13 spaces of the object.
        /// </summary>
        internal bool WithinRange(VisibleObject v) => Point.Distance(v.Point) < 13;

        public override string ToString() => $@"({Name}({Sprite}) - {Location})";
    }
}
