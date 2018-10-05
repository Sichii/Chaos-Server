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

namespace Chaos
{
    [JsonObject(MemberSerialization.OptIn)]
    internal sealed class LegendMark
    {
        [JsonProperty]
        private readonly string Mark;

        [JsonProperty]
        internal string Key { get; }
        [JsonProperty]
        internal MarkIcon Icon { get; }
        [JsonProperty]
        internal MarkColor Color { get; }

        [JsonProperty]
        internal int Count { get; set; }
        [JsonProperty]
        internal GameTime Added { get; set; }

        /// <summary>
        /// Base Constructor for an object representing a new legend mark.
        /// </summary>
        /// <param name="key">Key of the mark.</param>
        /// <param name="mark">Text of the mark.</param>
        /// <param name="now">Time the mark was added.</param>
        /// <param name="icon">Icon displayed on the mark.</param>
        /// <param name="color">Text color of the mark.</param>
        internal LegendMark(GameTime now, string mark, string key, MarkIcon icon, MarkColor color)
        {
            Added = now;
            Key = key;
            Mark = mark;
            Count = 1;
            Icon = icon;
            Color = color;
        }

        /// <summary>
        /// Json and Master constructor for an object representing an existing legend mark.
        /// </summary>
        [JsonConstructor]
        internal LegendMark(GameTime added, string mark, string key, MarkIcon icon, MarkColor color, int count)
        {
            Added = added;
            Mark = mark;
            Key = key;
            Icon = icon;
            Color = color;
            Count = count;
        }

        /// <summary>
        /// Returns string representation of a LegendMark ready for ingame use.
        /// </summary>
        public override string ToString() => (Count > 1) ? $@"{Mark} ({Count}) - {Added.ToString()}" : $@"{Mark} - {Added.ToString()}";
    }
}
