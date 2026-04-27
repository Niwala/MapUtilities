using System.Collections.Generic;

using Unity.Mathematics;

using UnityEngine;

namespace Heaj.Sam.MapUtilities
{
    public static partial class MapUtility<T>
    {
        public static List<(int2 a, int2 b)> TriangulateLines(List<int2> inputPoints)
        {
            var edges = new List<(int2 a, int2 b)>();
            if (inputPoints == null || inputPoints.Count < 2) 
                return edges;

            if (inputPoints.Count == 2)
                return new List<(int2 a, int2 b)> { (inputPoints[0], inputPoints[1]) };

            var points = ToFloat2List(inputPoints);
            var triangles = Bowyer_Watson(points);

            var seen = new HashSet<long>();
            foreach (var tri in triangles)
            {
                AddEdge(edges, seen, inputPoints[tri.a], inputPoints[tri.b]);
                AddEdge(edges, seen, inputPoints[tri.b], inputPoints[tri.c]);
                AddEdge(edges, seen, inputPoints[tri.c], inputPoints[tri.a]);
            }
            return edges;
        }

        public static (int2[] points, int[] triangles) TriangulateMesh(List<int2> inputPoints)
        {
            if (inputPoints == null || inputPoints.Count < 3)
                return (System.Array.Empty<int2>(), System.Array.Empty<int>());

            var points = ToFloat2List(inputPoints);
            var tris = Bowyer_Watson(points);

            var triIndices = new int[tris.Count * 3];
            for (int i = 0; i < tris.Count; i++)
            {
                triIndices[i * 3 + 0] = tris[i].a;
                triIndices[i * 3 + 1] = tris[i].b;
                triIndices[i * 3 + 2] = tris[i].c;
            }

            var pointArray = new int2[inputPoints.Count];
            for (int i = 0; i < inputPoints.Count; i++) pointArray[i] = inputPoints[i];

            return (pointArray, triIndices);
        }

        private struct Triangle
        {
            public int a, b, c;
            public Triangle(int a, int b, int c) { this.a = a; this.b = b; this.c = c; }
        }

        private static List<Triangle> Bowyer_Watson(List<float2> pts)
        {
            //find bounding box
            float minX = pts[0].x, minY = pts[0].y;
            float maxX = minX, maxY = minY;
            foreach (var p in pts)
            {
                if (p.x < minX) minX = p.x; if (p.x > maxX) maxX = p.x;
                if (p.y < minY) minY = p.y; if (p.y > maxY) maxY = p.y;
            }

            //Build super-triangle
            float dx = maxX - minX, dy = maxY - minY;
            float delta = math.max(dx, dy) * 10.0f;
            int si0 = pts.Count; pts.Add(new float2(minX - delta, minY - delta));
            int si1 = pts.Count; pts.Add(new float2(minX + delta * 2, minY - delta));
            int si2 = pts.Count; pts.Add(new float2(minX, minY + delta * 2));

            var triangulation = new List<Triangle> { new Triangle(si0, si1, si2) };

            //Insert each point
            for (int pi = 0; pi < si0; pi++)
            {
                var badTriangles = new List<Triangle>();
                foreach (var t in triangulation)
                    if (InCircumcircle(pts[pi], pts[t.a], pts[t.b], pts[t.c]))
                        badTriangles.Add(t);

                //Find boundary polygon (unshared edges)
                var boundary = new List<(int, int)>();
                foreach (var bad in badTriangles)
                {
                    TryAddBoundaryEdge(boundary, badTriangles, bad.a, bad.b);
                    TryAddBoundaryEdge(boundary, badTriangles, bad.b, bad.c);
                    TryAddBoundaryEdge(boundary, badTriangles, bad.c, bad.a);
                }

                foreach (var bad in badTriangles) triangulation.Remove(bad);

                foreach (var (ea, eb) in boundary)
                    triangulation.Add(new Triangle(ea, eb, pi));
            }

            //Remove triangles linked to the super-triangle
            triangulation.RemoveAll(t => t.a >= si0 || t.b >= si0 || t.c >= si0);
            pts.RemoveRange(si0, 3);

            return triangulation;
        }

        //Circumcircle test without sqrt (determinant method)
        private static bool InCircumcircle(float2 p, float2 a, float2 b, float2 c)
        {
            float ax = a.x - p.x, ay = a.y - p.y;
            float bx = b.x - p.x, by = b.y - p.y;
            float cx = c.x - p.x, cy = c.y - p.y;
            float det = ax * (by * (cx * cx + cy * cy) - cy * (bx * bx + by * by))
                       - ay * (bx * (cx * cx + cy * cy) - cx * (bx * bx + by * by))
                       + (ax * ax + ay * ay) * (bx * cy - by * cx);
            return det > 0;
        }

        //Add edge only if not shared by another bad triangle
        private static void TryAddBoundaryEdge(List<(int, int)> boundary, List<Triangle> bad, int ea, int eb)
        {
            int count = 0;
            foreach (var t in bad) if (SharesEdge(t, ea, eb)) count++;
            if (count == 1) boundary.Add((ea, eb));
        }

        private static bool SharesEdge(Triangle t, int ea, int eb)
        {
            return (t.a == ea && t.b == eb) || (t.b == ea && t.c == eb) || (t.c == ea && t.a == eb) ||
                   (t.a == eb && t.b == ea) || (t.b == eb && t.c == ea) || (t.c == eb && t.a == ea);
        }

        private static List<float2> ToFloat2List(List<int2> src)
        {
            var list = new List<float2>(src.Count);
            foreach (var p in src) list.Add(new float2(p.x, p.y));
            return list;
        }

        //Symmetric key to avoid duplicate edges (a,b) == (b,a)
        private static void AddEdge(List<(int2 a, int2 b)> edges, HashSet<long> seen, int2 p0, int2 p1)
        {
            int lo = math.min(p0.x + p0.y * 100000, p1.x + p1.y * 100000);
            int hi = math.max(p0.x + p0.y * 100000, p1.x + p1.y * 100000);
            long key = ((long)lo << 32) | (uint)hi;
            if (seen.Add(key)) edges.Add((p0, p1));
        }
    }
}
