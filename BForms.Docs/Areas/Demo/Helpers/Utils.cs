using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BForms.Models;
using BForms.Html;
using BForms.Docs.Areas.Demo.Models;

namespace BForms.Docs.Areas.Demo.Helpers
{
    public static class Utils
    {
        public static MvcHtmlString GetRoleIcon<T>(this HtmlHelper<T> helper, ProjectRole role)
        {
            var star = helper.BsGlyphicon(Glyphicon.Star);

            switch (role)
            {
                case ProjectRole.TeamLeader:
                    return new MvcHtmlString(star.ToString() + star + star);
                case ProjectRole.Developer:
                    return new MvcHtmlString(star.ToString() + star);
                default:
                    return new MvcHtmlString(star.ToString());
            }
        }

        public static string ToMonthNameDate(this DateTime current)
        {
            var month = current.Month;
            var dateTimeFormat = CultureInfo.CurrentCulture.DateTimeFormat;
            var monthName = dateTimeFormat.GetMonthName(month);

            return string.Format("{0} {1}", monthName, current.Year);
        }
    }
}