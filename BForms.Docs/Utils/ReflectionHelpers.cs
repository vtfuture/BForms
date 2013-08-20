using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BForms.Docs.Utils
{
    public static class ReflectionHelpers
    {
        /// <summary>
        /// Checks to see if a generic type is a subclass of a raw generic type. eg. List&lt;int&gt; for List&lt;&gt;
        /// </summary>
        /// <param name="generic">The generic without type specifiers, such as List&lt;&gt; or Dictionary&lt;,&gt;</param>
        /// <param name="toCheck">The generic subclass, such as List&lt;&gt; or Dictionary&lt;string,object&gt;</param>
        /// <returns></returns>
        internal static bool IsSubclassOfRawGeneric(this Type toCheck, Type generic)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        }
    }
}