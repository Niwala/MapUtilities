using Unity.Mathematics;

using UnityEngine;

namespace Heaj.Sam.MapUtilities
{
    public static partial class MapUtility<T>
    {
        /// <summary>
        /// Horizontal : Mirror the left side onto the right side of the map.
        /// Vertical : Mirror the bottom side onto the top side of the map.
        /// Corner : Mirror the left-bottom corner to the other corners of the map.
        /// </summary>
        public static void Mirror(T[,] map, MirrorAxis axis = MirrorAxis.Horizontal)
        {
            Mirror<T>(map, axis);
        }

        /// <summary>
        /// Horizontal : Mirror the left side onto the right side of the map.
        /// Vertical : Mirror the bottom side onto the top side of the map.
        /// Corner : Mirror the left-bottom corner to the other corners of the map.
        /// </summary>
        public static void Mirror(float[,] values, MirrorAxis axis = MirrorAxis.Horizontal)
        {
            Mirror<float>(values, axis);
        }

        /// <summary>
        /// Horizontal : Mirror the left side onto the right side of the map.
        /// Vertical : Mirror the bottom side onto the top side of the map.
        /// Corner : Mirror the left-bottom corner to the other corners of the map.
        /// </summary>
        public static void Mirror(bool[,] values, MirrorAxis axis = MirrorAxis.Horizontal)
        {
            Mirror<bool>(values, axis);
        }

        private static void Mirror<U>(U[,] map, MirrorAxis axis)
        {
            switch (axis)
            {
                case MirrorAxis.Horizontal: MirrorHorizontal(map); break;
                case MirrorAxis.Vertical: MirrorVertical(map); break;
                case MirrorAxis.Corner: MirrorBoth(map); break;
            }
        }

        private static void MirrorHorizontal<U>(U[,] map)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);
            int half = width / 2;

            U[,] copy = Copy(map);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int2 sourceCoord = new int2(width - (math.abs(x - half) + half), y);
                    if (sourceCoord.x < 0 || sourceCoord.x >= width)
                        map[x, y] = default;
                    else
                        map[x, y] = copy[sourceCoord.x, sourceCoord.y];
                }
            }
        }

        private static void MirrorVertical<U>(U[,] map)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);
            int half = height / 2;

            U[,] copy = Copy(map);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int2 sourceCoord = new int2(x, height - (math.abs(y - half) + half));
                    if (sourceCoord.y < 0 || sourceCoord.y >= height)
                        map[x, y] = default;
                    else
                        map[x, y] = copy[sourceCoord.x, sourceCoord.y];
                }
            }
        }

        private static void MirrorBoth<U>(U[,] map)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);
            int halfWidth = width / 2;
            int halfHeight = height / 2;

            U[,] copy = Copy(map);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int2 sourceCoord = new int2(width - (math.abs(x - halfWidth) + halfWidth), height - (math.abs(y - halfHeight) + halfHeight));
                    if (sourceCoord.y < 0 || sourceCoord.y >= height || sourceCoord.y < 0 || sourceCoord.y >= height)
                        map[x, y] = default;
                    else
                        map[x, y] = copy[sourceCoord.x, sourceCoord.y];
                }
            }
        }
    }
}