using System.Collections.Generic;
using UnityEngine;

public static class ListRandomExtensions
{
    public static T RandomElement<T>(this IList<T> list)
    {
        var i = Random.Range(0, list.Count);
        return list[i];
    }
    
    public static T RandomEvenElement<T>(this IList<T> list)
    {
        var count = (list.Count + 1) / 2;
        var k = Random.Range(0, count);
        return list[2 * k];
    }

    public static T RandomOddElement<T>(this IList<T> list)
    {
        var count = list.Count / 2;
        var k = Random.Range(0, count);
        return list[2 * k + 1];
    }
}
