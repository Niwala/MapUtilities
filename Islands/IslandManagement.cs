using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Heaj.Sam.MapUtilities
{
    public static partial class MapUtility<T>
    {
        /// <summary>
        /// Removes the island corresponding to the given ID.
        /// </summary>
        /// <param name="map">The map containing the island types.</param>
        /// <param name="ids">FloodFill data.</param>
        /// <param name="idToRemove">The ID of the island to remove.</param>
        public static void RemoveIsland(T[,] map, int[,] ids, int idToRemove)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (ids[x, y] == idToRemove)
                    {
                        ids[x, y] = -1;
                        map[x, y] = EmptyValue;
                    }
                }
            }
        }

        /// <summary>
        /// Removes the islands corresponding to the given IDs.
        /// </summary>
        /// <param name="map">The map containing the island types.</param>
        /// <param name="ids">FloodFill data.</param>
        /// <param name="idsToRemove">The IDs of the islands to remove.</param>
        public static void RemoveIslands(T[,] types, int[,] ids, List<int> idsToRemove)
        {
            HashSet<int> set = idsToRemove.ToHashSet();
            int width = types.GetLength(0);
            int height = types.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int id = ids[x, y];
                    if (id != -1 && set.Contains(id))
                    {
                        ids[x, y] = -1;
                        types[x, y] = EmptyValue;
                    }
                }
            }
        }

        /// <summary>
        /// Convert an entire island to the given type.
        /// </summary>
        /// <param name="map">The map containing the island types.</param>
        /// <param name="ids">FloodFill data.</param>
        /// <param name="islandId">The IDs of the islands to modify.</param>
        /// <param name="toType">Target type</param>
        public static void ReplaceIslandType(T[,] map, int[,] ids, int islandId, T toType)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (ids[x, y] == islandId)
                    {
                        if (toType.Equals(EmptyValue))
                            ids[x, y] = -1;
                        map[x, y] = toType;
                    }
                }
            }
        }

        /// <summary>
        /// Convert the cells of a certain type in a given island to another type.
        /// </summary>
        /// <param name="map">The map containing the island types.</param>
        /// <param name="ids">FloodFill data.</param>
        /// <param name="islandId">The IDs of the islands to modify.</param>
        /// <param name="fromType">The type of cells to be replaced.</param>
        /// <param name="toType">Target type</param>
        public static void ReplaceIslandType(T[,] map, int[,] ids, int islandId, T fromType, T toType)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (ids[x, y] == islandId && map[x, y].Equals(fromType))
                    {
                        if (toType.Equals(EmptyValue))
                            ids[x, y] = -1;
                        map[x, y] = toType;
                    }
                }
            }
        }
    }
}
