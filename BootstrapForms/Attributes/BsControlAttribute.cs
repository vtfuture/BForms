using System;
using BootstrapForms.Models;

namespace BootstrapForms.Attributes
{
    /// <summary>
    /// BForms control descriptor
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class BsControlAttribute : Attribute
    {
        /// <summary>
        /// Specifies the control type for BFroms HTML input and select elements
        /// </summary>
        public BsControlType ControlType { get; set; }

        /// <summary>
        /// Let's you specify custom css classes for each input
        /// </summary>
        public string CssClass { get; set; }

        public BsControlAttribute()
        {

        }

        /// <summary>
        /// Specifies the name of an BFroms control type to associate with an input HTML field
        /// </summary>
        public BsControlAttribute(BsControlType ControlType)
        {
            this.ControlType = ControlType;
        }
    }
}
