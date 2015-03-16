using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace BForms.Utilities
{
    internal static class MvcHelpers
    {
        internal static RouteValueDictionary ExtractRouteValues(this HtmlHelper htmlHelper)
        {
            var routeValues = htmlHelper.ViewContext.RouteData.Values;
            var query = htmlHelper.ViewContext.HttpContext.Request.QueryString;
            foreach (var item in query.AllKeys)
            {
                if (routeValues.All(r => r.Key != item))
                {
                    if (item != null && query[item] != null)
                        routeValues.Add(item, query[item]);
                }
            }

            return routeValues;
        }

        internal static void MergeClassAttribute(this TagBuilder tagBuilder, string value, IDictionary<string, object> dictAttr)
        {
            var key = "class";

            if (dictAttr != null && dictAttr.ContainsKey(key))
            {
                value += " " + dictAttr[key];
                dictAttr.Remove(key);
            }

            tagBuilder.MergeAttribute(key, value, true);
        }

        internal static string Serialize(object o)
        {
            return JsonConvert.SerializeObject(o);
        }
    }
}
