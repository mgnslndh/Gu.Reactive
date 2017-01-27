namespace Gu.Reactive.Tests.Collections.Filter
{
    using Gu.Reactive.Tests.Helpers;

    using Microsoft.Reactive.Testing;

    using NUnit.Framework;

    public class FilteredViewTestsBase : FilterTests
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            this.scheduler = new TestScheduler();
            this.view?.Dispose();
            this.view = this.ints.AsFilteredView(x => true, this.scheduler);
            this.actual = this.view.SubscribeAll();
        }
    }
}