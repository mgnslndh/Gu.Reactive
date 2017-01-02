namespace Gu.Reactive.Tests.Collections
{
    using System;
    using System.Collections.ObjectModel;

    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public class ReadOnlyFilteredViewNoScheduler : CrudSourceTests
    {
        public override void SetUp()
        {
            base.SetUp();
            this.View = new ReadOnlyFilteredView<int>(this.Ints, x => true, TimeSpan.Zero, null);
            this.Actual = this.View.SubscribeAll();
        }

        [Test]
        public void InitializeFiltered()
        {
            var ints = new ObservableCollection<int>(new[] { 1, 2 });
            var view = ints.AsReadOnlyFilteredView(x => x < 2);
            CollectionAssert.AreEqual(new[] { 1 }, view);
        }
    }
}