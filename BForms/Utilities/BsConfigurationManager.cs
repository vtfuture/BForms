using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BForms.Utilities
{
    /// <summary>
    /// Used for configuration, ie if debugging is enabled
    /// </summary>
    public static class BsConfigurationManager
    {
        #region Properties
        private static bool isRelease = false;
        #endregion

        #region Methods
        /// <summary>
        /// Sets current configuration
        /// </summary>
        /// <param name="release"></param>
        public static void Release(bool release)
        {
            isRelease = release;
        }

        public static bool GetRelease()
        {
            return isRelease;
        }
        #endregion
    }
}
