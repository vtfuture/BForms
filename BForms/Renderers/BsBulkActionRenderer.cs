using BForms.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BForms.Renderers
{
    public class BsBulkActionRenderer : BsBaseRenderer<BsBulkAction>
    {
        public BsBulkActionRenderer() { }

        public BsBulkActionRenderer(BsBulkAction builder)
            : base(builder)
        {

        }

        public override string Render()
        {
            var bulkButton = new TagBuilder("button");

            bulkButton.MergeAttributes(this.Builder.htmlAttributes);

            if (!String.IsNullOrEmpty(this.Builder.buttonClass))
            {
                bulkButton.AddCssClass(this.Builder.buttonClass);
            }

            bulkButton.AddCssClass("btn");

            if (this.Builder.ignore)
            {
                bulkButton.MergeAttribute("data-ignore", "true");
            }

            else
            {
                bulkButton.MergeAttribute("style", "display:none");
            }

            if (!String.IsNullOrEmpty(this.Builder.title))
            {
                bulkButton.MergeAttribute("title", this.Builder.title);
            }
           
            bulkButton.InnerHtml += (this.Builder.glyphIcon.HasValue ? GetGlyphicon(this.Builder.glyphIcon.Value) + " " : "") + this.Builder.text;

            return bulkButton.ToString();
        }
    }
}
