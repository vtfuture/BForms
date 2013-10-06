using System;
using BootstrapForms.Models;

namespace BootstrapForms.Mvc
{
    /// <summary>
    /// BForms grid column descriptor
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class BsGridAttribute : Attribute
    {
        public bool HasDetails { get; set; }

        public int DefaultPageSize { get; set; }

        /// <summary>
        /// Empty ctor
        /// </summary>
        public BsGridAttribute()
        {
            
        }
    }
}
