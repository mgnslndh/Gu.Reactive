﻿namespace Gu.Wpf.Reactive.Tests
{
    using System;
    using System.Reactive.Subjects;

    using Gu.Reactive;
    using Gu.Reactive.Tests;

    using NUnit.Framework;

    public class ObservingRelayCommandTests
    {
        [Test]
        public void NotifiesOnConditionChanged()
        {
            var fake = new FakeInpc { Prop1 = false };
            var observable = fake.ObservePropertyChanged(x => x.Prop1);
            var command = new ObservingRelayCommand(() => { }, () => false, observable);
            int count = 0;
            command.CanExecuteChanged += (sender, args) => count++;
            fake.Prop1 = true;
            Assert.AreEqual(1, count);
        }


        [TestCase(true)]
        [TestCase(false)]
        public void CanExecuteCondition(bool expected)
        {
            var observable = new Subject<object>();
            var command = new ObservingRelayCommand(() => { }, () => expected, observable);
            Assert.AreEqual(expected, command.CanExecute());
        }

        [TestCase(true)]
        [TestCase(false)]
        public void CanExecuteConditionParameter(bool expected)
        {
            var observable = new Subject<object>();
            var command = new ObservingRelayCommand<int>(_ => { }, _ => expected, observable);
            Assert.AreEqual(expected, command.CanExecute(0));
        }
    }
}