namespace Heaj.Sam.MapUtilities
{
    public static partial class MapUtility<T>
    {
        public static void Flip(T[,] map, Axis axis = Axis.Horizontal)
        {
            Flip<T>(map, axis);
        }

        public static void Flip(float[,] values, Axis axis = Axis.Horizontal)
        {
            Flip<float>(values, axis);
        }

        public static void Flip(bool[,] values, Axis axis = Axis.Horizontal)
        {
            Flip<bool>(values, axis);
        }

        private static void Flip<U>(U[,] map, Axis axis)
        {
            switch (axis)
            {
                case Axis.Horizontal: FlipHorizontal(map); break;
                case Axis.Vertical: FlipVertical(map); break;
            }
        }

        private static void FlipHorizontal<U>(U[,] map)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);
            U[,] copy = Copy(map);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    map[x, y] = copy[width - x - 1, y];
                }
            }
        }

        private static void FlipVertical<U>(U[,] map)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);
            U[,] copy = Copy(map);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    map[x, y] = copy[x, height - y - 1];
                }
            }
        }
    }
}