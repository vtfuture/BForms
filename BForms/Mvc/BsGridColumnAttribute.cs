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
        private bool _hasOrder = true;

        /// <summary>
        /// Wraps column header in a html node. Default is true
        /// </summary>
        public bool IsSortable
        {
            get
            {
                return _hasOrder;
            }
            set
            {
                _hasOrder = value;
            }
        }

        private bool _isEditable = true;

        /// <summary>
        /// Wraps cell in editable container
        /// </summary>
        public bool IsEditable
        {
            get
            {
                return _isEditable;
            }
            set
            {
                _isEditable = value;
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

        private BsGridColumnUsage _usage = BsGridColumnUsage.Both;

        /// <summary>
        /// Column usage expected values Both/Excel/Html
        /// </summary>
        public BsGridColumnUsage Usage
        {
            get
            {
                return _usage;
            }
            set
            {
                _usage = value;
            }
        }

        /// <summary>
        /// Empty ctor
        /// </summary>
        public BsGridColumnAttribute()
        {

        }
    }
}
