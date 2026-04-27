using Unity.Mathematics;

using UnityEditor;

using UnityEngine;

namespace Heaj.Sam.MapUtilities
{
    public static partial class MapUtility<T>
    {
        //Internal
        private static JumpFloodingData JumpFlood(T[,] map)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);
            JumpFloodingData data = new JumpFloodingData(width, height);

            //Init
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (!map[x, y].Equals(EmptyValue))
                    {
                        //Seed
                        data.coords[x, y] = new int2(x, y);
                    }
                    else
                    {
                        //Empty
                        data.coords[x, y] = new int2(-1, -1);
                    }
                }
            }

            int maxDim = Mathf.Max(width, height);
            int step = 1;

            //Find highest power of 2 <= maxDim
            while (step < maxDim)
                step <<= 1;
            step >>= 1;

            //Temp buffers
            int2[,] tempPos = new int2[width, height];

            //JFA passes
            while (step > 0)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        int2 center = new int2(x, y);
                        int2 bestPos = data.coords[x, y];
                        float bestDist = bestPos.x == -1 ? float.MaxValue : math.distancesq(center, bestPos);

                        //Check 8 directions
                        for (int dx = -1; dx <= 1; dx++)
                        {
                            for (int dy = -1; dy <= 1; dy++)
                            {
                                int nx = x + dx * step;
                                int ny = y + dy * step;

                                if (nx < 0 || ny < 0 || nx >= width || ny >= height)
                                    continue;

                                int2 npos = data.coords[nx, ny];
                                if (npos.x == -1)
                                    continue;

                                float dist = math.distancesq(center, npos);

                                if (dist < bestDist)
                                {
                                    bestDist = dist;
                                    bestPos = npos;
                                }
                            }
                        }

                        tempPos[x, y] = bestPos;
                    }
                }

                //Swap buffers
                var swapPos = data.coords;
                data.coords = tempPos;
                tempPos = swapPos;

                step >>= 1;
            }

            return data;
        }

        private static JumpFloodingData JumpFlood(T[,] map, int[,] ids)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);
            JumpFloodingData data = new JumpFloodingData(width, height);

            //Init
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (ids[x, y] != -1)
                    {
                        //Seed
                        data.ids[x, y] = ids[x, y];
                        data.coords[x, y] = new int2(x, y);
                    }
                    else
                    {
                        //Void
                        data.ids[x, y] = -1;
                        data.coords[x, y] = new int2(-1, -1);
                    }
                }
            }

            int maxDim = Mathf.Max(width, height);
            int step = 1;

            //Find highest power of 2 <= maxDim
            while (step < maxDim)
                step <<= 1;
            step >>= 1;

            //Temp buffers
            int[,] tempId = new int[width, height];
            int2[,] tempPos = new int2[width, height];

            //JFA passes
            while (step > 0)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        int2 center = new int2(x, y);
                        int bestId = data.ids[x, y];
                        int2 bestPos = data.coords[x, y];
                        float bestDist = bestId == -1 ? float.MaxValue : math.distancesq(center, bestPos);

                        //Check 8 directions
                        for (int dx = -1; dx <= 1; dx++)
                        {
                            for (int dy = -1; dy <= 1; dy++)
                            {
                                int nx = x + dx * step;
                                int ny = y + dy * step;

                                if (nx < 0 || ny < 0 || nx >= width || ny >= height)
                                    continue;

                                int nid = data.ids[nx, ny];
                                if (nid == -1)
                                    continue;

                                int2 npos = data.coords[nx, ny];
                                float dist = math.distancesq(center, npos);

                                if (dist < bestDist)
                                {
                                    bestDist = dist;
                                    bestId = nid;
                                    bestPos = npos;
                                }
                            }
                        }

                        tempId[x, y] = bestId;
                        tempPos[x, y] = bestPos;
                    }
                }

                //Swap buffers
                var swapId = data.ids;
                data.ids = tempId;
                tempId = swapId;

                var swapPos = data.coords;
                data.coords = tempPos;
                tempPos = swapPos;

                step >>= 1;
            }

            return data;
        }

        private static JumpFloodingData JumpFlood(T[,] map, bool[,] mask)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);
            JumpFloodingData data = new JumpFloodingData(width, height);

            //Init
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (mask[x, y])
                    {
                        data.ids[x, y] = 1;
                        data.coords[x, y] = new int2(x, y);
                    }
                    else
                    {
                        data.ids[x, y] = 0;
                        data.coords[x, y] = new int2(-1, -1);
                    }
                }
            }

            int maxDim = Mathf.Max(width, height);
            int step = 1;

            //Find highest power of 2 <= maxDim
            while (step < maxDim)
                step <<= 1;
            step >>= 1;

            //Temp buffers
            int[,] tempId = new int[width, height];
            int2[,] tempPos = new int2[width, height];

            //JFA passes
            while (step > 0)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        int2 center = new int2(x, y);
                        int bestId = data.ids[x, y];
                        int2 bestPos = data.coords[x, y];
                        float bestDist = bestId == -1 ? float.MaxValue : math.distancesq(center, bestPos);

                        //Check 8 directions
                        for (int dx = -1; dx <= 1; dx++)
                        {
                            for (int dy = -1; dy <= 1; dy++)
                            {
                                int nx = x + dx * step;
                                int ny = y + dy * step;

                                if (nx < 0 || ny < 0 || nx >= width || ny >= height)
                                    continue;

                                int nid = data.ids[nx, ny];
                                if (nid == -1)
                                    continue;

                                int2 npos = data.coords[nx, ny];
                                float dist = math.distancesq(center, npos);

                                if (dist < bestDist)
                                {
                                    bestDist = dist;
                                    bestId = nid;
                                    bestPos = npos;
                                }
                            }
                        }

                        tempId[x, y] = bestId;
                        tempPos[x, y] = bestPos;
                    }
                }

                //Swap buffers
                var swapId = data.ids;
                data.ids = tempId;
                tempId = swapId;

                var swapPos = data.coords;
                data.coords = tempPos;
                tempPos = swapPos;

                step >>= 1;
            }

            return data;
        }


        //Exposed
        public static JumpFloodingData JumpFlood(T[,] map, DistanceMode distanceMode = DistanceMode.Euclidian, float distanceScale = 1.0f, bool scaleDistanceByMapSize = true)
        {
            JumpFloodingData data = JumpFlood(map);
            data.RecomputeDistances(distanceMode, distanceScale, scaleDistanceByMapSize);
            return data;
        }

        public static JumpFloodingData JumpFlood(T[,] map, FloodFillData floodFill, DistanceMode distanceMode = DistanceMode.Euclidian, float distanceScale = 1.0f, bool scaleDistanceByMapSize = true)
            => JumpFlood(map, floodFill.ids, distanceMode, distanceScale, scaleDistanceByMapSize);

        public static JumpFloodingData JumpFlood(T[,] map, int[,] ids, DistanceMode distanceMode = DistanceMode.Euclidian, float distanceScale = 1.0f, bool scaleDistanceByMapSize = true)
        {
            JumpFloodingData data = JumpFlood(map, ids);
            data.RecomputeDistances(distanceMode, distanceScale, scaleDistanceByMapSize);
            return data;
        }

        public static JumpFloodingData JumpFlood(T[,] map, bool[,] mask, DistanceMode distanceMode = DistanceMode.Euclidian, float distanceScale = 1.0f, bool scaleDistanceByMapSize = true)
        {
            JumpFloodingData data = JumpFlood(map, mask);
            data.RecomputeDistances(distanceMode, distanceScale, scaleDistanceByMapSize);
            return data;
        }
    }
}