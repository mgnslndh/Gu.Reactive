namespace Gu.Reactive.PropertyPathStuff
{
    using System;
    using System.Reflection;

    public class ClassGetter<TSource, TValue> : Getter<TSource, TValue>
    {
        private readonly Func<TSource, TValue> getter;

        private ClassGetter(PropertyInfo property)
        {
            if (property.GetMethod == null)
            {
                throw new ArgumentException($"Expected get method to not be null. Property: {property}");
            }

            this.getter = (Func<TSource, TValue>)Delegate.CreateDelegate(typeof(Func<TSource, TValue>), property.GetMethod, true);
        }

        /// <inheritdoc/>
        public override TValue GetValue(TSource source)
        {
            return this.getter(source);
        }
    }
}