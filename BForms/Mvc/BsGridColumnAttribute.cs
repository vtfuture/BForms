using System;
using BForms.Models;

namespace BForms.Mvc
{
    /// <summary>
    /// BForms grid column descriptor
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class BsGridColumnAttribute : Attribute
    {
        private bool hasOrder = true;

        /// <summary>
        /// Wraps column header in a html node. Default is true
        /// </summary>
        public bool IsSortable
        {
            get
            {
                return this.hasOrder;
            }
            set
            {
                this.hasOrder = value;
            }
        }

        private bool isEditable = true;

        /// <summary>
        /// Wraps cell in editable container
        /// </summary>
        public bool IsEditable
        {
            get
            {
                return this.isEditable;
            }
            set
            {
                this.isEditable = value;
            }
        }

        /// <summary>
        /// Column order
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Column width expected values 1-12
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Empty ctor
        /// </summary>
        public BsGridColumnAttribute()
        {

        }
    }
}
