namespace Heaj.Sam.MapUtilities
{
    public static partial class MapUtility<T>
    {
        public static void Rotate(T[,] map, RotateAmount rotate = RotateAmount._90)
        {
            Rotate<T>(map, rotate);
        }

        public static void Rotate(float[,] values, RotateAmount rotate = RotateAmount._90)
        {
            Rotate<float>(values, rotate);
        }

        public static void Rotate(bool[,] values, RotateAmount rotate = RotateAmount._90)
        {
            Rotate<bool>(values, rotate);
        }

        private static void Rotate<U>(U[,] map, RotateAmount rotate)
        {
            switch (rotate)
            {
                case RotateAmount._90: Rotate_90(map); break;
                case RotateAmount._180: Rotate_180(map); break;
                case RotateAmount._270: Rotate_270(map); break;
            }
        }

        private static void Rotate_90<U>(U[,] map)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);
            U[,] copy = Copy(map);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    map[x, y] = copy[y, width - x - 1];
                }
            }
        }

        private static void Rotate_180<U>(U[,] map)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);
            U[,] copy = Copy(map);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    map[x, y] = copy[width - x - 1, height - y - 1];
                }
            }
        }

        private static void Rotate_270<U>(U[,] map)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);
            U[,] copy = Copy(map);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    map[x, y] = copy[height - y - 1, x];
                }
            }
        }
    }
}