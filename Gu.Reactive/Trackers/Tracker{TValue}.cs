﻿namespace Gu.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Gu.Reactive.Internals;
    using JetBrains.Annotations;

    /// <summary>
    /// A base class for trackers of aggretages of the values in a collection.
    /// </summary>
    public abstract class Tracker<TValue> : ITracker<TValue?>
        where TValue : struct
    {
        /// <summary>
        /// The source collection.
        /// </summary>
        protected readonly IReadOnlyList<TValue> Source;

        /// <summary>
        /// For locking.
        /// </summary>
        protected readonly object Gate;

        /// <summary>
        /// The value to use when the <see cref="Source"/> is empty.
        /// </summary>
        protected readonly TValue? WhenEmpty;

        private readonly IDisposable subscription;

        private TValue? value;
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="Tracker{TValue}"/> class.
        /// </summary>
        protected Tracker(
            IReadOnlyList<TValue> source,
            IObservable<NotifyCollectionChangedEventArgs> onChanged,
            TValue? whenEmpty)
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(onChanged, nameof(onChanged));
            this.Source = source;
            this.Gate = (source as ICollection)?.SyncRoot ?? new object();
            this.WhenEmpty = whenEmpty;
            this.subscription = onChanged.Subscribe(this.Refresh);
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc/>
        public TValue? Value
        {
            get
            {
                this.ThrowIfDisposed();
                return this.value;
            }

            protected set
            {
                if (Equals(value, this.value))
                {
                    return;
                }

                this.value = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Reset calculation of <see cref="Value"/>
        /// </summary>
        public void Reset()
        {
            this.ThrowIfDisposed();
            if (this.Source.Count == 0)
            {
                this.Value = this.WhenEmpty;
                return;
            }

            this.Value = this.GetValue(this.Source);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Protected implementation of Dispose pattern.
        /// </summary>
        /// <param name="disposing">true: safe to free managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            if (disposing)
            {
                this.subscription.Dispose();
            }
        }

        /// <summary>
        /// Called when the source collection notifies about collection changes.
        /// </summary>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/></param>
        protected virtual void Refresh(NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        if (this.Source.Count > 1 && e.IsSingleNewItem())
                        {
                            var newValue = e.NewItem<TValue>();
                            this.OnAdd(newValue);
                        }
                        else
                        {
                            this.Reset();
                        }

                        break;
                    }

                case NotifyCollectionChangedAction.Remove:
                    {
                        if (e.IsSingleOldItem())
                        {
                            var oldValue = e.OldItem<TValue>();
                            this.OnRemove(oldValue);
                        }
                        else
                        {
                            this.Reset();
                        }

                        break;
                    }

                case NotifyCollectionChangedAction.Replace:
                    {
                        if (e.IsSingleOldItem() && e.IsSingleOldItem())
                        {
                            var oldValue = e.OldItem<TValue>();
                            var newValue = e.NewItem<TValue>();
                            this.OnReplace(oldValue, newValue);
                        }
                        else
                        {
                            this.Reset();
                        }

                        break;
                    }

                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    {
                        this.Reset();
                        break;
                    }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Called when a value is added to the source collection.
        /// </summary>
        /// <param name="value">The new value.</param>
        protected abstract void OnAdd(TValue value);

        /// <summary>
        /// Called when a value is removed from the source collection.
        /// </summary>
        /// <param name="value">The removed value.</param>
        protected abstract void OnRemove(TValue value);

        /// <summary>
        /// Called when a value is replaced in the source collection.
        /// </summary>
        /// <param name="oldValue">The new value.</param>
        /// <param name="newValue">The removed value.</param>
        protected abstract void OnReplace(TValue oldValue, TValue newValue);

        /// <summary>
        /// Produce a <see cref="Value"/> from the source collection.
        /// </summary>
        /// <param name="source">The source collection.</param>
        /// <returns>The value.</returns>
        protected abstract TValue GetValue(IReadOnlyList<TValue> source);

        /// <summary>
        /// Check if the instance is disposed and throw ObjectDisposedException if it is.
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        /// <summary>
        /// Raise PropertyChanged event to any listeners.
        /// Properties/methods modifying this <see cref="Tracker{TValue}"/> will raise
        /// a property changed event through this virtual method.
        /// </summary>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}