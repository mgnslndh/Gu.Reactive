namespace Gu.Reactive.Tests.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Gu.Reactive.Tests.Helpers;
    using Moq;
    using NUnit.Framework;

    public partial class MappingViewTests
    {
        public class ReferenceTypeRemoving
        {
            [Test]
            public void Initializes()
            {
                var model1 = new Model(1);
                var model2 = new Model(2);
                var model3 = new Model(3);
                var source = new ObservableCollection<Model>(
                    new[]
                        {
                        model1,
                        model1,
                        model1,
                        model2,
                        model2,
                        model2,
                        model3,
                        model3,
                        model3,
                        });
                using (var view = source.AsMappingView(CreateMock, x => x.Object.Dispose()))
                {
                    Assert.AreSame(view[0], view[1]);
                    CollectionAssert.AreEqual(source, view.Select(x => x.Object.Model));
                }
            }

            [Test]
            public void Updates()
            {
                var source = new ObservableCollection<Model>();
                using (var view = source.AsMappingView(CreateMock, x => x.Object.Dispose()))
                {
                    var model = new Model(1);
                    source.Add(model);
                    CollectionAssert.AreEqual(source, view.Select(x => x.Object.Model));

                    var mock = view[0];
                    mock.Verify(x => x.Dispose(), Times.Never);
                    mock.Setup(x => x.Dispose());
                    source.Clear();
                    CollectionAssert.IsEmpty(view);
                    mock.Verify(x => x.Dispose(), Times.Once);

                    source.Add(model);
                    CollectionAssert.AreEqual(source, view.Select(x => x.Object.Model));
                }
            }

            [Test]
            public void Refresh()
            {
                var model1 = new Model(1);
                var model2 = new Model(2);
                var model3 = new Model(3);
                var source = new ObservableBatchCollection<Model>(
                    new[]
                        {
                        model1,
                        model1,
                        model1,
                        model2,
                        model2,
                        model2,
                        model3,
                        model3,
                        model3,
                        });
                using (var view = source.AsMappingView(CreateMock, x => x.Object.Dispose()))
                {
                    using (var expected = source.SubscribeAll())
                    {
                        using (var actual = view.SubscribeAll())
                        {
                            CollectionAssert.IsEmpty(actual);
                            CollectionAssert.AreEqual(source, view.Select(x => x.Object.Model));

                            source.AddRange(new[] { model1, model2, model2 });
                            CollectionAssert.AreEqual(source, view.Select(x => x.Object.Model));
                            CollectionAssert.AreEqual(expected, actual);

                            var mocks = view.ToArray();
                            foreach (var mock in mocks)
                            {
                                mock.Setup(x => x.Dispose());
                            }

                            source.Clear();
                            CollectionAssert.IsEmpty(view);
                            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                            foreach (var mock in mocks)
                            {
                                mock.Verify(x => x.Dispose(), Times.Once);
                            }
                        }
                    }
                }
            }

            [Test]
            public void Caches()
            {
                var source = new ObservableCollection<Model>();
                using (var view = source.AsMappingView(CreateMock, x => x.Object.Dispose()))
                {
                    var model = new Model(1);
                    source.Add(model);
                    CollectionAssert.AreEqual(source, view.Select(x => x.Object.Model));

                    source.Add(model);
                    CollectionAssert.AreEqual(source, view.Select(x => x.Object.Model));
                    Assert.AreSame(view[0], view[1]);

                    var mocks = view.ToArray();
                    var vm = view[0];
                    foreach (var mock in mocks)
                    {
                        mock.Setup(x => x.Dispose());
                    }

                    source.Clear();
                    CollectionAssert.IsEmpty(view);
                    foreach (var mock in mocks)
                    {
                        mock.Verify(x => x.Dispose(), Times.Once);
                    }

                    source.Clear();
                    CollectionAssert.IsEmpty(view);
                    source.Add(model);
                    Assert.AreNotSame(vm, view[0]);
                    Assert.AreSame(vm.Object.Model, view[0].Object.Model);
                }
            }

            [Test]
            public void CachesWhenNotEmpty()
            {
                var model1 = new Model(1);
                var model2 = new Model(2);
                var model3 = new Model(3);
                var source = new ObservableCollection<Model>(
                    new[]
                        {
                        model1,
                        model1,
                        model1,
                        model2,
                        model2,
                        model2,
                        model3,
                        model3,
                        model3,
                        });

                using (var view = source.AsMappingView(CreateMock, x => x.Object.Dispose()))
                {
                    CollectionAssert.AreEqual(source, view.Select(x => x.Object.Model));

                    var model4 = new Model(4);
                    source.Add(model4);
                    CollectionAssert.AreEqual(source, view.Select(x => x.Object.Model));

                    source.Add(model4);
                    CollectionAssert.AreEqual(source, view.Select(x => x.Object.Model));

                    source.Add(model1);
                    CollectionAssert.AreEqual(source, view.Select(x => x.Object.Model));

                    var mocks = view.ToArray();
                    foreach (var mock in mocks)
                    {
                        mock.Setup(x => x.Dispose());
                    }

                    source.Clear();
                    CollectionAssert.IsEmpty(view);
                    foreach (var mock in mocks)
                    {
                        mock.Verify(x => x.Dispose(), Times.Once);
                    }
                }
            }

            [Test]
            public void Add()
            {
                var source = new ObservableCollection<Model>();
                using (var view = source.AsMappingView(CreateMock, x => x.Object.Dispose()))
                {
                    using (var actual = view.SubscribeAll())
                    {
                        var model1 = new Model(1);
                        source.Add(model1);
                        CollectionAssert.AreEqual(source, view.Select(x => x.Object.Model));
                        var expected = new List<EventArgs>
                        {
                            CachedEventArgs.CountPropertyChanged,
                            CachedEventArgs.IndexerPropertyChanged,
                            Diff.CreateAddEventArgs(view[0], 0)
                        };
                        CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);

                        source.Add(model1);
                        CollectionAssert.AreEqual(source, view.Select(x => x.Object.Model));
                        expected.AddRange(new EventArgs[]
                        {
                            CachedEventArgs.CountPropertyChanged,
                            CachedEventArgs.IndexerPropertyChanged,
                            Diff.CreateAddEventArgs(view[1], 1)
                        });
                        CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);

                        source.Add(new Model(2));
                        CollectionAssert.AreEqual(source, view.Select(x => x.Object.Model));
                        expected.AddRange(new EventArgs[]
                        {
                            CachedEventArgs.CountPropertyChanged,
                            CachedEventArgs.IndexerPropertyChanged,
                            Diff.CreateAddEventArgs(view[2], 2)
                        });
                        CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                    }
                }
            }

            [Test]
            public void Remove()
            {
                var model1 = new Model(1);
                var model2 = new Model(2);
                var model3 = new Model(3);
                var source = new ObservableCollection<Model>(
                    new[]
                        {
                        model1,
                        model1,
                        model1,
                        model2,
                        model2,
                        model2,
                        model3,
                        model3,
                        model3,
                        });
                using (var view = source.AsMappingView(CreateMock, x => x.Object.Dispose()))
                {
                    using (var actual = view.SubscribeAll())
                    {
                        var mock = view[0];
                        source.RemoveAt(0);
                        mock.Verify(x => x.Dispose(), Times.Never);
                        CollectionAssert.AreEqual(source, view.Select(x => x.Object.Model));
                        var expected = new List<EventArgs>
                        {
                            CachedEventArgs.CountPropertyChanged,
                            CachedEventArgs.IndexerPropertyChanged,
                            Diff.CreateRemoveEventArgs(mock, 0)
                        };
                        CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);

                        source.RemoveAt(0);
                        mock.Verify(x => x.Dispose(), Times.Never);
                        CollectionAssert.AreEqual(source, view.Select(x => x.Object.Model));
                        expected.AddRange(new EventArgs[]
                        {
                            CachedEventArgs.CountPropertyChanged,
                            CachedEventArgs.IndexerPropertyChanged,
                            Diff.CreateRemoveEventArgs(mock, 0)
                        });
                        CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);

                        mock.Setup(x => x.Dispose());
                        source.RemoveAt(0);
                        mock.Verify(x => x.Dispose(), Times.Once);
                        CollectionAssert.AreEqual(source, view.Select(x => x.Object.Model));
                        expected.AddRange(new EventArgs[]
                        {
                            CachedEventArgs.CountPropertyChanged,
                            CachedEventArgs.IndexerPropertyChanged,
                            Diff.CreateRemoveEventArgs(mock, 0)
                        });
                        CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                    }
                }
            }

            [Test]
            public void Replace()
            {
                var model1 = new Model(1);
                var model2 = new Model(2);
                var model3 = new Model(3);
                var source = new ObservableCollection<Model>(
                    new[]
                        {
                        model1,
                        model1,
                        model1,
                        model2,
                        model2,
                        model2,
                        model3,
                        model3,
                        model3,
                        });
                using (var view = source.AsMappingView(CreateMock, x => x.Object.Dispose()))
                {
                    using (var actual = view.SubscribeAll())
                    {
                        var old = view[0];
                        var @new = view[5];
                        source[0] = source[5];
                        CollectionAssert.AreEqual(source, view.Select(x => x.Object.Model));
                        var expected = new List<EventArgs>
                        {
                            CachedEventArgs.IndexerPropertyChanged,
                            Diff.CreateReplaceEventArgs(@new, old, 0)
                        };
                        CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                    }
                }
            }

            [Test]
            public void Move()
            {
                var model1 = new Model(1);
                var model2 = new Model(2);
                var model3 = new Model(3);
                var source = new ObservableCollection<Model>(
                    new[]
                        {
                        model1,
                        model1,
                        model1,
                        model2,
                        model2,
                        model2,
                        model3,
                        model3,
                        model3,
                        });
                using (var view = source.AsMappingView(CreateMock, x => x.Object.Dispose()))
                {
                    using (var actual = view.SubscribeAll())
                    {
                        source.Move(0, 4);
                        CollectionAssert.AreEqual(source, view.Select(x => x.Object.Model));
                        var expected = new List<EventArgs>
                        {
                            CachedEventArgs.IndexerPropertyChanged,
                            Diff.CreateMoveEventArgs(view[4], 4, 0)
                        };
                        CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                    }
                }
            }

            [Test]
            public void Clear()
            {
                var model1 = new Model(1);
                var model2 = new Model(2);
                var model3 = new Model(3);
                var source = new ObservableCollection<Model>(
                    new[]
                        {
                        model1,
                        model1,
                        model1,
                        model2,
                        model2,
                        model2,
                        model3,
                        model3,
                        model3,
                        });
                using (var view = source.AsMappingView(CreateMock, x => x.Object.Dispose()))
                {
                    using (var actual = view.SubscribeAll())
                    {
                        var mocks = view.ToArray();
                        foreach (var mock in mocks)
                        {
                            mock.Setup(x => x.Dispose());
                        }

                        source.Clear();
                        CollectionAssert.IsEmpty(view);
                        foreach (var mock in mocks)
                        {
                            mock.Verify(x => x.Dispose(), Times.Once);
                        }

                        CollectionAssert.AreEqual(source, view.Select(x => x.Object.Model));
                        var expected = new List<EventArgs>
                        {
                            CachedEventArgs.CountPropertyChanged,
                            CachedEventArgs.IndexerPropertyChanged,
                            CachedEventArgs.NotifyCollectionReset
                        };
                        CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                    }
                }
            }

            [Test]
            public void Dispose()
            {
                var model1 = new Model(1);
                var model2 = new Model(2);
                var model3 = new Model(3);
                var source = new ObservableCollection<Model>(
                    new[]
                        {
                        model1,
                        model1,
                        model1,
                        model2,
                        model2,
                        model2,
                        model3,
                        model3,
                        model3,
                        });
                using (var view = source.AsMappingView(CreateMock, x => x.Object.Dispose()))
                {
                    var mocks = view.ToArray();
                    foreach (var mock in mocks)
                    {
                        mock.Setup(x => x.Dispose());
                    }

                    view.Dispose();
                    foreach (var mock in mocks)
                    {
                        mock.Verify(x => x.Dispose(), Times.Once);
                    }

                    Assert.Throws<ObjectDisposedException>(() => CollectionAssert.IsEmpty(view));
                    Assert.Throws<ObjectDisposedException>(() => Assert.AreEqual(0, view.Count));
                }
            }

            private static Mock<IDisposableVm> CreateMock(Model model)
            {
                var mock = new Mock<IDisposableVm>(MockBehavior.Strict);
                mock.SetupGet(x => x.Model)
                    .Returns(model);
                return mock;
            }

            public interface IDisposableVm : IDisposable
            {
                Model Model { get; }
            }
        }
    }
}