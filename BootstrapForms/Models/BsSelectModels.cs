using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BootstrapForms.Models
{
    /// <summary>
    /// SelectListItem for grouped DropDown
    /// </summary>
    public class BsGroupedSelectListItem : SelectListItem
    {
        public string GroupKey { get; set; }
        public string GroupName { get; set; }
    }
}
