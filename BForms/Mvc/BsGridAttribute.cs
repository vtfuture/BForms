using System;
using BForms.Grid;
using BForms.Models;

namespace BForms.Mvc
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
        /// BForms Theme
        /// </summary>
        public BsTheme Theme { get; set; }

        /// <summary>
        /// Empty ctor
        /// </summary>
        public BsGridAttribute()
        {
            
        }
    }
}
