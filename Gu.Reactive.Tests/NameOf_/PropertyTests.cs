﻿// ReSharper disable UnassignedGetOnlyAutoProperty
#pragma warning disable 618
namespace Gu.Reactive.Tests.NameOf_
{
    using System;

    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public class PropertyTests
    {
        public string StringProp { get; }

        public Fake Fake { get; }

        [Test]
        public void PropertyHappyPath()
        {
            var name = NameOf.Property(() => this.StringProp);
            Assert.AreEqual(nameof(this.StringProp), name);

            name = NameOf.Property<PropertyTests>(x => this.StringProp);
            Assert.AreEqual(nameof(this.StringProp), name);

            name = NameOf.Property<PropertyTests, string>(x => this.StringProp);
            Assert.AreEqual(nameof(this.StringProp), name);
        }

        [Test]
        public void PropertyHappyPath2()
        {
            var name = NameOf.Property(() => this.StringProp);
            Assert.AreEqual(nameof(this.StringProp), name);

            name = NameOf.Property<PropertyTests>(x => x.StringProp);
            Assert.AreEqual(nameof(this.StringProp), name);

            name = NameOf.Property<PropertyTests, string>(x => x.StringProp);
            Assert.AreEqual(nameof(this.StringProp), name);

            name = NameOf.Property<PropertyTests, string>(x => x.StringProp);
            Assert.AreEqual(nameof(this.StringProp), name);
        }

        [Test]
        public void BoxedPropertyHappyPath()
        {
            var fakeInpc = new Fake();
            var name = NameOf.Property(() => fakeInpc.IsTrue, allowNestedProperty: true);
            Assert.AreEqual("IsTrue", name);

            name = NameOf.Property<Fake>(x => x.IsTrue);
            Assert.AreEqual("IsTrue", name);

            name = NameOf.Property<Fake, bool>(x => x.IsTrue);
            Assert.AreEqual("IsTrue", name);
        }

        [Test]
        public void NestedPropertyHappyPath()
        {
            var name = NameOf.Property(() => this.Fake.Next.Name, allowNestedProperty: true);
            Assert.AreEqual("Name", name);

            name = NameOf.Property<PropertyTests>(x => this.Fake.Next.Name);
            Assert.AreEqual("Name", name);

            name = NameOf.Property<PropertyTests, string>(x => this.Fake.Next.Name);
            Assert.AreEqual("Name", name);
        }

        [Test]
        public void ThrowsOnNestedProperty()
        {
            var exception = Assert.Throws<ArgumentException>(() => NameOf.Property(() => this.StringProp.Length));
            Assert.AreEqual("Trying to get the name of a nested property: StringProp.Length", exception.Message);
        }

        [Test]
        public void ThrowsOnMethod()
        {
            Assert.Throws<ArgumentException>(() => NameOf.Property(() => this.Fake.Method()));
            Assert.Throws<ArgumentException>(() => NameOf.Property<PropertyTests>(x => x.Fake.Method()));
            Assert.Throws<ArgumentException>(() => NameOf.Property<PropertyTests, Level>(x => x.Fake.Method()));
        }

        [Test]
        public void ThrowsOnNestedMethod()
        {
            Assert.Throws<ArgumentException>(() => NameOf.Property(() => this.Fake.Next.Method()));
            Assert.Throws<ArgumentException>(() => NameOf.Property<PropertyTests>(x => x.Fake.Next.Method()));
            Assert.Throws<ArgumentException>(() => NameOf.Property<PropertyTests, Level>(x => x.Fake.Next.Method()));
        }
    }
}
