using EloBuddy;
using EloBuddy.SDK;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vEvade
{
    static class Extensions
    {
        public static List<Vector2> GetWaypoints(this Obj_AI_Base unit)
        {
            var result = new List<Vector2>();

            if (unit.IsVisible)
            {
                result.Add(unit.ServerPosition.To2D());
                var path = unit.Path;
                if (path.Length > 0)
                {
                    var first = path[0].To2D();
                    if (first.Distance(result[0], true) > 40)
                    {
                        result.Add(first);
                    }

                    for (int i = 1; i < path.Length; i++)
                    {
                        result.Add(path[i].To2D());
                    }
                }
            }
            else if (WaypointTracker.StoredPaths.ContainsKey(unit.NetworkId))
            {
                var path = WaypointTracker.StoredPaths[unit.NetworkId];
                var timePassed = (Utils.TickCount - WaypointTracker.StoredTick[unit.NetworkId]) / 1000f;
                if (path.PathLength() >= unit.MoveSpeed * timePassed)
                {
                    result = CutPath(path, (int)(unit.MoveSpeed * timePassed));
                }
            }

            return result;
        }

        public static List<Vector2> CutPath(this List<Vector2> path, float distance)
        {
            var result = new List<Vector2>();
            var Distance = distance;
            if (distance < 0)
            {
                path[0] = path[0] + distance * (path[1] - path[0]).Normalized();
                return path;
            }

            for (var i = 0; i < path.Count - 1; i++)
            {
                var dist = path[i].Distance(path[i + 1]);
                if (dist > Distance)
                {
                    result.Add(path[i] + Distance * (path[i + 1] - path[i]).Normalized());
                    for (var j = i + 1; j < path.Count; j++)
                    {
                        result.Add(path[j]);
                    }

                    break;
                }
                Distance -= dist;
            }
            return result.Count > 0 ? result : new List<Vector2> { path.Last() };
        }
    }
    internal static class WaypointTracker
    {
        public static readonly Dictionary<int, List<Vector2>> StoredPaths = new Dictionary<int, List<Vector2>>();
        public static readonly Dictionary<int, int> StoredTick = new Dictionary<int, int>();
    }

    internal static class SGeometry
    {
        public static float PathLength(this List<Vector2> path)
        {
            var distance = 0f;
            for (var i = 0; i < path.Count - 1; i++)
            {
                distance += path[i].Distance(path[i + 1]);
            }
            return distance;
        }
    }

    public class SharpGeometry
    {
        public static Vector2[] CircleCircleIntersection(Vector2 center1, Vector2 center2, float radius1, float radius2)
        {
            var D = center1.Distance(center2);
            //The Circles dont intersect:
            if (D > radius1 + radius2 || (D <= Math.Abs(radius1 - radius2)))
            {
                return new Vector2[] { };
            }

            var A = (radius1 * radius1 - radius2 * radius2 + D * D) / (2 * D);
            var H = (float)Math.Sqrt(radius1 * radius1 - A * A);
            var Direction = (center2 - center1).Normalized();
            var PA = center1 + A * Direction;
            var S1 = PA + H * Direction.Perpendicular();
            var S2 = PA - H * Direction.Perpendicular();
            return new[] { S1, S2 };
        }
    }

    static class EnumerableExtensions
    {
        public static T MinOrDefault<T, R>(this IEnumerable<T> container, Func<T, R> valuingFoo) where R : IComparable
        {
            var enumerator = container.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                return default(T);
            }

            var minElem = enumerator.Current;
            var minVal = valuingFoo(minElem);

            while (enumerator.MoveNext())
            {
                var currVal = valuingFoo(enumerator.Current);

                if (currVal.CompareTo(minVal) < 0)
                {
                    minVal = currVal;
                    minElem = enumerator.Current;
                }
            }

            return minElem;
        }
    }
}