﻿namespace Gu.Reactive.Analyzers.Tests.GUREA02ObservableAndCriteriaMustMatchTests
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public class Diagnostics
    {
        private const string FooCode = @"
namespace RoslynSandbox
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class Foo : INotifyPropertyChanged
    {
        private int value1;
        private int value2;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Value1
        {
            get
            {
                return this.value1;
            }

            set
            {
                if (value == this.value1)
                {
                    return;
                }

                this.value1 = value;
                this.OnPropertyChanged();
            }
        }

        public int Value2
        {
            get
            {
                return this.value2;
            }

            set
            {
                if (value == this.value2)
                {
                    return;
                }

                this.value2 = value;
                this.OnPropertyChanged();
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}";

        static Diagnostics()
        {
            AnalyzerAssert.MetadataReference.AddRange(MetadataReferences.All);
        }

        [Test]
        public void ObservingDifferentThanUsedInCriteria()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using Gu.Reactive;

    public class FooCondition : Condition
    {
        public FooCondition(Foo foo)
            ↓: base(
                foo.ObservePropertyChangedSlim(x => x.Value1),
                () => foo.Value2 == 2)
        {
        }
    }
}";
            var expected = "Observable and criteria must match.\r\n" +
                           "Observed:\r\n" +
                           "  RoslynSandbox.Foo.Value1\r\n" +
                           "Used in criteria:\r\n" +
                           "  RoslynSandbox.Foo.Value2\r\n" +
                           "Not observed:\r\n" +
                           "  RoslynSandbox.Foo.Value2";
            AnalyzerAssert.Diagnostics<GUREA02ObservableAndCriteriaMustMatch>(ExpectedMessage.Create(expected), FooCode, testCode);
        }
    }
}
