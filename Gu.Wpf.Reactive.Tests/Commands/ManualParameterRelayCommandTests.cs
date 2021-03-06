﻿namespace Gu.Wpf.Reactive.Tests
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using Gu.Reactive;
    using Gu.Wpf.Reactive.Tests.FakesAndHelpers;

    using NUnit.Framework;

    public class ManualParameterRelayCommandTests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            App.Start();
        }

        [Test(Description = "This is the most relevant test, it checks that the weak event implementation is correct")]
        public void MemoryLeak()
        {
            var command = new ManualRelayCommand<int>(x => { }, x => true);
            var listener = new CommandListener();
            var wr = new WeakReference(listener);
            command.CanExecuteChanged += listener.React;
            //// ReSharper disable once RedundantAssignment
            listener = null;
            GC.Collect();
            Assert.IsFalse(wr.IsAlive);
            command.CanExecute(0); // Touching it after to prevent GC
        }

        [Test]
        public async Task RaiseCanExecuteChanged()
        {
            var count = 0;
            var command = new ManualRelayCommand<int>(x => { }, x => true);
            command.CanExecuteChanged += (sender, args) => count++;
            Assert.AreEqual(0, count);
            command.RaiseCanExecuteChanged();
            await Application.Current.Dispatcher.SimulateYield();
            Assert.AreEqual(1, count);
        }

        [Test]
        public void ExecuteWithParameter()
        {
            var invokeCount = 0;
            var command = new ManualRelayCommand<int>(o => invokeCount = o, _ => true);
            command.Execute(4);
            Assert.AreEqual(4, invokeCount);
        }

        [Test]
        public void ExecuteNotifies()
        {
            var invokeCount = 0;
            var isExecutingCount = 0;
            var command = new ManualRelayCommand<int>(i => invokeCount += i, _ => true);
            using (command.ObservePropertyChangedSlim(nameof(command.IsExecuting), signalInitial: false)
                          .Subscribe(_ => isExecutingCount++))
            {
                Assert.IsFalse(command.IsExecuting);
                Assert.True(command.CanExecute(1));
                command.Execute(1);
                Assert.IsFalse(command.IsExecuting);
                Assert.True(command.CanExecute(1));
                Assert.AreEqual(1, invokeCount);
                Assert.AreEqual(2, isExecutingCount);
            }
        }

        [TestCase(1, 2, false)]
        [TestCase(1, 1, true)]
        public void CanExecuteWithParameter(int i, int parameter, bool expected)
        {
            var command = new ManualRelayCommand<int>(x => { }, x => i == x);
            Assert.AreEqual(expected, command.CanExecute(parameter));
        }
    }
}