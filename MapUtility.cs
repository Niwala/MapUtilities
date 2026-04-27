
using UnityEngine;
using Unity.Mathematics;

using System.Collections.Generic;
using System;
using System.Linq;

namespace Heaj.Sam.MapUtilities
{
    public static partial class MapUtility<T> where T : struct, Enum
    {
        private static T EmptyValue => default;

        //TODO : Add Kruskal's algorithm & border cleaning


        public static List<IslandConnection> ComputeIslandConnections(T[,] types, FloodFillData ids)
        {
            int width = types.GetLength(0);
            int height = types.GetLength(1);

            //Run JFA
            JumpFloodingData data = JumpFlood(types, ids);

            var connections = new Dictionary<int2, IslandConnection>();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    //Only void cells
                    if (ids[x, y] != -1)
                        continue;

                    //Right neighbor
                    TryProcess(x, y, x + 1, y);

                    //Top neighbor
                    TryProcess(x, y, x, y + 1);
                }
            }

            return connections.Values.ToList();

            void TryProcess(int x0, int y0, int x1, int y1)
            {
                //Bounds
                if (x1 >= width || y1 >= height)
                    return;

                //Both must be void
                if (ids[x1, y1] != -1)
                    return;

                int idA = data.ids[x0, y0];
                int idB = data.ids[x1, y1];

                //Invalid or same island
                if (idA == -1 || idB == -1 || idA == idB)
                    return;

                int2 key = OrderedKey(idA, idB);
                int2 posA = data.ids[x0, y0];
                int2 posB = data.ids[x1, y1];

                float dist = math.distancesq(posA, posB);

                if (connections.TryGetValue(key, out var existing))
                {
                    if (dist >= existing.distance)
                        return;
                }

                connections[key] = new IslandConnection
                {
                    idA = key.x,
                    idB = key.y,
                    positionA = new int2(posA.x, posA.y),
                    positionB = new int2(posB.x, posB.y),
                    distance = dist
                };
            }

            int2 OrderedKey(int a, int b)
            {
                return a < b ? new int2(a, b) : new int2(b, a);
            }
        }

        public static void Grow(T[,] map, FloodFillData floodFill, JumpFloodingData jumpFloodingData)
        {
            Dictionary<int, float> islandMaxRange = new Dictionary<int, float>();

            List<IslandConnection> connections = ComputeIslandConnections(map, floodFill);

            void ConnectionToMaxRange(int islandID, float distance)
            {
                if (islandMaxRange.ContainsKey(islandID))
                    islandMaxRange[islandID] = math.min(islandMaxRange[islandID], distance);
                else
                    islandMaxRange.Add(islandID, distance);
            }

            for (int i = 0; i < connections.Count; i++)
            {
                IslandConnection connection = connections[i];
                ConnectionToMaxRange(connection.idA, connection.distance / 2);
                ConnectionToMaxRange(connection.idB, connection.distance / 2);
            }

            foreach (var cell in GetAllCells(map))
            {
                if (!map[cell.x, cell.y].Equals(EmptyValue))
                    continue;

                int islandId = jumpFloodingData.ids[cell.x, cell.y];

                //Attach to closest island
                if (jumpFloodingData.distances[cell.x, cell.y] < 1)
                {
                    int2 coords = jumpFloodingData.coords[cell.x, cell.y];
                    map[cell.x, cell.y] = map[coords.x, coords.y];
                }
            }
        }



        public struct IslandDistances
        {
            public int idA;
            public int2 cellA;
            public float distanceA;

            public int idB;
            public int2 cellB;
            public float distanceB;
        }

        public struct IslandConnection
        {
            public int2 positionA;
            public int idA;
            public int2 positionB;
            public int idB;
            public float distance;
        }
    }
}