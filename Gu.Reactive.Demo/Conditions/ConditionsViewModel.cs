﻿namespace Gu.Reactive.Demo
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using Gu.Wpf.Reactive;

    public sealed class ConditionsViewModel : INotifyPropertyChanged, IDisposable
    {
        public static readonly ConditionsViewModel Instance = new ConditionsViewModel();

        private readonly List<ICondition> conditions;
        private bool disposed;

        private ConditionsViewModel()
        {
            this.conditions = new List<ICondition>
            {
                this.IsLeftDoorOpenCondition,
                this.IsLeftDoorClosedCondition,
                this.IsMotorRunningCondition,
                new IsAnyDoorOpen(),
                new IsAnyDoorOpen().Negate(),
                this.CanStartCondition,
                this.CanStartCondition.Negate(),
                new SyncErrorCondition()
            };

            this.StartCommand = new ConditionRelayCommand(() => ConditionState.Instance.IsMotorRunning = true, this.CanStartCondition);

            this.StopCommand = new ConditionRelayCommand(() => ConditionState.Instance.IsMotorRunning = false, this.CanStopCondition);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICondition IsLeftDoorOpenCondition { get; } = new IsLeftDoorOpen();

        public ICondition IsRightDoorOpenCondition { get; } = new IsRightDoorOpen();

        public ICondition IsBackDoorOpenCondition { get; } = new IsBackDoorOpen();

        public ICondition IsLeftDoorClosedCondition { get; } = new IsLeftDoorOpen().Negate();

        public ICondition IsMotorRunningCondition { get; } = new IsMotorRunning();

        public ICondition CanStartCondition { get; } = new CanStart();

        public ICondition CanStopCondition { get; } = new IsMotorRunning();

        public ConditionRelayCommand StartCommand { get; }

        public ConditionRelayCommand StopCommand { get; }

        public IEnumerable<ICondition> Conditions => this.conditions;

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            this.IsLeftDoorOpenCondition.Dispose();
            this.IsRightDoorOpenCondition.Dispose();
            this.IsBackDoorOpenCondition.Dispose();
            this.IsLeftDoorClosedCondition.Dispose();
            this.IsMotorRunningCondition.Dispose();
            this.CanStartCondition.Dispose();
            this.CanStopCondition.Dispose();
            this.StartCommand.Dispose();
            this.StopCommand.Dispose();
        }

        // ReSharper disable once UnusedMember.Local
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
