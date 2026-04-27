using System.Collections.Generic;

using Unity.Mathematics;

using UnityEngine;

namespace Heaj.Sam.MapUtilities
{
    public static partial class MapUtility<T>
    {
        /// <summary>
        /// Draw a cubic curve connecting a and d with tangents b and c.<br/>
        /// </summary>
        /// <param name="map">The map that will be modified.</param>
        /// <param name="a">The starting point of the line (expressed as in grid coordinate).</param>
        /// <param name="b">The output tangent at point a (expressed as grid coordinate).</param>
        /// <param name="c">The input tangent at point d (expressed as grid coordinate).</param>
        /// <param name="d">The endpoint of the line (expressed as in grid coordinate).</param>
        /// <param name="type">The type in which the line should be drawn.</param>
        /// <param name="drawMode">Determines which cells should be replaced. Defaults to DrawMode.Everything.</param>
        public static void DrawCurve(T[,] map, int2 a, float2 b, float2 c, int2 d, T type, DrawMode drawMode = DrawMode.Everything)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);

            foreach (var cell in GetCurve(a, b, c, d))
            {
                if (math.any(cell < 0) || math.any(cell >= new int2(width, height)))
                    continue;

                bool isEmpty = map[cell.x, cell.y].Equals(EmptyValue);

                switch (drawMode)
                {
                    case DrawMode.Everything:
                        map[cell.x, cell.y] = type;
                        break;

                    case DrawMode.DrawOnEmptyOnly:
                        if (isEmpty)
                            map[cell.x, cell.y] = type;
                        break;

                    case DrawMode.DrawOnOccupiedOnly:
                        if (!isEmpty)
                            map[cell.x, cell.y] = type;
                        break;
                }
            }
        }

        /// <summary>
        /// Returns all coordinates between cells a and d with tangents b and c.<br/>
        /// </summary>
        /// <param name="a">The starting point of the line (expressed as in grid coordinate).</param>
        /// <param name="b">The output tangent at point a (expressed as grid coordinate).</param>
        /// <param name="c">The input tangent at point d (expressed as grid coordinate).</param>
        /// <param name="d">The endpoint of the line (expressed as in grid coordinate).</param>
        public static IEnumerable<int2> GetCurve(int2 a, float2 b, float2 c, int2 d)
        {
            //Enough steps so that no two consecutive samples are more than ~1 cell apart.
            float polygonLength = math.length((float2)a - b)
                                + math.length(b - c)
                                + math.length(c - (float2)d);
            int steps = math.max(1, (int)math.ceil(polygonLength));

            int2 previous = a;
            bool firstPoint = true;

            for (int i = 0; i <= steps; i++)
            {
                float t = i / (float)steps;

                float mt = 1f - t;
                float mt2 = mt * mt;
                float mt3 = mt2 * mt;
                float t2 = t * t;
                float t3 = t2 * t;

                float2 point = mt3 * (float2)a
                             + 3f * mt2 * t * b
                             + 3f * mt * t2 * c
                             + t3 * (float2)d;

                int2 cell = new int2((int)math.round(point.x), (int)math.round(point.y));

                //Bridge any gap between samples with GetLine to avoid skipped cells
                if (firstPoint)
                {
                    yield return cell;
                    firstPoint = false;
                }
                else if (!cell.Equals(previous))
                {
                    //Skip first point of GetLine to avoid duplicates
                    bool skip = true;

                    foreach (int2 lineCell in GetLine(previous, cell))
                    {
                        if (skip) 
                        { 
                            skip = false;
                            continue; 
                        }
                        yield return lineCell;
                    }
                }

                previous = cell;
            }
        }
    }
}
