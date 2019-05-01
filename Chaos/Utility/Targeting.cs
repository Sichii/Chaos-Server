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
using System.Linq;

namespace Chaos
{
    internal static class Targeting
    {
        /// <summary>
        /// Retreives a list of diagonal points in relevance to the user, with an optional distance and direction. Direction.All is optional. Direction.Invalid direction returns empty list.
        /// </summary>
        internal static IEnumerable<Point> GetInterCardinalPoints(Point start, int degree = 1, Direction direction = Direction.All)
        {
            for (int i = 1; i <= degree; i++)
            {
                switch (direction)
                {
                    case Direction.North:
                        yield return (start.X - i, start.Y - i);
                        yield return (start.X + i, start.Y - i);
                        break;
                    case Direction.East:
                        yield return (start.X + i, start.Y - i);
                        yield return (start.X + i, start.Y + i);
                        break;
                    case Direction.South:
                        yield return (start.X + i, start.Y + i);
                        yield return (start.X - i, start.Y + i);
                        break;
                    case Direction.West:
                        yield return (start.X - i, start.Y - i);
                        yield return (start.X - i, start.Y + i);
                        break;
                    case Direction.All:
                        yield return (start.X - i, start.Y - i);
                        yield return (start.X + i, start.Y - i);
                        yield return (start.X + i, start.Y + i);
                        yield return (start.X - i, start.Y + i);
                        break;
                    default:
                        yield break;
                }
            }
        }

        /// <summary>
        /// Retreives a list of points in a line from the user, with an option for distance and direction. Direction.All is optional. Direction.Invalid direction returns empty list.
        /// </summary>
        internal static List<Point> GetCardinalPoints(Point start, int degree = 1, Direction direction = Direction.All)
        {
            var cardinalPoints = new List<Point>();

            if (direction == Direction.Invalid)
                return cardinalPoints;

            for (int i = 1; i <= degree; i++)
            {
                switch (direction)
                {
                    case Direction.All:
                        cardinalPoints.Add(start.Offset(Direction.North, i));
                        cardinalPoints.Add(start.Offset(Direction.East, i));
                        cardinalPoints.Add(start.Offset(Direction.South, i));
                        cardinalPoints.Add(start.Offset(Direction.West, i));
                        break;
                    default:
                        cardinalPoints.Add(start.Offset(direction, i));
                        break;
                }
            }

            return cardinalPoints;
        }

        /// <summary>
        /// Creates an enumerable list of points representing a path between two given points, and returns it.
        /// </summary>
        /// <param name="start">Starting point for the creation of the path.</param>
        /// <param name="end">Ending point for the creation of the path.</param>
        /// <param name="includeStart">Whether or not to include the starting point in the result enumerable.</param>
        /// <param name="includeEnd">Whether or not to include the ending point in the result enumerable.</param>
        internal static IEnumerable<Point> GetDirectPath(Point start, Point end, bool includeStart = false, bool includeEnd = false)
        {
            if (includeStart)
                yield return start;

            while (start != end)
            {
                start = start.Offset(end.Relation(start));
                yield return start;
            }

            if (includeEnd)
                yield return end;
        }

        /// <summary>
        /// Retreives targets based on the target type of the effect.
        /// </summary>
        /// <param name="client">The client source of the effect.</param>
        /// <param name="targetPoint">The target point of the effect.</param>
        /// <param name="type">The target type to base the return on.</param>
        internal static List<Creature> TargetsFromType(Client client, Point targetPoint, TargetsType type = TargetsType.None)
        {
            var creatures = new List<Creature>();

            switch (type)
            {
                //generally skill types
                case TargetsType.None:
                    break;
                case TargetsType.Self:
                    creatures.Add(client.User);
                    break;
                case TargetsType.Front:
                    if (client.User.Map.TryGetObject(obj => obj.Point == client.User.Point.Offset(client.User.Direction), out Creature creature))
                        creatures.Add(creature);
                    break;
                case TargetsType.Surround:
                    creatures.AddRange(client.User.Map.ObjectsVisibleFrom(client.User.Point, false, 1).OfType<Creature>());
                    break;
                case TargetsType.Cleave:
                    creatures.AddRange(client.User.Map.ObjectsVisibleFrom(client.User.Point, false, 2).OfType<Creature>().Where(c =>
                        (c.Point.Distance(client.User.Point) == 1 && c.Point.Relation(client.User.Point) != client.User.Direction.Reverse()) ||
                        GetInterCardinalPoints(client.User.Point, 1, client.User.Direction).Contains(c.Point)));
                    break;
                case TargetsType.StraightProjectile:
                    List<Point> line = GetCardinalPoints(client.User.Point, 13, client.User.Direction);
                    creature = client.User.Map.ObjectsVisibleFrom(client.User.Point).OfType<Creature>().Where(c => line.Contains(c.Point)).Aggregate((c1, c2) => c1.Point.Distance(client.User.Point) < c2.Point.Distance(client.User.Point) ? c1 : c2);

                    if (creature != null)
                        creatures.Add(creature);
                    break;




                //generally spell types
                case TargetsType.Cluster1:
                    creatures.AddRange(client.User.Map.ObjectsVisibleFrom(targetPoint, false, 1).OfType<Creature>());
                    break;
                case TargetsType.Cluster2:
                    creatures.AddRange(client.User.Map.ObjectsVisibleFrom(targetPoint, false, 2).OfType<Creature>());
                    break;
                case TargetsType.Cluster3:
                    creatures.AddRange(client.User.Map.ObjectsVisibleFrom(targetPoint, false, 3).OfType<Creature>());
                    break;
                case TargetsType.Screen:
                    creatures.AddRange(client.User.Map.ObjectsVisibleFrom(targetPoint, false).OfType<Creature>());
                    break;
            }

            return creatures.Where(c => c.IsAlive).ToList();
        }

        /// <summary>
        /// Retreives points based on the target type of the effect.
        /// </summary>
        /// <param name="client">The client source of the effect.</param>
        /// <param name="targetPoint">The target point of the effect.</param>
        /// <param name="type">The target type to base the return on.</param>
        internal static List<Point> PointsFromType(Client client, Point targetPoint, TargetsType type = TargetsType.None)
        {
            var points = new List<Point>();

            switch (type)
            {
                //generally self cast types
                case TargetsType.None:
                    break;
                case TargetsType.Self:
                    points.Add(client.User.Point);
                    break;
                case TargetsType.Front:
                    points.Add(client.User.Point.Offset(client.User.Direction));
                    break;
                case TargetsType.Surround:
                    points.AddRange(client.User.Map.Points.Flatten().Where(p => p.Distance(client.User.Point) == 1));
                    break;
                case TargetsType.Cleave:
                    points.AddRange(GetInterCardinalPoints(client.User.Point, 1, client.User.Direction));
                    points.AddRange(client.User.Map.Points.Flatten().Where(p => p.Distance(client.User.Point) == 1 && p.Relation(client.User.Point) != client.User.Direction.Reverse()));
                    break;
                case TargetsType.StraightProjectile:
                    int distance = 13;
                    List<Point> line = GetCardinalPoints(client.User.Point, 13, client.User.Direction);
                    Creature creature = null;

                    foreach (Creature c in client.User.Map.ObjectsVisibleFrom(client.User.Point))
                    {
                        if (line.Contains(c.Point))
                        {
                            int dist = c.Point.Distance(client.User.Point);

                            if (dist < distance)
                            {
                                distance = dist;
                                creature = c;
                            }
                        }
                    }

                    if (creature != null)
                        points.Add(creature.Point);
                    break;




                //generally spell types
                case TargetsType.Cluster1:
                    points.AddRange(client.User.Map.Tiles.Keys.Where(p => p.Distance(targetPoint) <= 1));
                    break;
                case TargetsType.Cluster2:
                    points.AddRange(client.User.Map.Tiles.Keys.Where(p => p.Distance(targetPoint) <= 2));
                    break;
                case TargetsType.Cluster3:
                    points.AddRange(client.User.Map.Tiles.Keys.Where(p => p.Distance(targetPoint) <= 3));
                    break;
                case TargetsType.Screen:
                    points.AddRange(client.User.Map.Tiles.Keys.Where(p => p.Distance(targetPoint) <= 13));
                    break;
            }

            return points;
        }
    }
}
