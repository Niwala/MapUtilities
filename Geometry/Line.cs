using System.Collections.Generic;

using Unity.Mathematics;

using UnityEngine;

namespace Heaj.Sam.MapUtilities
{
    public static partial class MapUtility<T>
    {
        /// <summary>
        /// Draw a line connecting a and b.<br/>
        /// Can draw diagonals.
        /// </summary>
        /// <param name="map">The map that will be modified.</param>
        /// <param name="a">The starting point of the line (expressed as a cell coordinate).</param>
        /// <param name="b">The endpoint of the line (expressed as a cell coordinate).</param>
        /// <param name="type">The type in which the line should be drawn.</param>
        /// <param name="drawMode">Determines which cells should be replaced. Defaults to DrawMode.Everything.</param>
        public static void DrawLine(T[,] map, int2 a, int2 b, T type, DrawMode drawMode = DrawMode.Everything)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);

            foreach (var cell in GetLine(a, b))
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
        /// Draw up to two lines at right angles to connect a and b.<br/>
        /// Reversing the points allows you to reverse the direction of the bend.
        /// </summary>
        /// <param name="map">The map that will be modified.</param>
        /// <param name="a">The starting point of the line (expressed as a cell coordinate).</param>
        /// <param name="b">The endpoint of the line (expressed as a cell coordinate).</param>
        /// <param name="type">The type in which the line should be drawn.</param>
        /// <param name="drawMode">Determines which cells should be replaced. Defaults to DrawMode.Everything.</param>
        public static void DrawLShape(T[,] map, int2 a, int2 b, T type, DrawMode drawMode = DrawMode.Everything)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);

            foreach (var cell in GetLShape(a, b))
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
        /// List all the cells that connect point A to point B.
        /// </summary>
        /// <param name="a">The starting point of the line (expressed as a cell coordinate).</param>
        /// <param name="b">The endpoint of the line (expressed as a cell coordinate).</param>
        public static IEnumerable<int2> GetLine(int2 a, int2 b)
        {
            int x = a.x;
            int y = a.y;

            int dx = math.abs(b.x - a.x);
            int dy = math.abs(b.y - a.y);

            int sx = a.x < b.x ? 1 : -1;
            int sy = a.y < b.y ? 1 : -1;

            int err = dx - dy;

            while (true)
            {
                yield return new int2(x, y);

                //End
                if (x == b.x && y == b.y)
                    break;

                int e2 = err * 2;

                //No diagonal: choose dominant axis first
                if (e2 > -dy)
                {
                    err -= dy;
                    x += sx;
                }
                else
                {
                    err += dx;
                    y += sy;
                }
            }
        }

        /// <summary>
        /// List all the cells that connect a and b via two lines at right angles.<br/>
        /// Reversing the points allows you to reverse the direction of the bend.
        /// </summary>
        /// <param name="a">The starting point of the line (expressed as a cell coordinate).</param>
        /// <param name="b">The endpoint of the line (expressed as a cell coordinate).</param>
        public static IEnumerable<int2> GetLShape(int2 a, int2 b)
        {
            int x = a.x;
            int y = a.y;

            int sx = a.x < b.x ? 1 : -1;
            int sy = a.y < b.y ? 1 : -1;

            //x
            while (x != b.x)
            {
                yield return new int2(x, y);
                x += sx;
            }

            //y
            while (y != b.y)
            {
                yield return new int2(x, y);
                y += sy;
            }

            yield return b;
        }
    }
}
