using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BForms.Mvc
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class BsGroupEditorAttribute : Attribute
    {
        public string Name { get; set; }

        public object Id { get; set; }

        public bool Selected { get; set; }

        /// <summary>
        /// Empty ctor
        /// </summary>
        public BsGroupEditorAttribute()
        {
        }
    }
}
