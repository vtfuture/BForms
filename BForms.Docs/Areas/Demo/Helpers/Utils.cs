using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BForms.Models;
using BForms.Html;
using BForms.Docs.Areas.Demo.Models;
using System.Reflection;

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

        public static string BFromsVersion()
        {
            try
            {
                var version = Assembly.GetExecutingAssembly().GetReferencedAssemblies().Where(a => a.Name.Contains("BForms")).First().Version.ToString();

                string filePath = System.Reflection.Assembly.GetCallingAssembly().Location;
                const int c_PeHeaderOffset = 60;
                const int c_LinkerTimestampOffset = 8;
                byte[] b = new byte[2048];
                System.IO.Stream s = null;

                try
                {
                    s = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                    s.Read(b, 0, 2048);
                }
                finally
                {
                    if (s != null)
                    {
                        s.Close();
                    }
                }

                int i = System.BitConverter.ToInt32(b, c_PeHeaderOffset);
                int secondsSince1970 = System.BitConverter.ToInt32(b, i + c_LinkerTimestampOffset);
                DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0);
                dt = dt.AddSeconds(secondsSince1970);
                dt = dt.AddHours(TimeZone.CurrentTimeZone.GetUtcOffset(dt).Hours);

                return "v" + version + " at " + dt.ToString("dd.MM.yyyy hh:mm");
            }
            catch (Exception)
            {

            }

            return "v1.0.0";
        }

        public static DateTime BFromsDate()
        {
            try
            {
                var version = Assembly.GetExecutingAssembly().GetReferencedAssemblies().Where(a => a.Name.Contains("BForms")).First().Version.ToString();

                string filePath = System.Reflection.Assembly.GetCallingAssembly().Location;
                const int c_PeHeaderOffset = 60;
                const int c_LinkerTimestampOffset = 8;
                byte[] b = new byte[2048];
                System.IO.Stream s = null;

                try
                {
                    s = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                    s.Read(b, 0, 2048);
                }
                finally
                {
                    if (s != null)
                    {
                        s.Close();
                    }
                }

                int i = System.BitConverter.ToInt32(b, c_PeHeaderOffset);
                int secondsSince1970 = System.BitConverter.ToInt32(b, i + c_LinkerTimestampOffset);
                DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0);
                dt = dt.AddSeconds(secondsSince1970);
                dt = dt.AddHours(TimeZone.CurrentTimeZone.GetUtcOffset(dt).Hours);

                return dt;
            }
            catch (Exception)
            {

            }

            return DateTime.Now;
        }
    }
}