﻿namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;

    using Gu.Reactive.Internals;

    /// <inheritdoc/>
    public class ReadOnlyFilteredView<T> : ReadonlySerialViewBase<T, T>, IReadOnlyFilteredView<T>
    {
        private readonly IDisposable refreshSubscription;
        private readonly Chunk<NotifyCollectionChangedEventArgs> chunk;

        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyFilteredView{T}"/> class.
        /// </summary>
        public ReadOnlyFilteredView(ObservableCollection<T> collection, Func<T, bool> filter, TimeSpan bufferTime, IScheduler scheduler, params IObservable<object>[] triggers)
            : this((IReadOnlyList<T>)collection, filter, bufferTime, scheduler, triggers)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyFilteredView{T}"/> class.
        /// </summary>
        public ReadOnlyFilteredView(ReadOnlyObservableCollection<T> collection, Func<T, bool> filter, TimeSpan bufferTime, IScheduler scheduler, params IObservable<object>[] triggers)
            : this((IReadOnlyList<T>)collection, filter, bufferTime, scheduler, triggers)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyFilteredView{T}"/> class.
        /// </summary>
        public ReadOnlyFilteredView(IObservableCollection<T> collection, Func<T, bool> filter, TimeSpan bufferTime, IScheduler scheduler, params IObservable<object>[] triggers)
            : this((IReadOnlyList<T>)collection, filter, bufferTime, scheduler, triggers)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyFilteredView{T}"/> class.
        /// </summary>
        public ReadOnlyFilteredView(IReadOnlyObservableCollection<T> collection, Func<T, bool> filter, TimeSpan bufferTime, IScheduler scheduler, params IObservable<object>[] triggers)
            : this((IReadOnlyList<T>)collection, filter, bufferTime, scheduler, triggers)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyFilteredView{T}"/> class.
        /// </summary>
        public ReadOnlyFilteredView(IEnumerable<T> source, Func<T, bool> filter, TimeSpan bufferTime, IScheduler scheduler, IEnumerable<IObservable<object>> triggers)
            : this(source, filter, scheduler, bufferTime, triggers?.ToArray())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyFilteredView{T}"/> class.
        /// </summary>
        public ReadOnlyFilteredView(IEnumerable<T> source, Func<T, bool> filter, TimeSpan bufferTime, IScheduler scheduler, IObservable<object> trigger)
            : this(source, filter, scheduler, bufferTime, new[] { trigger })
        {
            Ensure.NotNull(trigger, nameof(trigger));
        }

        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        private ReadOnlyFilteredView(IEnumerable<T> source, Func<T, bool> filter, IScheduler scheduler, TimeSpan bufferTime, IEnumerable<IObservable<object>> triggers)
            : base(source, x => x.Where(filter), true)
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(filter, nameof(filter));
            this.Filter = filter;
            this.BufferTime = bufferTime;
            this.chunk = new Chunk<NotifyCollectionChangedEventArgs>(bufferTime, scheduler ?? DefaultScheduler.Instance);
            this.refreshSubscription = Observable.Merge(
                                                     source.ObserveCollectionChangedSlimOrDefault(false),
                                                     triggers.MergeOrNever()
                                                             .Select(x => CachedEventArgs.NotifyCollectionReset))
                                                 .Slide(this.chunk)
                                                 .ObserveOn(scheduler ?? ImmediateScheduler.Instance)
                                                 .StartWith(this.chunk.Add(CachedEventArgs.NotifyCollectionReset))
                                                 .Subscribe(this.Refresh);
        }

        /// <inheritdoc/>
        public TimeSpan BufferTime { get; }

        /// <inheritdoc/>
        public Func<T, bool> Filter { get; }

        /// <inheritdoc/>
        public override void Refresh()
        {
            using (this.chunk.ClearTransaction())
            {
                base.Refresh();
            }
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            if (disposing)
            {
                this.refreshSubscription.Dispose();
                this.chunk.ClearItems();
            }

            base.Dispose(disposing);
        }

        private void Refresh(Chunk<NotifyCollectionChangedEventArgs> changes)
        {
            if (changes == null || changes.Count == 0)
            {
                return;
            }

            using (changes.ClearTransaction())
            {
                foreach (var change in changes)
                {
                    if (Filtered.AffectsFilteredOnly(change, this.Filter))
                    {
                        continue;
                    }

                    base.Refresh(CachedEventArgs.SingleNotifyCollectionReset);
                    return;
                }
            }
        }
    }
}