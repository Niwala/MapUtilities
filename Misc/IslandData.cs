using Unity.Mathematics;

namespace Heaj.Sam.MapUtilities
{
    public struct IslandData
    {
        public int islandID;
        public int cellCount;

        public int2 boundsMin;
        public int2 boundsMax;
        public float2 boundsCenter;
        public int2 boundsSize;

        public float2 weightedCenter;
        public int2 oneCell;

        public IslandData(int islandID, int2 firstCell)
        {
            this.islandID = islandID;

            boundsMin = firstCell;
            boundsMax = firstCell;
            cellCount = 1;

            boundsSize = default;
            boundsCenter = default;
            weightedCenter = default;
            oneCell = firstCell;
        }

        public void Add(int2 cell)
        {
            boundsMin = math.min(boundsMin, cell);
            boundsMax = math.max(boundsMax, cell);
            weightedCenter += cell;
            cellCount++;
        }
    }
}
