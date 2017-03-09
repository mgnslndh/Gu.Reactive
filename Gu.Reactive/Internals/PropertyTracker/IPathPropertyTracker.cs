namespace Gu.Reactive.Internals
{
    using System;
    using System.ComponentModel;

    internal interface IPathPropertyTracker : IDisposable
    {
        IPathProperty Property { get; }

        IPropertyPathTracker PathTracker { get; }

        INotifyPropertyChanged Source { get; set; }

        void OnTrackedPropertyChanged(object sender, INotifyPropertyChanged newSource, PropertyChangedEventArgs e);
    }
}