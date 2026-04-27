using Unity.Mathematics;

using UnityEngine;

namespace Heaj.Sam.MapUtilities
{
    public struct JumpFloodingData
    {
        public int[,] ids;
        public int2[,] coords;
        public float[,] distances;

        public JumpFloodingData(int width, int height)
        {
            ids = new int[width, height];
            coords = new int2[width, height];
            distances = new float[width, height];
        }

        public int GetID(int2 cellCoords)
        {
            return ids[cellCoords.x, cellCoords.y];
        }

        public int GetID(int x, int y)
        {
            return ids[x, y];
        }

        public int2 GetCoords(int2 cellCoords)
        {
            return coords[cellCoords.x, cellCoords.y];
        }

        public int2 GetCoords(int x, int y)
        {
            return coords[x, y];
        }

        public float GetDistance(int2 cellCoords)
        {
            return distances[cellCoords.x, cellCoords.y];
        }

        public float GetDistance(int x, int y)
        {
            return distances[x, y];
        }
        public void GetData(int2 cellCoords, out int id, out int2 coords, out float distance)
        {
            id = ids[cellCoords.x, cellCoords.y];
            coords = this.coords[cellCoords.x, cellCoords.y];
            distance = distances[cellCoords.x, cellCoords.y];
        }

        public void GetData(int x, int y, out int id, out int2 coords, out float distance)
        {
            id = ids[x, y];
            coords = this.coords[x, y];
            distance = distances[x, y];
        }

        public void RecomputeDistances(DistanceMode distanceMode, float distanceScale, bool scaleDistanceByMapSize)
        {
            if (distanceMode == DistanceMode.None)
                return;

            int width = ids.GetLength(0);
            int height = ids.GetLength(1);

            //Distance factor
            float distanceFactor = distanceScale;
            if (scaleDistanceByMapSize)
                distanceFactor = distanceScale * math.rcp(math.sqrt(width * height));

            //Compute distances
            switch (distanceMode)
            {
                case DistanceMode.Euclidian:
                    for (int y = 0; y < height; y++)
                        for (int x = 0; x < width; x++)
                            distances[x, y] = math.distance(new int2(x, y), coords[x, y]) * distanceFactor;
                    break;

                case DistanceMode.Chebyshev:
                    for (int y = 0; y < height; y++)
                        for (int x = 0; x < width; x++)
                        {
                            float2 dist = math.abs(new int2(x, y) - coords[x, y]);
                            distances[x, y] = math.max(dist.x, dist.y) * distanceFactor;
                        }
                    break;

                case DistanceMode.Manhattan:
                    for (int y = 0; y < height; y++)
                        for (int x = 0; x < width; x++)
                        {
                            float2 dist = math.abs(new int2(x, y) - coords[x, y]);
                            distances[x, y] = (dist.x + dist.y) * distanceFactor;
                        }
                    break;
            }
        }
    }
}
