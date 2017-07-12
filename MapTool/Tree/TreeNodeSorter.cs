using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MapTool
{
    internal class TreeNodeSorter : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x is MapTreeNode && y is MapTreeNode)
            {
                MapTreeNode node1 = x as MapTreeNode;
                MapTreeNode node2 = y as MapTreeNode;

                //sort by ascending id
                return node1.Map.Name.CompareTo(node2.Map.Name);
            }
            else if (x is WarpTreeNode && y is WarpTreeNode)
            {
                WarpTreeNode node1 = x as WarpTreeNode;
                WarpTreeNode node2 = y as WarpTreeNode;

                //sort ascending target map id, if theyre the same then sort by point.x, then point.y
                int id1 = node1.Warp.TargetMapId, id2 = node2.Warp.TargetMapId;
                if (id1 == id2)
                {
                    Point p1 = node1.Warp.Point, p2 = node2.Warp.Point;
                    if (p1.X == p2.X)
                        return p1.Y.CompareTo(p2.Y);
                    else
                        return p1.X.CompareTo(p2.X);
                }
                else
                    return id1.CompareTo(id2);
            }
            else if(x is DoorTreeNode && y is DoorTreeNode)
            {
                DoorTreeNode node1 = x as DoorTreeNode;
                DoorTreeNode node2 = x as DoorTreeNode;

                //sort by x position, then y position
                Point p1 = node1.Door.Point, p2 = node2.Door.Point;
                if (p1.X == p2.X)
                    return p1.Y.CompareTo(p2.Y);
                else
                    return p1.X.CompareTo(p2.X);
            }
            else if (x is WorldMapNodeTreeNode && y is WorldMapNodeTreeNode)
            {
                WorldMapNodeTreeNode node1 = x as WorldMapNodeTreeNode;
                WorldMapNodeTreeNode node2 = y as WorldMapNodeTreeNode;

                //sort by x position
                return node1.WorldMapNode.Position.X.CompareTo(node2.WorldMapNode.Position.X);
            }
            else if (x is WorldMapTreeNode && y is WorldMapTreeNode)
            {
                WorldMapTreeNode node1 = x as WorldMapTreeNode;
                WorldMapTreeNode node2 = y as WorldMapTreeNode;

                //sort by crc
                return node1.WorldMap.GetCrc32().CompareTo(node2.WorldMap.GetCrc32());
            }
            return 0;
        }
    }
}
