﻿// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable VirtualMemberNeverOverridden.Global
namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// This is a baseclass when you want to have a nonstatic Criteria method
    /// </summary>
    public abstract class AbstractCondition : ICondition
    {
        private readonly Condition condition;
        private readonly IDisposable subscription;

        private bool disposed;
        private string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractCondition"/> class.
        /// </summary>
        protected AbstractCondition(IObservable<object> observable)
        {
            this.condition = new Condition(observable, this.Criteria);
            this.subscription = this.condition.ObserveIsSatisfiedChanged()
                                              .Subscribe(_ => this.OnPropertyChanged(CachedEventArgs.IsSatisfiedChanged));
            this.name = this.GetType().Name;
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc/>
        public bool? IsSatisfied => this.condition.IsSatisfied;

        /// <inheritdoc/>
        IReadOnlyList<ICondition> ICondition.Prerequisites
        {
            get
            {
                this.ThrowIfDisposed();
                return this.condition.Prerequisites;
            }
        }

        /// <inheritdoc/>
        public IEnumerable<ConditionHistoryPoint> History => this.condition.History;

        /// <inheritdoc/>
        public string Name
        {
            get
            {
                this.ThrowIfDisposed();
                return this.name;
            }

            set
            {
                this.ThrowIfDisposed();
                if (value == this.name)
                {
                    return;
                }

                this.name = value;
                this.OnPropertyChanged();
            }
        }

        /// <inheritdoc/>
        public ICondition Negate()
        {
            this.ThrowIfDisposed();
            return new NegatedCondition(this);
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
                this.condition.Dispose();
                this.subscription?.Dispose();
            }
        }

        /// <summary>
        /// Throws an <see cref="ObjectDisposedException"/> if the instance is disposed.
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        /// <summary>
        /// The criteria for <see cref="IsSatisfied"/>
        /// </summary>
        protected abstract bool? Criteria();

        /// <summary>
        /// Raise PropertyChanged event to any listeners.
        /// </summary>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Raise PropertyChanged event to any listeners.
        /// Properties/methods modifying this <see cref="AbstractCondition"/> will raise
        /// a property changed event through this virtual method.
        /// </summary>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, e);
        }
    }
}