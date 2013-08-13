using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BootstrapForms.Models
{
    /// <summary>
    /// SelectListItem for grouped DropDownList and ListBox
    /// </summary>
    public class BsGroupedSelectListItem : SelectListItem
    {
        /// <summary>
        /// Group unique id
        /// </summary>
        public string GroupKey { get; set; }
        /// <summary>
        /// Group display name
        /// </summary>
        public string GroupName { get; set; }
    }
}
