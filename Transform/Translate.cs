using Unity.Mathematics;

namespace Heaj.Sam.MapUtilities
{
    public static partial class MapUtility<T>
    {
        public static void Translate(T[,] map, int2 offset, RepeatMode repeat = RepeatMode.Empty)
        {
            Translate(map, offset, repeat, EmptyValue);
        }

        public static void Translate(float[,] values, int2 offset, RepeatMode repeat = RepeatMode.Empty)
        {
            Translate(values, offset, repeat, 0.0f);
        }

        public static void Translate(float[,] values, int2 offset, RepeatMode repeat = RepeatMode.Empty, float defaultValue = 0.0f)
        {
            Translate(values, offset, repeat, defaultValue);
        }

        public static void Translate(bool[,] values, int2 offset, RepeatMode repeat = RepeatMode.Empty)
        {
            Translate(values, offset, repeat, false);
        }

        private static void Translate<U>(U[,] map, int2 offset, RepeatMode repeat, U emptyValue)
        {
            switch (repeat)
            {
                case RepeatMode.Empty: TranslateEmpty(map, offset, emptyValue); break;
                case RepeatMode.Clamp: TranslateClamp(map, offset); break;
                case RepeatMode.Repeat: TranslateRepeat(map, offset); break;
            }
        }

        private static void TranslateEmpty<U>(U[,] map, int2 offset, U emptyValue)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);
            U[,] copy = Copy(map);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int2 sourceCoord = new int2(x, y) + offset;
                    U type;
                    if (math.all(sourceCoord >= 0) && math.all(sourceCoord < new int2(width, height)))
                        type = copy[sourceCoord.x, sourceCoord.y];
                    else
                        type = emptyValue;
                    map[x, y] = type;
                }
            }
        }

        private static void TranslateClamp<U>(U[,] map, int2 offset)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);
            U[,] copy = Copy(map);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int2 sourceCoord = math.clamp(new int2(x, y) + offset, 0, new int2(width - 1, height - 1));
                    map[x, y] = copy[sourceCoord.x, sourceCoord.y];
                }
            }
        }

        private static void TranslateRepeat<U>(U[,] map, int2 offset)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);
            U[,] copy = Copy(map);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int2 sourceCoord = new int2(x, y) + offset;
                    sourceCoord %= new int2(width, height);
                    if (sourceCoord.x < 0)
                        sourceCoord.x += width;
                    if (sourceCoord.y < 0)
                        sourceCoord.y += height;
                    map[x, y] = copy[sourceCoord.x, sourceCoord.y];
                }
            }
        }
    }
}
