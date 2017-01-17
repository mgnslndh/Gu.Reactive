﻿// ReSharper disable All
namespace Gu.Reactive.Tests.Internals
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reactive;

    using Gu.Reactive.PropertyPathStuff;
    using Gu.Reactive.Tests.Helpers;

    using Moq;

    using NUnit.Framework;

    /// <summary>
    /// Dunno if it is nice to test internals like this, was used for development.
    /// </summary>
    public class PathObservableTests
    {
        private List<EventPattern<PropertyChangedEventArgs>> changes;

        [SetUp]
        public void SetUp()
        {
            this.changes = new List<EventPattern<PropertyChangedEventArgs>>();
        }

        [Test(Description = "All parts of the property path must be INotifyPropertyChanged")]
        public void ThrowsIfNotNotifyingPath()
        {
            var exception = Assert.Throws<ArgumentException>(() => new PropertyPathObservable<Fake, int>(new Fake(), x => x.Name.Length));
            Console.WriteLine(exception.Message);
        }

        [Test(Description = "All parts of the property path must be class")]
        public void ThrowsIfStructInPath()
        {
            var exception = Assert.Throws<ArgumentException>(() => new PropertyPathObservable<Fake, string>(new Fake(), x => x.StructLevel.Name));
            Console.WriteLine(exception.Message);
        }

        [Test]
        public void Implicit()
        {
            var path = PropertyPath.GetOrCreate<IReadOnlyObservableCollection<Fake>, int>(x => x.Count);
            var source = Mock.Of<IReadOnlyObservableCollection<Fake>>();
            Assert.DoesNotThrow(() => new PropertyPathObservable<IReadOnlyObservableCollection<Fake>, int>(source, path));
        }

        [Test]
        public void SubscribeToExistsingChangeLastValueInPath()
        {
            var fake = new Fake { Next = new Level() };
            var observable = new PropertyPathObservable<Fake, bool>(fake, x => x.Next.IsTrue);
            observable.Subscribe(this.changes.Add);
            fake.Next.IsTrue = !fake.Next.IsTrue;
            Assert.AreEqual(1, this.changes.Count);
        }

        [Test]
        public void TwoLevelsValueType()
        {
            var fake = new Fake();
            var observable = new PropertyPathObservable<Fake, bool>(fake, x => x.Next.IsTrue);
            observable.Subscribe(this.changes.Add);
            fake.Next = new Level();
            Assert.AreEqual(1, this.changes.Count);
            fake.Next.IsTrue = !fake.Next.IsTrue;
            Assert.AreEqual(2, this.changes.Count);
            fake.Next = null;
            Assert.AreEqual(3, this.changes.Count);
        }

        [Test]
        public void TwoLevelsExisting()
        {
            var fake = new Fake { Next = new Level { Next = new Level() } };
            var observable = new PropertyPathObservable<Fake, Level>(fake, x => x.Next.Next);
            observable.Subscribe(this.changes.Add);
            fake.Next.Next = new Level();
            Assert.AreEqual(1, this.changes.Count);
            fake.Next.Next = null;
            Assert.AreEqual(2, this.changes.Count);
            fake.Next = null;
            Assert.AreEqual(2, this.changes.Count);
        }

        [Test]
        public void TwoLevelsReferenceType()
        {
            var fake = new Fake();
            var observable = new PropertyPathObservable<Fake, Level>(fake, x => x.Next.Next);
            observable.Subscribe(this.changes.Add);
            fake.Next = new Level();
            Assert.AreEqual(0, this.changes.Count);
            fake.Next.Next = new Level();
            Assert.AreEqual(1, this.changes.Count);
            fake.Next.Next = null;
            Assert.AreEqual(2, this.changes.Count);
            fake.Next = null;
            Assert.AreEqual(2, this.changes.Count);
        }

        [Test]
        public void Unsubscribes()
        {
            var fake = new Fake();
            var observable = new PropertyPathObservable<Fake, bool>(fake, x => x.Next.IsTrue);
            observable.Subscribe(this.changes.Add);
            fake.Next = new Level
                        {
                            Next = new Level()
                        };

            Assert.AreEqual(1, this.changes.Count);
            var temp = fake.Next;
            fake.Next = null;
            Assert.AreEqual(2, this.changes.Count);
            temp.IsTrue = !temp.IsTrue;
            Assert.AreEqual(2, this.changes.Count);
        }

        [Test]
        public void ThreeLevels()
        {
            var fake = new Fake();
            var observable = new PropertyPathObservable<Fake, bool>(fake, x => x.Next.Next.IsTrue);
            observable.Subscribe(this.changes.Add);
            fake.Next = new Level();
            Assert.AreEqual(0, this.changes.Count);
            fake.Next.Next = new Level
                             {
                                 Next = new Level()
                             };
            Assert.AreEqual(1, this.changes.Count);
            fake.Next.Next = null;
            Assert.AreEqual(2, this.changes.Count);
            fake.Next = null;
            Assert.AreEqual(2, this.changes.Count);
            fake.Next = new Level
                        {
                            Next = new Level
                                   {
                                       IsTrue = true
                                   }
                        };
            Assert.AreEqual(3, this.changes.Count);
            fake.Next.Next.IsTrue = false;
            Assert.AreEqual(4, this.changes.Count);
            fake.Next.Next = null;
            Assert.AreEqual(5, this.changes.Count);
            fake.Next = null;
            Assert.AreEqual(5, this.changes.Count);
        }

        [Test]
        public void MemoryLeakTest()
        {
            var fake = new Fake();
            var level1 = new Level();
            var wr = new WeakReference(level1);
            Assert.IsTrue(wr.IsAlive);
            using (fake.ObservePropertyChanged(x => x.Next.IsTrue)
                       .Subscribe())
            {
                fake.Next = level1;
                fake.Next = null;
                level1 = null;
            }

            GC.Collect();
            Assert.IsFalse(wr.IsAlive);
        }
    }
}