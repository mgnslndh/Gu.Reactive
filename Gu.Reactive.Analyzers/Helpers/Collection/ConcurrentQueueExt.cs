﻿namespace Gu.Reactive.Analyzers
{
    using System;
    using System.Collections.Concurrent;

    internal static class ConcurrentQueueExt
    {
        internal static T GetOrCreate<T>(this ConcurrentQueue<T> queue)
            where T : new()
        {
            if (queue.TryDequeue(out T item))
            {
                return item;
            }

            return new T();
        }

        internal static T GetOrCreate<T>(this ConcurrentQueue<T> queue, Func<T> create)
        {
            if (queue.TryDequeue(out T item))
            {
                return item;
            }

            return create();
        }
    }
}