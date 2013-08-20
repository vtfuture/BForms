using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BootstrapForms.Models;

namespace BForms.Docs.Areas.Demo.Mock
{
    public static class Lists
    {
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

        public static BsSelectList<T> AllNotificationTypes<T>()
        {
            var list = new BsSelectList<T>();

            list.Items.Add(new BsSelectListItem() { Text = "Never", Value = "1" });
            list.Items.Add(new BsSelectListItem() { Text = "Daily", Value = "2" });
            list.Items.Add(new BsSelectListItem() { Text = "Weekly", Value = "3" });
            list.Items.Add(new BsSelectListItem() { Text = "Monthly", Value = "4" });


            return list;
        }

        public static BsSelectList<T> AllTech<T>()
        {
            var list = new BsSelectList<T>();

            list.Items.Add(new BsSelectListItem() { Text = "ASP.NET MVC", Value = "1", GroupKey = "server", GroupName = "Back-end" });
            list.Items.Add(new BsSelectListItem() { Text = "ASP.NET WebApi", Value = "2", GroupKey = "server", GroupName = "Back-end" });
            list.Items.Add(new BsSelectListItem() { Text = "WCF", Value = "3", GroupKey = "server", GroupName = "Back-end" });
            list.Items.Add(new BsSelectListItem() { Text = "jQuery", Value = "4", GroupKey = "client", GroupName = "Front-end" });
            list.Items.Add(new BsSelectListItem() { Text = "Bootstrap", Value = "5", GroupKey = "client", GroupName = "Front-end" });
            list.Items.Add(new BsSelectListItem() { Text = "RequireJS", Value = "6", GroupKey = "client", GroupName = "Front-end" });

            return list;
        }

        public static BsSelectList<T> AllLanguages<T>()
        {
            var list = new BsSelectList<T>();

            list.Items.Add(new BsSelectListItem() { Text = "C#", Value = "1" });
            list.Items.Add(new BsSelectListItem() { Text = "Java", Value = "2" });
            list.Items.Add(new BsSelectListItem() { Text = "C++", Value = "3" });
            list.Items.Add(new BsSelectListItem() { Text = "Objective-C", Value = "4" });
            list.Items.Add(new BsSelectListItem() { Text = "Javascript", Value = "5" });
            list.Items.Add(new BsSelectListItem() { Text = "F#", Value = "6" });

            return list;
        }

        public static BsSelectList<T> AllGenders<T>()
        {
            var list = new BsSelectList<T>();

            list.Items.Add(new BsSelectListItem() { Text = "Male", Value = "1" });
            list.Items.Add(new BsSelectListItem() { Text = "Female", Value = "2" });

            return list;
        }
    }
}