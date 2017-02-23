namespace Gu.Reactive.Tests.Conditions
{
    using System;

    using Gu.Reactive.Tests.Helpers;

    using Moq;
    using NUnit.Framework;

    public class OrConditionTests
    {
        private Mock<ICondition> mock1;
        private Mock<ICondition> mock2;
        private Mock<ICondition> mock3;

        [SetUp]
        public void SetUp()
        {
            this.mock1 = new Mock<ICondition>();
            this.mock2 = new Mock<ICondition>();
            this.mock3 = new Mock<ICondition>();
        }

        [TestCase(true, true, true, true)]
        [TestCase(true, true, null, true)]
        [TestCase(true, true, false, true)]
        [TestCase(true, false, null, true)]
        [TestCase(false, null, null, null)]
        [TestCase(null, null, null, null)]
        public void IsSatisfied(bool? first, bool? second, bool? third, bool? expected)
        {
            this.mock1.SetupGet(x => x.IsSatisfied).Returns(first);
            this.mock2.SetupGet(x => x.IsSatisfied).Returns(second);
            this.mock3.SetupGet(x => x.IsSatisfied).Returns(third);
            using (var collection = new OrCondition(this.mock1.Object, this.mock2.Object, this.mock3.Object))
            {
                Assert.AreEqual(expected, collection.IsSatisfied);
            }
        }

        [Test]
        public void Notifies()
        {
            var count = 0;
            var fake1 = new Fake { IsTrue = false };
            var fake2 = new Fake { IsTrue = false };
            var fake3 = new Fake { IsTrue = false };
            using (var condition1 = new Condition(fake1.ObservePropertyChanged(x => x.IsTrue), () => fake1.IsTrue))
            {
                using (var condition2 = new Condition(fake2.ObservePropertyChanged(x => x.IsTrue), () => fake2.IsTrue))
                {
                    using (var condition3 = new Condition(fake3.ObservePropertyChanged(x => x.IsTrue), () => fake3.IsTrue))
                    {
                        using (var collection = new OrCondition(condition1, condition2, condition3))
                        {
                            using (collection.ObserveIsSatisfiedChanged()
                                             .Subscribe(_ => count++))
                            {
                                Assert.AreEqual(false, collection.IsSatisfied);
                                fake1.IsTrue = !fake1.IsTrue;
                                Assert.AreEqual(true, collection.IsSatisfied);
                                Assert.AreEqual(1, count);

                                fake2.IsTrue = !fake2.IsTrue;
                                Assert.AreEqual(true, collection.IsSatisfied);
                                Assert.AreEqual(1, count);

                                fake3.IsTrue = !fake3.IsTrue;
                                Assert.AreEqual(true, collection.IsSatisfied);
                                Assert.AreEqual(1, count);

                                fake1.IsTrue = !fake1.IsTrue;
                                Assert.AreEqual(true, collection.IsSatisfied);
                                Assert.AreEqual(1, count);

                                fake2.IsTrue = !fake2.IsTrue;
                                Assert.AreEqual(true, collection.IsSatisfied);
                                Assert.AreEqual(1, count);

                                fake3.IsTrue = !fake3.IsTrue;
                                Assert.AreEqual(false, collection.IsSatisfied);
                                Assert.AreEqual(2, count);
                            }
                        }
                    }
                }
            }
        }

        [Test]
        public void ThrowsIfEmpty()
        {
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new OrCondition());
        }

        [Test]
        public void Prerequisites()
        {
            using (var collection = new OrCondition(this.mock1.Object, this.mock2.Object, this.mock3.Object))
            {
                CollectionAssert.AreEqual(new[] { this.mock1.Object, this.mock2.Object, this.mock3.Object }, collection.Prerequisites);
            }
        }
    }
}