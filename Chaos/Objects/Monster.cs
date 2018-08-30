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

using System;
using System.Collections.Generic;

namespace Chaos
{
    /// <summary>
    /// Represents an in-game enemy, or monster.
    /// </summary>
    internal sealed class Monster : Creature
    {
        internal bool IsActive { get; set; }
        internal TimeSpan UpdateInterval { get; }
        internal DateTime LastUpdate { get; set; }
        internal List<Item> Items { get; set; }
        internal uint Gold;
        internal override byte HealthPercent => Utility.Clamp<byte>((CurrentHP * 100) / MaximumHP, 0, (int)MaximumHP);
        internal override uint MaximumHP { get; }
        internal override uint CurrentHP { get; set; }

        /// <summary>
        /// Master constructor an object representing an in-game monster.
        /// </summary>
        internal Monster(string name, ushort sprite, CreatureType type, Point point, Map map, TimeSpan updateInterval, Direction direction = Direction.South)
            : base(name, sprite, type, point, map, direction)
        {
            UpdateInterval = updateInterval;
            LastUpdate = DateTime.MinValue;
            IsActive = false;
            Items = new List<Item>();
        }
    }
}
