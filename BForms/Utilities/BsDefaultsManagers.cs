using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Vml.Spreadsheet;

namespace BForms.Utilities
{
    /// <summary>
    /// Handles default values of BForms components
    /// </summary>
    public static class BsDefaultsManagers
    {
        #region Properties
        private static List<int> _pageSizeValues;
        #endregion

        #region Public
        /// <summary>
        /// Sets default page size values for BsGridPager
        /// </summary>
        /// <param name="sizeValues"></param>
        public static void PageSizeValues(List<int> sizeValues)
        {
            _pageSizeValues = sizeValues;
        }

        /// <summary>
        /// Gets the default page size values for BsGridPager
        /// </summary>
        /// <returns></returns>
        public static List<int> GetPageSizeValues()
        {
            return _pageSizeValues ?? new List<int>() { 5, 10, 50, 100, 500 };
        }
        #endregion
    }
}
