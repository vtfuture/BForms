using System;
using BootstrapForms.Models;

namespace BootstrapForms.Attributes
{
    /// <summary>
    /// BForms control descriptor
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class BsControlAttribute : Attribute
    {
        /// <summary>
        /// BForms input type works with DataAnnotations.DataType to create HTML5 inputs, when DataType is not specified, it will render type="text"
        /// </summary>
        public BsControlType ControlType { get; set; }

        /// <summary>
        /// Let's you specify custom css classes for each input
        /// </summary>
        public string CssClass { get; set; }
    }
}
