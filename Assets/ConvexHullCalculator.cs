using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Algoritm is taken from
// http://en.wikibooks.org/wiki/Algorithm_Implementation/Geometry/Convex_hull/Monotone_chain
public class ConvexHullCalculator
{
    public static IEnumerable<Vector2> Calculate(List<Vector2> points)
    {
        points.Sort(new VectorComparer());

        var upper = new List<Vector2>();
        var lower = new List<Vector2>();

        for (var i = 0; i < points.Count; i++)
        {
            var point = points[i];
            while (lower.Count >= 2 && Cross(lower[lower.Count - 2], lower[lower.Count - 1], point) <= 0)
            {
                Pop(lower);
            }
            lower.Add(point);
        }

        for (var i = points.Count - 1; i >= 0; i--)
        {
            var point = points[i];
            while (upper.Count >= 2 && Cross(upper[upper.Count - 2], upper[upper.Count - 1], point) <= 0)
            {
                Pop(upper);
            }
            upper.Add(point);
        }

        Pop(upper);
        Pop(lower);
        return lower.Concat(upper);
    }

    private static void Pop(List<Vector2> list)
    {
        list.RemoveAt(list.Count - 1);
    }

    private static float Cross(Vector2 o, Vector2 a, Vector2 b)
    {
        return (a.x - o.x) * (b.y - o.y) - (a.y - o.y) * (b.x - o.x);
    }
}

public class VectorComparer : IComparer<Vector2>
{
    public int Compare(Vector2 val1, Vector2 val2)
    {
        if (AreSame(val1.x, val2.x))
        {
            return Compare(val1.y, val2.y);
        }

        return Compare(val1.x, val2.x);
    }

    private int Compare(float val1, float val2)
    {
        if (AreSame(val1, val2))
        {
            return 0;
        }

        if (val1 > val2)
        {
            return 1;
        }

        return -1;
    }

    private bool AreSame(float val1, float val2)
    {
        return Math.Abs(val1 - val2) < 0.0001f;
    }
}