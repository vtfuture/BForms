using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BForms.Utilities
{
    public static class BsResourceManager
    {
        #region Properties
        private static Dictionary<string, string> _localResources = new Dictionary<string, string>
        {
            { "NoResults", "There are no results" },
            { "Of", "of" },
            { "Items", "items" },
            { "ResultsPerPage", "results per page" }
        };

        private static ResourceManager _resourceManager;
        #endregion

        #region Public
        public static void Register(ResourceManager resourceManager)
        {
            if (_resourceManager == null)
            {
                _resourceManager = resourceManager;
            }
        }
        #endregion

        #region Internal
        internal static string Resource(string key)
        {
            if (_resourceManager != null)
            {
                var resource = _resourceManager.GetString(key);

                if (!string.IsNullOrEmpty(resource))
                {
                    return resource;
                }
            }
            return _localResources[key];
        }
        #endregion
    }
}
