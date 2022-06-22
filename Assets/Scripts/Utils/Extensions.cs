using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

public static class Extensions
{
    public static Vector3 ZeroX(this Vector3 vector) {
        vector.x = 0;
        return vector;
    }

    public static Vector3 ZeroY(this Vector3 vector) {
        vector.y = 0;
        return vector;
    }

    public static Vector3 ZeroZ(this Vector3 vector) {
        vector.z = 0;
        return vector;
    }

    public static string Get(this XElement el, string name)
    {
        return el.Attribute(name)?.Value;
    }

    public static int Random(this double[] weights, double r)
    {
        var threshold = r * weights.Sum();

        double sum = 0;
        for(int i = 0; i < weights.Length; i++)
        {
            sum += weights[i];
            if(sum >= threshold)
                return i;
        }

        return weights.Length - 1;
    }

    public static string Join<T>(this IEnumerable<T> list)
    {
        return string.Join(", ", list);
    }

    public static int Sum(this IEnumerable<bool> list)
    {
        return list.Sum(val => val ? 1 : 0);
    }

    public static bool LessThan(this Vector2 vec, Vector2 other)
    {
        return vec.x <= other.x && vec.y <= other.y;
    }
}