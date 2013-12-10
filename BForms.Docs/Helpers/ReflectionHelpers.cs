using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace BForms.Docs.Helpers
{
    public static class ReflectionHelpers
    {
        public static string GetPropertyName<T, TProp>(this T instance, Expression<Func<T, TProp>> selector)
        {
            return GetPropertyName(selector);
        }

        public static string GetPropertyName<T, TProp>(Expression<Func<T, TProp>> selector)
        {
            var member = selector.Body as MemberExpression;
            var unary = selector.Body as UnaryExpression;
            var memberInfo = member ?? (unary != null ? unary.Operand as MemberExpression : null);
            if (memberInfo == null)
            {
                throw new Exception("Could not get selector from specified expression.");
            }
            return memberInfo.Member.Name;
        }
    }
}