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
            var countryList = new SortedDictionary<string, string>();
            // Iterate the Framework Cultures...
            foreach (var ci in CultureInfo.GetCultures(CultureTypes.FrameworkCultures))
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
                var newKeyValuePair = new KeyValuePair<string, string>(ri.EnglishName, ri.ThreeLetterISORegionName);

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

            list.Items.Add(new BsSelectListItem() { Text = "IIS", Value = "1", GroupKey = "server", GroupName = "Windows" });
            list.Items.Add(new BsSelectListItem() { Text = "SQL Server", Value = "2", GroupKey = "server", GroupName = "Windows" });
            list.Items.Add(new BsSelectListItem() { Text = "Azure", Value = "3", GroupKey = "server", GroupName = "Windows" });
            list.Items.Add(new BsSelectListItem() { Text = "Apache", Value = "4", GroupKey = "client", GroupName = "Linux" });
            list.Items.Add(new BsSelectListItem() { Text = "SOLR", Value = "5", GroupKey = "client", GroupName = "Linux" });
            list.Items.Add(new BsSelectListItem() { Text = "Lucene", Value = "6", GroupKey = "client", GroupName = "Linux" });

            return list;
        }

        public static BsSelectList<T> AllAsp<T>()
        {
            var list = new BsSelectList<T>();

            list.Items.Add(new BsSelectListItem() { Text = "MVC", Value = "1" });
            list.Items.Add(new BsSelectListItem() { Text = "Razor", Value = "2" });
            list.Items.Add(new BsSelectListItem() { Text = "Web Forms", Value = "3" });
            list.Items.Add(new BsSelectListItem() { Text = "SignalR", Value = "4" });
            list.Items.Add(new BsSelectListItem() { Text = "Web Api", Value = "5" });

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
            list.Items.Add(new BsSelectListItem() { Text = "Fortran", Value = "6" });
            list.Items.Add(new BsSelectListItem() { Text = "Ruby", Value = "7" });
            list.Items.Add(new BsSelectListItem() { Text = "PHP", Value = "8" });
            list.Items.Add(new BsSelectListItem() { Text = "Python", Value = "9" });
            list.Items.Add(new BsSelectListItem() { Text = "Scala", Value = "10" });
            list.Items.Add(new BsSelectListItem() { Text = "Ada", Value = "11" });
            list.Items.Add(new BsSelectListItem() { Text = "ActionScript", Value = "12" });
            list.Items.Add(new BsSelectListItem() { Text = "Visual Basic", Value = "13" });

            return list;
        }

        public static BsSelectList<T> AllIde<T>()
        {
            var list = new BsSelectList<T>();

            list.Items.Add(new BsSelectListItem() { Text = "Visual Studio", Value = "1" });
            list.Items.Add(new BsSelectListItem() { Text = "IntelliJ IDEA", Value = "2" });
            list.Items.Add(new BsSelectListItem() { Text = "Eclipse", Value = "3" });
            list.Items.Add(new BsSelectListItem() { Text = "Xcode", Value = "4" });
            list.Items.Add(new BsSelectListItem() { Text = "MonoDevelop", Value = "5" });
            list.Items.Add(new BsSelectListItem() { Text = "NetBeans", Value = "6" });
            list.Items.Add(new BsSelectListItem() { Text = "Anjuta", Value = "7" });

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