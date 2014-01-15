using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BForms.Models;
using BForms.Resources;

namespace BForms.Utilities
{
    public static class BsUIManager
    {
        #region Properties
        private static BsTheme? globalTheme;
        #endregion

        #region Public
        /// <summary>
        /// Sets the global theme
        /// </summary>
        /// <param name="theme"></param>
        public static void Theme(BsTheme theme)
        {
            globalTheme = theme;
        }

        /// <summary>
        /// Gets the global theme
        /// </summary>
        /// <returns></returns>
        public static BsTheme GetGlobalTheme()
        {
            return globalTheme ?? BsTheme.Default;
        }
        #endregion
    }
}
