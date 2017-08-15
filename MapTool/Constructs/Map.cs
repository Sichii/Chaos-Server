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
