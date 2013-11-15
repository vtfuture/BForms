using BForms.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BForms.Renderers
{
    public class BsGridColumnRenderer<TRow> : BsBaseRenderer<BsGridColumn<TRow>> where TRow : new()
    {
        public BsGridColumnRenderer()
        {

        }

        public BsGridColumnRenderer(BsGridColumn<TRow> builder)
            : base(builder)
        { 

        }

        public override string Render()
        {
            var columnBuilder = new TagBuilder("div");

            if (this.Builder.Property != null && this.Builder.IsSortable)
            {
                var linkBuilder = new TagBuilder("a");
                linkBuilder.MergeAttribute("href", "#");
                linkBuilder.MergeAttribute("class", "bs-orderColumn");

                if (this.Builder.OrderType == BsOrderType.Ascending)
                {
                    linkBuilder.AddCssClass("sort_asc");
                }
                else if (this.Builder.OrderType == BsOrderType.Descending)
                {
                    linkBuilder.AddCssClass("sort_desc");
                }

                linkBuilder.InnerHtml = this.Builder.DisplayName;

                columnBuilder.InnerHtml += linkBuilder.ToString();
            }
            else
            {
                columnBuilder.InnerHtml += this.Builder.DisplayName;
            }

            if (this.Builder.IsEditable)
            {
                columnBuilder.InnerHtml += this.Builder.EditableContent;
            }

            if (this.Builder.htmlAttributes != null)
            {
                columnBuilder.MergeAttributes(this.Builder.htmlAttributes);
            }

            columnBuilder.AddCssClass(this.Builder.GetWidthClasses());

            columnBuilder.MergeAttribute("data-name", this.Builder.PrivateName);

            return columnBuilder.ToString();
        }
    }
}
