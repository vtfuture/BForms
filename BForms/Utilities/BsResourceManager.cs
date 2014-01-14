using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BForms.Resources;

namespace BForms.Utilities
{
    public static class BsResourceManager
    {
        #region Properties
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

            var localResource = BFormsResources.ResourceManager.GetString(key);

            return string.IsNullOrEmpty(localResource) ? string.Empty : localResource;
        }
        #endregion
    }
}
