﻿namespace Gu.Reactive.Demo
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Reactive.Concurrency;
    using System.Runtime.CompilerServices;
    using System.Windows.Data;

    using JetBrains.Annotations;

    using Wpf.Reactive;

    public class CollectionViewDemoViewModel : INotifyPropertyChanged
    {
        private Func<int, bool> filter = x => true;

        public CollectionViewDemoViewModel()
        {
            this.Enumerable = new[] { 1, 2, 3, 4, 5 };
            this.FilteredView1 = this.Enumerable.AsReadOnlyFilteredView(this.FilterMethod, TimeSpan.FromMilliseconds(10), WpfSchedulers.Dispatcher, this.ObservePropertyChanged(x => x.Filter));
            this.FilteredView2 = this.Enumerable.AsReadOnlyFilteredView(this.FilterMethod, TimeSpan.FromMilliseconds(10), WpfSchedulers.Dispatcher, this.ObservePropertyChanged(x => x.Filter));

            this.ObservableCollection = new ObservableCollection<int>(new[] { 1, 2, 3, 4, 5 });
            this.ObservableDefaultView = CollectionViewSource.GetDefaultView(this.ObservableCollection);
            this.ObservableFilteredView = this.ObservableCollection.AsFilteredView(this.Filter, TimeSpan.Zero, WpfSchedulers.Dispatcher);
            this.ThrottledFilteredView = this.ObservableCollection.AsFilteredView(this.Filter, TimeSpan.FromMilliseconds(10), WpfSchedulers.Dispatcher);

            this.ObservePropertyChanged(x => x.Filter, false)
                .Subscribe(
                    x =>
                    {
                        WpfSchedulers.Dispatcher.Schedule(() => this.ObservableDefaultView.Filter = o => this.Filter((int)o));
                        this.ObservableFilteredView.Filter = this.Filter;
                        this.ThrottledFilteredView.Filter = this.Filter;
                    });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public IEnumerable<int> Enumerable { get; }

        public ReadOnlyFilteredView<int> FilteredView1 { get; private set; }

        public ReadOnlyFilteredView<int> FilteredView2 { get; private set; }

        public ObservableCollection<int> ObservableCollection { get; }

        public ICollectionView ObservableDefaultView { get; }

        public IFilteredView<int> ObservableFilteredView { get; }

        public IFilteredView<int> ThrottledFilteredView { get; }

        public Func<int, bool> Filter
        {
            get
            {
                return this.filter;
            }

            set
            {
                if (Equals(value, this.filter))
                {
                    return;
                }

                this.filter = value;
                this.OnPropertyChanged();
            }
        }

        private bool FilterMethod(int value)
        {
            return this.Filter(value);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
