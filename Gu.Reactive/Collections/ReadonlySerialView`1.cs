namespace Gu.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Reactive.Disposables;

    [DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    public sealed class ReadOnlySerialView<T> : SerialViewBase<T>, IReadOnlyObservableCollection<T>, IUpdater
    {
        private readonly SerialDisposable _refreshSubscription = new SerialDisposable();
        private bool _disposed;

        public ReadOnlySerialView()
            : this(null)
        {
        }

        public ReadOnlySerialView(IEnumerable<T> source)
            : base(source, true, true)
        {
            _refreshSubscription.Disposable = ThrottledRefresher.Create(this, Source, TimeSpan.Zero, null, false)
                                                                .Subscribe(Refresh);
        }

        object IUpdater.IsUpdatingSourceItem => null;

        public new void SetSource(IEnumerable<T> source)
        {
            base.SetSource(source);
            _refreshSubscription.Disposable = ThrottledRefresher.Create(this, Source, TimeSpan.Zero, null, false)
                                                                .Subscribe(Refresh);
        }

        public new void ClearSource()
        {
            base.ClearSource();
            _refreshSubscription.Disposable = null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _refreshSubscription.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}