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

namespace MapTool
{
    internal class Map
    {
        internal ushort Id { get; set; }
        internal byte SizeX { get; set; }
        internal byte SizeY { get; set; }
        internal MapFlags Flags { get; set; }
        internal string Name { get; set; }
        internal sbyte Music { get; set; }
        internal Dictionary<Point, Warp> Exits { get; set; }
        internal Dictionary<Point, WorldMap> WorldMaps { get; set; }
        internal Dictionary<Point, Door> Doors { get; set; }

        internal Map(ushort id, byte sizeX, byte sizeY, MapFlags flags, string name, sbyte music)
        {
            Id = id;
            SizeX = sizeX;
            SizeY = sizeY;
            Flags = flags;
            Name = name;
            Music = music;
            Exits = new Dictionary<Point, Warp>();
            WorldMaps = new Dictionary<Point, WorldMap>();
            Doors = new Dictionary<Point, Door>();
        }
    }
    [Flags]
    internal enum MapFlags : uint
    {
        Hostile = 1,
        NonHostile = 2,
        NoSpells = 4,
        NoSkills = 8,
        NoChat = 16,
        Snowing = 32,
        PvP = 64
    }
}
