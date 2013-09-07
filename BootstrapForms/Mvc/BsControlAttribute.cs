using System;
using BootstrapForms.Models;

namespace BootstrapForms.Mvc
{
    /// <summary>
    /// BForms control descriptor
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class BsControlAttribute : Attribute
    {
        /// <summary>
        /// Specifies the name of an BFroms control type to associate with an input HTML field
        /// </summary>
        public BsControlType ControlType { get; set; }

        /// <summary>
        /// Sets custom css
        /// </summary>
        public string CssClass { get; set; }

        /// <summary>
        /// Sets readonly html attribute
        /// </summary>
        public bool IsReadonly { get; set; }

        /// <summary>
        /// Empty ctor
        /// </summary>
        public BsControlAttribute()
        {

        }

        /// <summary>
        /// Specifies the name of an BFroms control type to associate with an input HTML field
        /// </summary>
        public BsControlAttribute(BsControlType controlType)
        {
            ControlType = controlType;
        }
    }
}
