﻿namespace Gu.Reactive.Demo
{
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for ObservableFixedSizeQueueView.xaml
    /// </summary>
    public partial class ObservableFixedSizeQueueView : UserControl
    {
        public ObservableFixedSizeQueueView()
        {
            this.InitializeComponent();
            this.DataContext = new ObservableFixedSizeQueueViewModel();
        }
    }
}
