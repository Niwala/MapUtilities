using System.Collections.Generic;

using UnityEngine;

namespace Heaj.Sam.MapUtilities
{
    public static partial class MapUtility<T>
    {
        public static FloodFillData FloodFill(T[,] types, bool supportDiagonals, T emptyValue = default)
        {
            int width = types.GetLength(0);
            int height = types.GetLength(1);

            FloodFillData data = new FloodFillData(width, height);

            //-1 = unassigned
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    data[x, y] = -1;

            int currentId = 16;

            Queue<(int x, int y)> queue = new Queue<(int, int)>();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    //Skip void or already visited
                    if (types[x, y].Equals(EmptyValue) || data[x, y] != -1)
                        continue;

                    //Start new island
                    queue.Clear();
                    queue.Enqueue((x, y));
                    data[x, y] = currentId;

                    while (queue.Count > 0)
                    {
                        var (cx, cy) = queue.Dequeue();

                        //Neighbors (4-dir)
                        TryEnqueue(cx + 1, cy);
                        TryEnqueue(cx - 1, cy);
                        TryEnqueue(cx, cy + 1);
                        TryEnqueue(cx, cy - 1);

                        if (supportDiagonals)
                        {
                            TryEnqueue(cx + 1, cy + 1);
                            TryEnqueue(cx - 1, cy - 1);
                            TryEnqueue(cx + 1, cy - 1);
                            TryEnqueue(cx - 1, cy + 1);
                        }
                    }

                    currentId++;
                }
            }

            return data;

            //Local function
            void TryEnqueue(int x, int y)
            {
                //Bounds
                if (x < 0 || y < 0 || x >= width || y >= height)
                    return;

                //Skip void or visited
                if (types[x, y].Equals(EmptyValue) || data[x, y] != -1)
                    return;

                data[x, y] = currentId;
                queue.Enqueue((x, y));
            }
        }

    }
}
