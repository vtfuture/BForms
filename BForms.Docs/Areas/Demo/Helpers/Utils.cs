using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BForms.Models;
using BForms.Docs.Areas.Demo.Models;

namespace BForms.Docs.Areas.Demo.Helpers
{
    public static class Utils
    {
        public static string GetRoleIcon<T>(this HtmlHelper<T> helper, ProjectRole role)
        {
            switch (role)
            {
                case ProjectRole.TeamLeader:
                    return "icn-three_stars";
                case ProjectRole.Developer:
                    return "icn-two_stars";
                default:
                    return "icn-one_star";
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