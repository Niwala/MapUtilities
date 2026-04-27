using Unity.Mathematics;

using UnityEngine;

namespace Heaj.Sam.MapUtilities
{
    public struct FloodFillData
    {
        public int[,] ids;

        public int this[int2 index]
        {
            get => ids[index.x, index.y];
            set => ids[index.x, index.y] = value;
        }

        public int this[int x, int y]
        {
            get => ids[x, y];
            set => ids[x, y] = value;
        }

        public FloodFillData(int width, int height)
        {
            ids = new int[width, height];
        }
    }
}
