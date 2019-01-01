using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExtensionMethods
{
    public static class ListExtensions
    {
        public static void MoveItemAtFrontToEnd<T>(this List<T> list)
        {
            T item = list[0];
            list.RemoveAt(0);
            list.Add(item);
        }

        public static T GetRandomElement<T>(this List<T> list)
        {
            return list[Random.Range(0,list.Count)];
        }
    }
}