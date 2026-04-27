using System.Collections.Generic;

using Unity.Mathematics;

using UnityEngine;

namespace Heaj.Sam.MapUtilities
{
    public static partial class MapUtility<T>
    {
        /// <summary>
        /// Provides information about the island, such as the number of cells and its bounding box
        /// </summary>
        public static List<IslandData> GetIslandData(T[,] map, FloodFillData floodFill)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);

            //List cells
            Dictionary<int, IslandData> data = new Dictionary<int, IslandData>();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int id = floodFill[x, y];

                    if (!data.ContainsKey(id))
                    {
                        data.Add(id, new IslandData(id, new int2(x, y)));
                    }
                    else
                    {
                        IslandData island = data[id];
                        island.Add(new int2(x, y));
                        data[id] = island;
                    }
                }
            }

            //Compute final data
            List<IslandData> islands = new List<IslandData>();
            foreach (var kvp in data)
            {
                IslandData island = kvp.Value;
                island.boundsCenter = (island.boundsMin + island.boundsMax) / 2;
                island.boundsSize = island.boundsMax - island.boundsMin;
                island.weightedCenter = (int2) math.round(island.weightedCenter / (float2) island.cellCount);
                islands.Add(island);
            }
            return islands;
        }
    }
}
