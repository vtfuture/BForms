using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BootstrapForms.Models
{
    /// <summary>
    /// BForms input descriptor
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class BsInputAttribute : Attribute
    {
        /// <summary>
        /// BForms input type works with DataAnnotations.DataType to create HTML5 inputs
        /// </summary>
        public BsInputType BsInputType { get; set; }

        /// <summary>
        /// Let's you specify custom css classes for each input
        /// </summary>
        public string Css { get; set; }
    }
}
