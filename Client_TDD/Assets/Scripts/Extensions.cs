using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static T CutRandom<T>(this List<T> list)
    {
        var randIndex = Random.Range(0, list.Count);
        list.RemoveAt(randIndex);
        return list[randIndex];
    }
    public static T GetRandom<T>(this List<T> list)
    {
        if (list.Count == 0) throw new System.Exception("you are trying to get a random element from an empty list");

        var randIndex = Random.Range(0, list.Count);
        return list[randIndex];
    }
    public static void AddMultiple<T>(this List<T> list, params T[] args)
    {
        list.AddRange(args);
    }
}