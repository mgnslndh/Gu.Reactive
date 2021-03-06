namespace Gu.Reactive
{
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Gu.Reactive.Internals;

    internal class MemberExpressionComparer : IEqualityComparer<MemberExpression>
    {
        bool IEqualityComparer<MemberExpression>.Equals(MemberExpression x, MemberExpression y) => Equals(x, y);

        int IEqualityComparer<MemberExpression>.GetHashCode(MemberExpression obj) => GetHashCode(obj);

        internal static bool Equals(MemberExpression x, MemberExpression y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return x.Member == y.Member &&
                   x.Expression.Type == y.Expression.Type;
        }

        internal static int GetHashCode(MemberExpression obj)
        {
            Ensure.NotNull(obj, nameof(obj));
            return obj.Member.GetHashCode() * 397 ^ obj.Expression.Type.GetHashCode();
        }
    }
}