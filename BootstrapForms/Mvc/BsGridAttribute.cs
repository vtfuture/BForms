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
        /// <summary>
        /// Flag for grid row container 
        /// </summary>
        public bool HasDetails { get; set; }

        /// <summary>
        /// Pager default size
        /// </summary>
        public int DefaultPageSize { get; set; }

        /// <summary>
        /// Empty ctor
        /// </summary>
        public BsGridAttribute()
        {
            
        }
    }
}
