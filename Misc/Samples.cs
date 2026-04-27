using System.Collections.Generic;

using Unity.Mathematics;

using UnityEngine;

namespace Heaj.Sam.MapUtilities
{
    public static partial class MapUtility<T>
    {
        private static int2[] offsets = new int2[]
        {
            new int2(1, 0),
            new int2(0, 1),
            new int2(-1, 0),
            new int2(0, -1),
            new int2(-1, -1),
            new int2(-1, 1),
            new int2(1, -1),
            new int2(1, 1)
        };

        public static bool HasTypeAround(T[,] map, int2 cellID, T type, bool supportDiagonals = false)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);

            for (int i = 0; i < (supportDiagonals ? 8 : 4); i++)
            {
                int2 offset = offsets[i];
                int2 neighborID = cellID + offset;

                if (math.any(neighborID < 0) || math.any(neighborID >= new int2(width, height)))
                    continue;

                if (map[neighborID.x, neighborID.y].Equals(type))
                    return true;
            }
            return false;
        }

        public static int GetTypeCountAround(T[,] map, int2 cellID, T type, bool supportDiagonals = false)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);
            int count = 0;

            for (int i = 0; i < (supportDiagonals ? 8 : 4); i++)
            {
                int2 offset = offsets[i];
                int2 neighborID = cellID + offset;

                if (math.any(neighborID < 0) || math.any(neighborID >= new int2(width, height)))
                    continue;

                if (map[neighborID.x, neighborID.y].Equals(type))
                    count++;
            }
            return count;
        }

        public static IEnumerable<int2> GetTypeAround(T[,] map, int2 cellID, T type, bool supportDiagonals = false)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);

            for (int i = 0; i < (supportDiagonals ? 8 : 4); i++)
            {
                int2 offset = offsets[i];
                int2 neighborID = cellID + offset;

                if (math.any(neighborID < 0) || math.any(neighborID >= new int2(width, height)))
                    continue;

                if (map[neighborID.x, neighborID.y].Equals(type))
                    yield return neighborID;
            }
        }

        public static bool HasTypesTouchingAround(T[,] map, int2 cellID, T typeA, T typeB)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);

            for (int i = 0; i < 8; i++)
            {
                int2 neighborA = cellID + offsets[i];
                int2 neighborB = cellID + offsets[(i + 1) % 8];

                if (math.any(neighborA < 0) || math.any(neighborA >= new int2(width, height)) ||
                    math.any(neighborB < 0) || math.any(neighborB >= new int2(width, height)))
                    continue;

                if (map[neighborA.x, neighborA.y].Equals(typeA) && map[neighborB.x, neighborB.y].Equals(typeB) ||
                    map[neighborA.x, neighborA.y].Equals(typeB) && map[neighborB.x, neighborB.y].Equals(typeA))
                    return true;
            }

            return false;
        }
    }
}
