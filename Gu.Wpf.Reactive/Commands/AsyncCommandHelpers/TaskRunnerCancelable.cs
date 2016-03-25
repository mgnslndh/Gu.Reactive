namespace Gu.Wpf.Reactive
{
    using System;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Gu.Reactive;
    using Gu.Reactive.Internals;

    public class TaskRunnerCancelable : TaskRunnerBase, ITaskRunner
    {
        private readonly Func<CancellationToken, Task> _action;
        private readonly SerialDisposable _cancellationSubscription = new SerialDisposable();

        private CancellationTokenSource _cancellationTokenSource;
        public TaskRunnerCancelable(Func<CancellationToken, Task> action)
        {
            Ensure.NotNull(action, nameof(action));
            _action = action;

            var observable = Observable.Merge<object>(this.ObservePropertyChangedSlim(nameof(CanCancel)),
                                                      this.CanRunCondition.ObserveIsSatisfiedChanged());
            CanCancelCondition = new Condition(observable, () => CanCancel) { Name = "CanCancel" };
        }

        public void Run()
        {
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationSubscription.Disposable = _cancellationTokenSource.Token.AsObservable()
                                                                           .Subscribe(_ => OnPropertyChanged(nameof(CanCancel)));
            TaskCompletion = new NotifyTaskCompletion(_action(_cancellationTokenSource.Token));
        }

        public bool? CanCancel
        {
            get
            {
                if (CanRun()== true)
                {
                    return false;
                }
                var cts = _cancellationTokenSource;
                if (cts == null)
                {
                    return false;
                }
                return !cts.IsCancellationRequested;
            }
        }

        public void Cancel()
        {
            _cancellationTokenSource?.Cancel();
        }

        public override ICondition CanCancelCondition { get; }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _cancellationTokenSource.Dispose();
                _cancellationSubscription.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}