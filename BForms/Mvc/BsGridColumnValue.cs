using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BForms.Mvc
{
    public class BsGridColumnValue
    {
    }

    /// <summary>
    /// Used for rendering bs grid columns with data-value attributes
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TDisplay"></typeparam>
    public class BsGridColumnValue<TValue, TDisplay> : BsGridColumnValue
    {
        /// <summary>
        /// Used for no offset pagination
        /// </summary>
        public TValue ItemValue { get; set; }

        /// <summary>
        /// Used for rendering
        /// </summary>
        public TDisplay DisplayValue { get; set; }

        /// <summary>
        /// Parameterless constructor
        /// </summary>
        public BsGridColumnValue()
        {

        }

        /// <summary>
        /// Sets ItemValue and DisplayValue
        /// </summary>
        /// <param name="value"></param>
        /// <param name="display"></param>
        public BsGridColumnValue(TValue value, TDisplay display)
        {
            this.ItemValue = value;
            this.DisplayValue = display;
        }
    }
}
