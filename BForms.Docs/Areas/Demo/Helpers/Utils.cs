using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BootstrapForms.Models;

namespace BForms.Docs.Areas.Demo.Helpers
{
    public class Utils
    {
        public static List<SelectListItem> GetCounties()
        {
            SortedDictionary<string, string> countryList = new SortedDictionary<string, string>();
            // Iterate the Framework Cultures...
            foreach (CultureInfo ci in CultureInfo.GetCultures(CultureTypes.FrameworkCultures))
            {
                RegionInfo ri = null;
                try
                {
                    ri = new RegionInfo(ci.Name);
                }
                catch
                {
                    // If a RegionInfo object could not be created we don't want to use the CultureInfo
                    //    for the country list.
                    continue;
                }
                // Create new country dictionary entry.
                KeyValuePair<string, string> newKeyValuePair = new KeyValuePair<string, string>(ri.EnglishName, ri.ThreeLetterISORegionName);

                // If the country is not alreayd in the countryList add it...
                if (!(countryList.ContainsKey(ri.EnglishName)))
                {
                    countryList.Add(newKeyValuePair.Key, newKeyValuePair.Value);
                }
            }

            var list = new List<SelectListItem>();

            foreach (var item in countryList)
            {
                list.Add(new SelectListItem() { Text = item.Key, Value = item.Value });
            }

            return list;
        }

        public static BsSelectList<T> AllCounties<T>()
        {
            SortedDictionary<string, string> countryList = new SortedDictionary<string, string>();
            // Iterate the Framework Cultures...
            foreach (CultureInfo ci in CultureInfo.GetCultures(CultureTypes.FrameworkCultures))
            {
                RegionInfo ri = null;
                try
                {
                    ri = new RegionInfo(ci.Name);
                }
                catch
                {
                    // If a RegionInfo object could not be created we don't want to use the CultureInfo
                    //    for the country list.
                    continue;
                }
                // Create new country dictionary entry.
                KeyValuePair<string, string> newKeyValuePair = new KeyValuePair<string, string>(ri.EnglishName, ri.ThreeLetterISORegionName);

                // If the country is not alreayd in the countryList add it...
                if (!(countryList.ContainsKey(ri.EnglishName)))
                {
                    countryList.Add(newKeyValuePair.Key, newKeyValuePair.Value);
                }
            }

            var list = new BsSelectList<T>();

            foreach (var item in countryList)
            {
                list.Items.Add(new BsSelectListItem() { Text = item.Key, Value = item.Value });
            }

            return list;
        }

        public static BsSelectList<T> GetNotificationTypes<T>()
        {
            var list = new BsSelectList<T>();

            list.Items.Add(new BsSelectListItem() { Text = "Never", Value = "0" });
            list.Items.Add(new BsSelectListItem() { Text = "Daily", Value = "1" });
            list.Items.Add(new BsSelectListItem() { Text = "Weekly", Value = "2" });
            list.Items.Add(new BsSelectListItem() { Text = "Monthly", Value = "3" });


            return list;
        }

        public static BsSelectList<T> GetTech<T>()
        {
            var list = new BsSelectList<T>();

            list.Items.Add(new BsSelectListItem() { Text = "ASP.NET MVC", Value = "0", GroupKey = "server", GroupName = "Back-end" });
            list.Items.Add(new BsSelectListItem() { Text = "ASP.NET WebApi", Value = "1", GroupKey = "server", GroupName = "Back-end" });
            list.Items.Add(new BsSelectListItem() { Text = "WCF", Value = "2", GroupKey = "server", GroupName = "Back-end" });
            list.Items.Add(new BsSelectListItem() { Text = "jQuery", Value = "3", GroupKey = "client", GroupName = "Front-end" });
            list.Items.Add(new BsSelectListItem() { Text = "Bootstrap", Value = "4", GroupKey = "client", GroupName = "Front-end" });
            list.Items.Add(new BsSelectListItem() { Text = "RequireJS", Value = "5", GroupKey = "client", GroupName = "Front-end" });

            return list;
        }
    }
}