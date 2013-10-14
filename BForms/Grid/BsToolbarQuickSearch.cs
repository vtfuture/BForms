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
    /// <summary>
    /// Grid toolbar inline search component
    /// </summary>
    public class BsToolbarQuickSearch : BaseComponent
    {
        private string placeholder = "search";

        public BsToolbarQuickSearch() { }

        public BsToolbarQuickSearch(ViewContext viewContext)
            : base(viewContext) { }

        /// <summary>
        /// Set input placeholder, default is "search"
        /// </summary>
        public BsToolbarQuickSearch Placeholder(string placeholder)
        {
            this.placeholder = placeholder;
            return this;
        } 

        public override string Render()
        {
            var inputGroupBuilder = new TagBuilder("div");
            inputGroupBuilder.AddCssClass("input-group bs-quickSearchContainer");


            var inputBuilder = new TagBuilder("input");
            inputBuilder.AddCssClass("form-control bs-text");
            inputBuilder.MergeAttribute("type", "search");
            inputBuilder.MergeAttribute("placeholder", this.placeholder);

            inputGroupBuilder.InnerHtml += inputBuilder.ToString(TagRenderMode.SelfClosing);

            return inputGroupBuilder.ToString();
        }
    }
}
