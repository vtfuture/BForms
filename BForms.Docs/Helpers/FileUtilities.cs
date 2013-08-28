using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;

using JetBrains.Annotations;

namespace BForms.Docs.Helpers
{
    public static class FileUtilities
    {
        public static MvcHtmlString InsertFileContent(this HtmlHelper helper, [PathReference] string path)
        {
            var absPath = HttpContext.Current.Server.MapPath(path);
            var text = HttpUtility.HtmlEncode(File.ReadAllText(absPath));
            return new MvcHtmlString(text);
        }

        public static MvcHtmlString InsertFileContentCached(this HtmlHelper helper, [PathReference] string path, int duration = 0)
        {
            if (Convert.ToBoolean(ConfigurationManager.AppSettings["IgnoreCacheForInsertFileContentCached"]))
            {
                return InsertFileContent(helper, path);
            }
            var cacheKey = "insertFile|" + path.GetHashCode();
            var cached = HttpRuntime.Cache.Get(cacheKey) as MvcHtmlString;
            if (cached != null)
            {
                return cached;
            }
            var result = InsertFileContent(helper, path);
            HttpRuntime.Cache.Add(cacheKey, 
                result,
                null /*dependencies*/,
                Cache.NoAbsoluteExpiration,
                duration == 0 ? Cache.NoSlidingExpiration : TimeSpan.FromSeconds(duration),
                CacheItemPriority.Normal,
                null /*onRemoveCallback*/);
            return result;
        }
    }
}

