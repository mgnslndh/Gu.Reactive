﻿namespace Gu.Wpf.Reactive
{
    using System;
    using System.Collections.ObjectModel;
    using Gu.Reactive;
    using Gu.Reactive.Internals;

    /// <summary>
    /// Factory methods for creating <see cref="FilteredView{T}"/>
    /// </summary>
    public static partial class FilteredView
    {
        /// <summary>
        /// Create a filtered view for <paramref name="collection"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="collection">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="triggers">Triggers for when filtering is updated.</param>
        /// <returns>A <see cref="FilteredView{T}"/></returns>
        public static FilteredView<T> AsFilteredView<T>(
            this ObservableCollection<T> collection,
            Func<T, bool> filter,
            params IObservable<object>[] triggers)
        {
            Ensure.NotNull(collection, nameof(collection));
            return new FilteredView<T>(collection, filter, TimeSpan.Zero, null, triggers);
        }

        /// <summary>
        /// Create a filtered view for <paramref name="collection"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="collection">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="bufferTime">The time to buffer changes in <paramref name="collection"/></param>
        /// <param name="triggers">Triggers for when filtering is updated.</param>
        /// <returns>A <see cref="FilteredView{T}"/></returns>
        public static FilteredView<T> AsFilteredView<T>(
            this ObservableCollection<T> collection,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            params IObservable<object>[] triggers)
        {
            Ensure.NotNull(collection, nameof(collection));
            Ensure.NotNull(filter, nameof(filter));
            return new FilteredView<T>(collection, filter, bufferTime, null, triggers);
        }

        /// <summary>
        /// Create a filtered view for <paramref name="collection"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="collection">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="triggers">Triggers for when filtering is updated.</param>
        /// <returns>A <see cref="FilteredView{T}"/></returns>
        public static FilteredView<T> AsFilteredView<T>(
            this IObservableCollection<T> collection,
            Func<T, bool> filter,
            params IObservable<object>[] triggers)
        {
            Ensure.NotNull(collection, nameof(collection));
            return new FilteredView<T>(collection, filter, TimeSpan.Zero, null, triggers);
        }

        /// <summary>
        /// Create a filtered view for <paramref name="collection"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="collection">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="bufferTime">The time to buffer changes in <paramref name="collection"/></param>
        /// <param name="triggers">Triggers for when filtering is updated.</param>
        /// <returns>A <see cref="FilteredView{T}"/></returns>
        public static FilteredView<T> AsFilteredView<T>(
            this IObservableCollection<T> collection,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            params IObservable<object>[] triggers)
        {
            Ensure.NotNull(collection, nameof(collection));
            Ensure.NotNull(filter, nameof(filter));
            return new FilteredView<T>(collection, filter, bufferTime, null, triggers);
        }
    }
}