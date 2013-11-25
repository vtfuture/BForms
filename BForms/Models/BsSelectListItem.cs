using System.Collections.Generic;
using System.Web.Mvc;

namespace BForms.Models
{
    /// <summary>
    /// Represents the selected item in an instance of the BsSelectList
    /// </summary>
    public class BsSelectListItem : SelectListItem
    {
        /// <summary>
        /// Option group unique ID
        /// </summary>
        public string GroupKey { get; set; }

        /// <summary>
        /// Option group display name
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// The dictionary items are serialized in html as data- attributes
        /// </summary>
        public Dictionary<string, string> Data { get; set; }
    }
}
