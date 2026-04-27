using Unity.Mathematics;

namespace Heaj.Sam.MapUtilities
{
    public struct IslandConnection
    {
        public int2 positionA;
        public int idA;
        public int2 positionB;
        public int idB;
        public float distance;
    }
}