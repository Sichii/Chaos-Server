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

using System.Collections.Generic;
using System.Windows.Forms;
using Chaos.Containers.WorldContainers;
using Chaos.Structs;

namespace ChaosTool.Tree
{
    internal class WorldMapTreeNode : TreeNode
    {
        internal Point Point { get; }
        internal WorldMap WorldMap { get; }

        internal WorldMapTreeNode(WorldMap wmap, string name)
            :this(new KeyValuePair<Point, WorldMap>(Point.None, wmap), name)
        {
        }

        internal WorldMapTreeNode(KeyValuePair<Point, WorldMap> kvp, string name)
            : base(name)
        {
            Point = kvp.Key;
            WorldMap = kvp.Value;
            Name = kvp.Value.CheckSum.ToString();
        }
    }
}