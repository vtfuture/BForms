using BForms.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BForms.Renderers
{
    public class BsBulkSelectorRenderer : BsBaseRenderer<BsBulkSelector>
    {
        public BsBulkSelectorRenderer(){}

        public BsBulkSelectorRenderer(BsBulkSelector builder)
            : base(builder)
        { 

        }

        public override string Render()
        {
            var bulkSelector = new TagBuilder("li");
            var bulkSelectorAnchor = new TagBuilder("a");
            bulkSelectorAnchor.MergeAttribute("href", "#");
            bulkSelectorAnchor.MergeAttribute("class", this.Builder.styleClass);
            bulkSelectorAnchor.InnerHtml += this.Builder.text;

            bulkSelector.InnerHtml += bulkSelectorAnchor.ToString();

            return bulkSelector.ToString();
        }
    }
}
