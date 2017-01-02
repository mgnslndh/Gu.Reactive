﻿namespace Gu.Reactive.Demo
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Wpf.Reactive;

    public class AsyncCommandsViewModel : INotifyPropertyChanged
    {
        private int delay = 500;

        private int count;

        public AsyncCommandsViewModel()
        {
            this.AsyncCommand = new AsyncCommand(this.SimpleTask);

            this.AsyncCancelableCommand = new AsyncCommand(this.CancelableTask);

            this.AsyncParameterCommand = new AsyncCommand<string>(this.ParameterTask);
            this.AsyncCancelableParameterCommand = new AsyncCommand<string>(this.CancelableParameterTask);

            this.AsyncThrowCommand = new AsyncCommand(this.VoidTaskThrowMethod);
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public AsyncCommand AsyncCommand { get; }

        public AsyncCommand AsyncCancelableCommand { get; }

        public AsyncCommand<string> AsyncParameterCommand { get; }

        public AsyncCommand<string> AsyncCancelableParameterCommand { get; }

        public AsyncCommand AsyncThrowCommand { get; }

        public int Count
        {
            get { return this.count; }

            private set
            {
                if (value == this.count)
                {
                    return;
                }

                this.count = value;
                this.OnPropertyChanged();
            }
        }

        public int Delay
        {
            get
            {
                return this.delay;
            }

            set
            {
                if (value == this.delay)
                {
                    return;
                }

                this.delay = value;
                this.OnPropertyChanged();
            }
        }

        private async Task SimpleTask()
        {
            await Task.Delay(this.Delay).ConfigureAwait(false);
        }

        private async Task CancelableTask(CancellationToken token)
        {
            this.Count = 0;
            for (int i = 0; i < 5; i++)
            {
                token.ThrowIfCancellationRequested();
                this.Count++;
                await Task.Delay(this.Delay, token).ConfigureAwait(false);
            }
        }

        private Task ParameterTask(string arg)
        {
            return this.SimpleTask();
        }

        private Task CancelableParameterTask(string arg, CancellationToken token)
        {
            return this.CancelableTask(token);
        }

        public async Task VoidTaskThrowMethod()
        {
            await Task.Delay(this.Delay).ConfigureAwait(false);
            throw new Exception("Something went wrong");
        }


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
