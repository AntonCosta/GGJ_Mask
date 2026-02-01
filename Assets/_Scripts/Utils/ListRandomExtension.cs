using System.Collections.Generic;
using UnityEngine;

public static class ListRandomExtensions
{
    public static T RandomElement<T>(this IList<T> list)
    {
        var i = Random.Range(0, list.Count);
        return list[i];
    }
}
