﻿namespace Gu.Reactive.Internals
{
    /// <summary>
    /// An <see cref="IGetter"/> for a notifying property.
    /// </summary>
    internal interface INotifyingGetter : IGetter
    {
        /// <summary>
        /// Create tracker for the property.
        /// </summary>
        IPropertyTracker CreateTracker(IPropertyPathTracker tracker);
    }
}