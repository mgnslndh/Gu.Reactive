﻿namespace Gu.Reactive.Analyzers
{
    using System.Collections.Generic;

    internal static class ListExt
    {
        internal static bool AddIfNotExits<T>(this List<T> list, T item)
        {
            if (list == null ||
                item == null)
            {
                return false;
            }

            if (list.Contains(item))
            {
                return false;
            }

            list.Add(item);
            return true;
        }

        internal static void PurgeDuplicates<T>(this List<T> list, IEqualityComparer<T> comparer = null)
        {
            comparer = comparer ?? EqualityComparer<T>.Default;
            for (var i = 0; i < list.Count; i++)
            {
                for (var j = list.Count - 1; j > i; j--)
                {
                    if (comparer.Equals(list[i], list[j]))
                    {
                        list.RemoveAt(j);
                    }
                }
            }
        }
    }
}