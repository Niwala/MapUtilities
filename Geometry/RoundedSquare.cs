using Unity.Mathematics;

using UnityEngine;

namespace Heaj.Sam.MapUtilities
{
    public static partial class MapUtility<T>
    {
        /// <summary>
        /// Draws a rounded square shape on map.
        /// </summary>
        /// <param name="map">Target map.</param>
        /// <param name="size">Normalized size [0,1].<br/>1 = fills the entire map.</param>
        /// <param name="roundness">Shape interpolation [0,1].<br/>0 = square, 1 = circle.</param>
        /// <param name="type">Cell type to write.</param>
        /// <param name="invert">If true, writes outside the shape instead of inside.</param>
        /// <param name="offset">Offset in cells from the center.</param>
        public static void DrawRoundedSquare(T[,] map, float size, float roundness, T type, bool invert = false, int2 offset = default)
        {
            int rows = map.GetLength(0);
            int cols = map.GetLength(1);

            //Map center
            float centerR = (rows - 1) / 2.0f + offset.y;
            float centerC = (cols - 1) / 2.0f + offset.x;

            //Half-extents in cells size. 1.0 fills the shorter axis entirely
            float halfExtent = size * math.min(rows, cols) / 2.0f;
            if (halfExtent <= 0.0f) 
                return;

            //Superellipse exponent:
            //  roundness 0 : exponent very large       behaves like a square (Chebyshev)
            //  roundness 1 : exponent 2                perfect circle
            float t = math.clamp(roundness, 0.0f, 1.0f);
            float exponent = t < 0.001f ? 1000.0f : 2.0f / t;

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    //Normalized distance from center along each axis [-1, 1]
                    float dr = (r - centerR) / halfExtent;
                    float dc = (c - centerC) / halfExtent;

                    //Superellipse equation: |x|^n + |y|^n <= 1
                    float superDist = math.pow(math.abs(dc), exponent)
                                     + math.pow(math.abs(dr), exponent);

                    bool inside = superDist <= 1.0;

                    if (inside != invert)
                        map[r, c] = type;
                }
            }
        }

        /// <summary>
        /// Draws a rounded square shape on map.
        /// </summary>
        /// <param name="map">Target map.</param>
        /// <param name="borderSize">Padding from each edges<br/>0 = fills the entire map.</param>
        /// <param name="roundness">Shape interpolation [0,1]<br/>0 = square, 1 = circle.</param>
        /// <param name="type">Cell type to write.</param>
        /// <param name="invert">If true, writes outside the shape instead of inside.</param>
        /// <param name="offset">Offset in cells from the center.</param>
        public static void DrawRoundedSquare(T[,] map, int borderSize, float roundness, T type, bool invert = false, int2 offset = default)
        {
            int rows = map.GetLength(0);
            int cols = map.GetLength(1);

            float halfExtentCells = math.min(rows, cols) / 2f - borderSize;
            if (halfExtentCells <= 0f) 
                return;

            float normalizedSize = halfExtentCells / (math.min(rows, cols) / 2f);
            DrawRoundedSquare(map, normalizedSize, roundness, type, invert, offset);
        }

        /// <summary>
        /// Returns a distance field of a rounded square shape.<br/>
        /// Values: 1 at center, 0 at the shape boundary, negative outside.<br/>
        /// </summary>
        /// <param name="map">Used only to read dimensions.</param>
        /// <param name="size">Normalized size [0,1]. Controls gradient steepness.</param>
        /// <param name="roundness">Shape interpolation [0,1]. 0 = square, 1 = circle.</param>
        /// <param name="offset">Offset in cells from the center.</param>
        public static float[,] DrawRoundedSquare(T[,] map, float size, float roundness, bool invert = false, int2 offset = default)
        {
            int rows = map.GetLength(0);
            int cols = map.GetLength(1);
            float[,] field = new float[rows, cols];

            float centerR = (rows - 1) / 2.0f + offset.y;
            float centerC = (cols - 1) / 2.0f + offset.x;

            float halfExtent = math.min(rows, cols) / 2.0f * math.clamp(size, 0.0001f, float.MaxValue);

            float t = math.clamp(roundness, 0.0f, 1.0f);
            float exponent = t < 0.001f ? 1000f : 2.0f / t;

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    float dr = (r - centerR) / halfExtent;
                    float dc = (c - centerC) / halfExtent;

                    float superDist = math.pow(math.abs(dc), exponent)
                                     + math.pow(math.abs(dr), exponent);

                    field[r, c] = invert ? superDist : (1.0f - (float)superDist);
                }
            }

            return field;
        }
    }
}