namespace Gu.Reactive.Tests.Collections
{
    using System;

    using Microsoft.Reactive.Testing;

    public class FilteredMappedView : CrudSourceTests
    {
        public override void SetUp()
        {
            base.SetUp();
            this.Scheduler = new TestScheduler();
            (this.View as IDisposable)?.Dispose();
            this.View = this.Source.AsFilteredView(x => true, TimeSpan.FromMilliseconds(10), this.Scheduler)
                                 .AsMappingView(x => x);
            this.Scheduler.Start();
        }
    }
}