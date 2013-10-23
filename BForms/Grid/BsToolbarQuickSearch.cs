using BForms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BForms.Mvc;

namespace BForms.Grid
{
    // Step 1: Inherit from BaseComponent
    /// <summary>
    /// Grid toolbar inline search component
    /// </summary>
    public class BsToolbarQuickSearch : BaseComponent
    {
        private string placeholder = "search";

        public BsToolbarQuickSearch() { }

        // Step 2: pass viewContext to BaseComponent - 
        // used for writing the output html
        public BsToolbarQuickSearch(ViewContext viewContext)
            : base(viewContext) { }

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

        // Step 4: implement Render html function. Here you decide how your 
        // custom control will look like based on the customization settings
        /// <summary>
        /// Renders custom control
        /// </summary>
        public override string Render()
        {
            var inputGroupBuilder = new TagBuilder("div");
            inputGroupBuilder.AddCssClass("input-group bs-quick_search");

            var inputBuilder = new TagBuilder("input");
            inputBuilder.AddCssClass("form-control bs-text");
            inputBuilder.MergeAttribute("type", "search");
            inputBuilder.MergeAttribute("placeholder", this.placeholder);

            inputGroupBuilder.InnerHtml += inputBuilder.ToString(TagRenderMode.SelfClosing);

            return inputGroupBuilder.ToString();
        }
    }
}
