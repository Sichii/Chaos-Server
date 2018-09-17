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

using System.Collections;

namespace ChaosTool
{
    internal class TreeNodeSorter : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x is MapTreeNode && y is MapTreeNode)
            {
                var node1 = x as MapTreeNode;
                var node2 = y as MapTreeNode;

                //sort by ascending id
                return node1.Map.Name.CompareTo(node2.Map.Name);
            }
            else if (x is WarpTreeNode && y is WarpTreeNode)
            {
                var node1 = x as WarpTreeNode;
                var node2 = y as WarpTreeNode;

                //sort ascending target map id, if theyre the same then sort by point.x, then point.y
                int id1 = node1.Warp.TargetMapId, id2 = node2.Warp.TargetMapId;
                if (id1 == id2)
                {
                    Chaos.Point p1 = node1.Warp.Point, p2 = node2.Warp.Point;
                    if (p1.X == p2.X)
                        return p1.Y.CompareTo(p2.Y);
                    else
                        return p1.X.CompareTo(p2.X);
                }
                else
                    return id1.CompareTo(id2);
            }
            else if (x is WorldMapNodeTreeNode && y is WorldMapNodeTreeNode)
            {
                var node1 = x as WorldMapNodeTreeNode;
                var node2 = y as WorldMapNodeTreeNode;

                //sort by x position
                return node1.WorldMapNode.Position.X.CompareTo(node2.WorldMapNode.Position.X);
            }
            else if (x is WorldMapTreeNode && y is WorldMapTreeNode)
            {
                var node1 = x as WorldMapTreeNode;
                var node2 = y as WorldMapTreeNode;

                //sort by crc
                return node1.WorldMap.CheckSum.CompareTo(node2.WorldMap.CheckSum);
            }
            return 0;
        }
    }
}
