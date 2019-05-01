using System.Collections.Generic;
using System.Linq;

namespace Chaos
{
    internal sealed class Pathfinder
    {
        private readonly Map Map;
        private readonly Pathnode[,] Pathnodes;
        private readonly LinkedList<Pathnode> Opened;

        internal Pathfinder(Map map)
        {
            Map = map;

            //initialize containers
            Pathnodes = new Pathnode[map.SizeX, map.SizeY];
            Opened = new LinkedList<Pathnode>();

            Initialize();
        }

        private void Initialize()
        {
            //create each pathnode
            for (byte x = 0; x < Map.SizeX; x++)
                for (byte y = 0; y < Map.SizeY; y++)
                {
                    var pathNode = new Pathnode((x, y));
                    pathNode.IsWall = Map.IsWall(pathNode.Point);
                    Pathnodes[x, y] = pathNode;
                }

            //warps are also walls
            foreach (Warp warp in Map.GetLockedInstance<Warp>())
                Pathnodes[warp.Point.X, warp.Point.Y].IsWall = true;

            //worldmaps are also walls
            foreach (KeyValuePair<Point, WorldMap> kvp in Map.GetLockedInstance<KeyValuePair<Point, WorldMap>>())
                Pathnodes[kvp.Key.X, kvp.Key.Y].IsWall = true;

            //set each pathnode's neighbors
            for (byte x = 0; x < Map.SizeX; x++)
                for (byte y = 0; y < Map.SizeY; y++)
                {
                    Pathnode node = Pathnodes[x, y];

                    if (y > 0)
                        node.Neighbors[0] = Pathnodes[x, y - 1];
                    if (x < Map.SizeX - 1)
                        node.Neighbors[1] = Pathnodes[x + 1, y];
                    if (y < Map.SizeY - 1)
                        node.Neighbors[2] = Pathnodes[x, y + 1];
                    if (x > 0)
                        node.Neighbors[3] = Pathnodes[x - 1, y];
                }
        }

        private Pathnode GetOptimalNode()
        {
            //the best node of the bunch, start with the first.
            LinkedListNode<Pathnode> optimalNode = Opened.First;

            //search all open nodes for the lowest aggregate
            for (LinkedListNode<Pathnode> currentNode = Opened.First; currentNode != null; currentNode = currentNode.Next)
                if (currentNode.Value.Aggregate < optimalNode.Value.Aggregate)
                    optimalNode = currentNode;

            return optimalNode.Value;
        }

        private void OpenNode(Pathnode node, Point sourcePoint, Point targetPoint)
        {
            //sets a node to open, add it to the opened list, and sets the aggregate
            node.Open = true;
            node.Aggregate = sourcePoint.Distance(node.Point) + targetPoint.Distance(node.Point);
            Opened.AddFirst(node);
        }

        private void CloseNode(Pathnode node)
        {
            //closes a node, removes it from the opened list
            node.Open = false;
            node.Closed = true;
            Opened.Remove(node);
        }

        internal Stack<Point> FindPath(Point sourcePoint, Point targetPoint)
        {
            //current pathnode is the point we're starting from
            Pathnode current = Pathnodes[sourcePoint.X, sourcePoint.Y];
            Pathnode target = Pathnodes[targetPoint.X, targetPoint.Y];
            var path = new Stack<Point>();

            //cleanup previous path's variables
            Opened.Clear();
            for (byte x = 0; x < Map.SizeX; x++)
                for (byte y = 0; y < Map.SizeY; y++)
                {
                    Pathnode node = Pathnodes[x, y];
                    node.HasCreature = false;
                    node.Open = false;
                    node.Closed = false;
                    node.Parent = null;
                }

            //set hascreature for all creatures spots
            foreach (Creature creature in Map.GetLockedInstance<Creature>())
                Pathnodes[creature.Point.X, creature.Point.Y].HasCreature = true;

            //opens the current node
            OpenNode(current, sourcePoint, targetPoint);

            //while the next optimal node is not the target node
            while ((current = GetOptimalNode()) != target)
            {
                //if there is no optimal node (we are likely walled in or something)
                if (current == default)
                {
                    current = Pathnodes[sourcePoint.X, sourcePoint.Y];
                    Direction optimalDirection2 = targetPoint.Relation(current.Point);

                    //for each direction, starting with the best direction, push the first walkable point.
                    foreach(Direction direction in optimalDirection2.StartEnumerable())
                    {
                        Pathnode neighbor = current.Neighbors[(int)direction];

                        if(!neighbor.IsWall && !neighbor.HasCreature)
                        {
                            path.Push(neighbor.Point);
                            return path;
                        }
                    }

                    //if no walkable point, return an empty path
                    return path;
                }

                Direction optimalDirection = targetPoint.Relation(current.Point);

                //for each direction, the best direction being last, check nodes viability and place them in the opened list. Best node will be placed at the beginning of the list to cut down iterations.
                foreach(Direction direction in optimalDirection.StartEnumerable().Reverse())
                {
                    Pathnode neighbor = current.Neighbors[(int)optimalDirection];

                    if (neighbor != default && !neighbor.IsWall && !neighbor.HasCreature && !neighbor.Open && !neighbor.Closed)
                    {
                        OpenNode(neighbor, sourcePoint, targetPoint);
                        neighbor.Parent = current;
                    }
                }

                //closes the current node, all of it's neighbors have been searched
                CloseNode(current);
            }

            //generate the path
            while (current.Parent != null)
            {
                path.Push(current.Point);
                current = current.Parent;
            }

            return path;
        }
    }
}
