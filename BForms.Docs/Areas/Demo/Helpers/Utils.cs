using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
    }
}