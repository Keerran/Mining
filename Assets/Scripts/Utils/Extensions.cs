using System;
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

    public static Dictionary<T, U> ToDictionary<S, T, U>(this IEnumerable<S> list, Func<S, (T, U)> selector)
    {
        return list.Select(selector).ToDictionary(o => o.Item1, o => o.Item2);
    }

    public static T Random<T>(this T[] arr)
    {
        if(arr.Length == 0)
            return default(T);
        return arr[UnityEngine.Random.Range(0, arr.Length)];
    }

    public static bool Raycast(this Collider[] cols, Ray ray, out RaycastHit hit, float distance)
    {
        foreach(var col in cols)
        {
            if(col.Raycast(ray, out hit, distance))
            {
                return true;
            }
        }
        hit = new RaycastHit();
        return false;
    }

    public static IEnumerable<(int, T)> Enumerate<T>(this IEnumerable<T> arr)
    {
        return arr.Select((v, i) => (i, v));
    }

    public static int ApplyDeadzone(this float value, float amount)
    {
        if(Mathf.Abs(value) < amount)
            return 0;
        return (int)Mathf.Sign(value);
    }

    public static Vector2 XY(this Vector3 vec)
    {
        return new Vector2(vec.x, vec.y);
    }
}