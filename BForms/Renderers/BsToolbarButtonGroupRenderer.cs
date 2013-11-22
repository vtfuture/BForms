using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BForms.Grid;

namespace BForms.Renderers
{
    public class BsToolbarButtonGroupRenderer<TToolbar> : BsBaseRenderer<BsToolbarButtonGroup<TToolbar>>
    {
        public BsToolbarButtonGroupRenderer()
        {

        }
        public BsToolbarButtonGroupRenderer(BsToolbarButtonGroup<TToolbar> model)
            : base(model)
        {
        }

        public override string Render()
        {
            var buttonGroupBuilder = new TagBuilder("div");
            buttonGroupBuilder.AddCssClass("btn-group");
            if (!string.IsNullOrEmpty(this.Builder.title))
            {
                buttonGroupBuilder.MergeAttribute("title", this.Builder.title);
            }
            var buttonArrowBuilder = new TagBuilder("a");
            buttonArrowBuilder.AddCssClass("btn dropdown-toggle");
            buttonArrowBuilder.MergeAttribute("data-toggle", "dropdown");
            buttonArrowBuilder.InnerHtml += (this.Builder.glyphIcon.HasValue ? GetGlyphicon(this.Builder.glyphIcon.Value) + " " : "") + this.Builder.name + "<span class=\"caret\"></span>";

            buttonGroupBuilder.InnerHtml += buttonArrowBuilder.ToString();


            var containerList = new TagBuilder("ul");
            containerList.AddCssClass("dropdown-menu");
            containerList.MergeAttribute("role", "menu");

            foreach (var item in this.Builder.Actions)
            {
                var defaultAction = item as BsToolbarAction<TToolbar>;
                var itemContainerBuilder = new TagBuilder("li");
                // remove btn from descriptorClass for styling
                if (defaultAction != null)
                {
                    var descriptorClass = defaultAction.GetDescriptorClass().Replace("btn ", "");
                    defaultAction.DescriptorClass(descriptorClass);
                    itemContainerBuilder.InnerHtml = item.ToString();
                }
                else
                {
                    itemContainerBuilder.InnerHtml = item.ToString();
                }
                containerList.InnerHtml += itemContainerBuilder.ToString();
            }

            buttonGroupBuilder.InnerHtml += containerList.ToString();

            return buttonGroupBuilder.ToString();
        }
    }
}
