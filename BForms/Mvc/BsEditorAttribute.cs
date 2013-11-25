using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BForms.Mvc
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class BsEditorTabAttribute : Attribute
    {
        public string Name { get; set; }

        public object Id { get; set; }

        public bool Selected { get; set; }

        public bool Editable { get; set; }

        /// <summary>
        /// Empty ctor
        /// </summary>
        public BsEditorTabAttribute()
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class BsEditorGroupAttribute : Attribute
    {
        public object Id { get; set; }

        /// <summary>
        /// Empty ctor
        /// </summary>
        public BsEditorGroupAttribute()
        {

        }
    }
}
