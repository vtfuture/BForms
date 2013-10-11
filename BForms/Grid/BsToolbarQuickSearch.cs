using BForms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BForms.Grid
{
    public class BsToolbarQuickSearch : BaseComponent
    {
        private Glyphicon? glyphIcon;

        public BsToolbarQuickSearch() { }

        public BsToolbarQuickSearch(ViewContext viewContext)
            : base(viewContext) { }


        public override string Render()
        {
            var inputGroupBuilder = new TagBuilder("div");
            inputGroupBuilder.MergeAttribute("class", "input-group");


            var inputBuilder = new TagBuilder("input");
            inputBuilder.MergeAttribute("class", "form-control bs-text");
            inputBuilder.MergeAttribute("type", "text");
            inputBuilder.MergeAttribute("placeholder", "search");
            inputBuilder.MergeAttribute("id", "quick_search");

            inputGroupBuilder.InnerHtml += inputBuilder.ToString(TagRenderMode.SelfClosing);

            return inputGroupBuilder.ToString();
        }
    }
}
