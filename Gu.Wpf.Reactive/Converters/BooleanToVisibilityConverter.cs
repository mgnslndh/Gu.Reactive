﻿namespace Gu.Wpf.Reactive
{
    using System.Windows;

    /// <summary>
    /// Usage:
    /// Visibility="{Binding SomeProperty, Converter={common:BooleanToVisibilityConverter WhenTrue=Visible, WhenFalse=Collapsed}}"
    /// No resource declaration is necessary
    /// </summary>
    public class BooleanToVisibilityConverter : MarkupConverter<bool?, Visibility>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BooleanToVisibilityConverter"/> class
        /// </summary>
        public BooleanToVisibilityConverter()
        {
            WhenTrue = Visibility.Visible;
            WhenFalse = Visibility.Collapsed;
            WhenNull = Visibility.Collapsed;
        }

        /// <summary>
        /// Gets or sets the value to be returned when the converted value is true
        /// </summary>
        public Visibility WhenTrue { get; set; }

        /// <summary>
        /// Gets or sets the value to be returned when the converted value is false
        /// </summary>
        public Visibility WhenFalse { get; set; }

        /// <summary>
        /// Gets or sets the value to be returned when the converted value is null
        /// </summary>
        public Visibility WhenNull { get; set; }

        protected override Visibility Convert(bool? value)
        {
            return value == true ? WhenTrue : WhenFalse;
        }
    }
}