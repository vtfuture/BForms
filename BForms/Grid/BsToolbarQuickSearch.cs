using BForms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BForms.Mvc;
using BForms.Renderers;

namespace BForms.Grid
{
    // Step 1: Inherit from BaseComponent
    /// <summary>
    /// Grid toolbar inline search component
    /// </summary>
    public class BsToolbarQuickSearch : BsBaseComponent
    {
        internal string placeholder = "search";

        public BsToolbarQuickSearch() 
        {
            this.renderer = new BsToolbarQuickSearchRenderer(this);
        }

        // Step 2: pass viewContext to BaseComponent - 
        // used for writing the output html
        public BsToolbarQuickSearch(ViewContext viewContext)
            : base(viewContext) 
        {
            this.renderer = new BsToolbarQuickSearchRenderer(this);
        }

        // Step 3: Add customization. In this case we can 
        // set the quick search input placeholder
        /// <summary>
        /// Set input placeholder, default is "search"
        /// </summary>
        public BsToolbarQuickSearch Placeholder(string placeholder)
        {
            this.placeholder = placeholder;
            //return this for fluent api
            return this;
        }
    }
}
