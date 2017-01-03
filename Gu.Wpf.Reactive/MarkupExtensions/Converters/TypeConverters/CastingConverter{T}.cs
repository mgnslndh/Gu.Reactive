﻿#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements must be documented
namespace Gu.Wpf.Reactive
{
    using System;
    using System.Globalization;

    [Obsolete("To be removed.")]
    public class CastingConverter<T> : ITypeConverter<T>
    {
        private readonly ITypeConverter<object> inner;

        public CastingConverter(ITypeConverter<object> inner)
        {
            this.inner = inner;
        }

        public bool IsValid(object value)
        {
            return this.inner.IsValid(value);
        }

        public bool CanConvertTo(object value, CultureInfo culture)
        {
            return this.inner.CanConvertTo(value, culture);
        }

        public T ConvertTo(object value, CultureInfo culture)
        {
            return (T)this.inner.ConvertTo(value, culture);
        }

        object ITypeConverter.ConvertTo(object value, CultureInfo culture)
        {
            return this.ConvertTo(value, culture);
        }
    }
}
