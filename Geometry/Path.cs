using System.Collections.Generic;

using Unity.Mathematics;

using UnityEngine;

namespace Heaj.Sam.MapUtilities
{
    public static partial class MapUtility<T>
    {
        public static bool DrawPath(T[,] map, int2 start, int2 end, T drawType, bool supportDiagonals, params T[] walkableTypes)
        {
            return DrawPath(map, start, end, drawType, supportDiagonals, false, walkableTypes);
        }

        public static bool DrawPath(T[,] map, int2 start, int2 end, T drawType, bool supportDiagonals, bool invertWalkables, params T[] walkableTypes)
        {
            int2 lastCell = start;
            foreach (var item in GetPath(map, start, end, supportDiagonals, invertWalkables, walkableTypes))
            {
                map[item.x, item.y] = drawType;
                lastCell = item;
            }
            return math.all(lastCell == end);
        }

        public static IEnumerable<int2> GetPath(T[,] map, int2 start, int2 end, bool supportDiagonals, params T[] walkableTypes)
            => GetPath(map, start, end, supportDiagonals, false, walkableTypes);

        public static IEnumerable<int2> GetPath(T[,] map, int2 start, int2 end, bool supportDiagonals, bool invertWalkables, params T[] walkableTypes)
        {
            int rows = map.GetLength(0);
            int cols = map.GetLength(1);
            int neighCount = supportDiagonals ? 8 : 4;

            //Walkable lookup
            var walkable = new HashSet<T>(walkableTypes, EqualityComparer<T>.Default);

            bool IsWalkable(int x, int y) =>
                x >= 0 && y >= 0 && x < rows && y < cols &&
                    (invertWalkables ? !walkable.Contains(map[x, y]) : walkable.Contains(map[x, y]));

            if (!IsWalkable(start.x, start.y) || !IsWalkable(end.x, end.y))
                yield break;

            //A* structures
            var gScore = new Dictionary<int2, float>();
            var fScore = new Dictionary<int2, float>();
            var cameFrom = new Dictionary<int2, int2>();
            var openSet = new SortedSet<(float f, int2 pos)>(Comparer<(float, int2)>.Create((a, b) =>
            {
                int cmp = a.Item1.CompareTo(b.Item1);
                if (cmp != 0) 
                    return cmp;

                cmp = a.Item2.x.CompareTo(b.Item2.x);
                return cmp != 0 ? cmp : a.Item2.y.CompareTo(b.Item2.y);
            }));

            gScore[start] = 0f;
            fScore[start] = Heuristic(start, end);
            openSet.Add((fScore[start], start));

            int[] dx = { -1, 1, 0, 0, -1, -1, 1, 1 };
            int[] dy = { 0, 0, -1, 1, -1, 1, -1, 1 };
            float[] moveCost = { 1f, 1f, 1f, 1f, 1.414f, 1.414f, 1.414f, 1.414f };

            while (openSet.Count > 0)
            {
                var (_, current) = openSet.Min;
                openSet.Remove(openSet.Min);

                if (current.Equals(end))
                {
                    foreach (int2 cell in ReconstructPath(cameFrom, current))
                        yield return cell;
                    yield break;
                }

                float currentG = gScore.GetValueOrDefault(current, float.MaxValue);

                for (int d = 0; d < neighCount; d++)
                {
                    int nx = current.x + dx[d];
                    int ny = current.y + dy[d];

                    if (!IsWalkable(nx, ny)) 
                        continue;

                    //Block diagonal movement if both adjacent cardinals are blocked
                    if (supportDiagonals && d >= 4)
                    {
                        if (!IsWalkable(current.x + dx[d], current.y) &&
                            !IsWalkable(current.x, current.y + dy[d]))
                            continue;
                    }

                    var neighbor = new int2(nx, ny);
                    float tentativeG = currentG + moveCost[d];

                    if (tentativeG < gScore.GetValueOrDefault(neighbor, float.MaxValue))
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentativeG;
                        float f = tentativeG + Heuristic(neighbor, end);
                        fScore[neighbor] = f;
                        openSet.Add((f, neighbor));
                    }
                }
            }
            //No path found
        }

        private static float Heuristic(int2 a, int2 b)
        {
            //Octile distance — admissible heuristic for 8-directional movement
            float dx = math.abs(a.x - b.x);
            float dy = math.abs(a.y - b.y);
            return Mathf.Max(dx, dy) + (1.414f - 1f) * Mathf.Min(dx, dy);
        }

        private static IEnumerable<int2> ReconstructPath(Dictionary<int2, int2> cameFrom, int2 current)
        {
            var path = new List<int2>();
            while (cameFrom.ContainsKey(current))
            {
                path.Add(current);
                current = cameFrom[current];
            }
            path.Add(current);
            path.Reverse();
            return path;
        }
    }
}
