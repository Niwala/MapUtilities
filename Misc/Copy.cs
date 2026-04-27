using UnityEngine;

namespace Heaj.Sam.MapUtilities
{
    public static partial class MapUtility<T>
    {
        public static T[,] Copy(T[,] map)
        {
            return Copy<T>(map);
        }

        public static float[,] Copy(float[,] values)
        {
            return Copy<float>(values);
        }

        public static bool[,] Copy(bool[,] values)
        {
            return Copy<bool>(values);
        }

        private static U[,] Copy<U>(U[,] values)
        {
            int width = values.GetLength(0);
            int height = values.GetLength(1);
            U[,] copy = new U[width, height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    copy[x, y] = values[x, y];
                }
            }
            return copy;
        }
    }
}