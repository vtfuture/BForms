using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BForms.Grid
{
    public static class BsGridResourceManager
    {
        private static Dictionary<string, string> _localResources;
        private static Dictionary<string, string> _importedResources;
        private static Type _type {get; set;}

        public static void Register(Type resourceType)
        {
            if (_type == null)
            {
                 _type = resourceType;
                 _localResources = new Dictionary<string, string>
                 {
                     { "NoResults", "There are no results" },
                     { "Of", "of" },
                     { "Items", "items" },
                     { "ResultsPerPage", "results per page" }
                 };
                 _importedResources = new Dictionary<string, string>();
            }
        }

        internal static string Resource(string key)
        {
            if (_type == null)
            {
                throw new Exception("Register resource type in app start global.asx");
            }

            if (_importedResources.ContainsKey(key))
            {
                var imported = _importedResources[key];

                if (imported != null)
                {
                    return imported;
                }
            }

            var prop = _type.GetProperty(key);

            if (prop != null)
            {
                var val = prop.GetValue(null, null);

                if (val != null)
                {
                    var result = val as string;

                    _importedResources[key] = result;

                    return result;
                }
            }

            return _localResources[key];
        }
    }
}
