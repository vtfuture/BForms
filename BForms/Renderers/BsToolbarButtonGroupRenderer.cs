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
            if (this.Builder.Direction == BsToolbarGroupButtonDirection.Up)
                buttonGroupBuilder.AddCssClass("dropup");

            var buttonArrowBuilder = new TagBuilder("button");
            buttonArrowBuilder.AddCssClass("btn btn-default dropdown-toggle");
            buttonArrowBuilder.MergeAttribute("type", "button");
            buttonArrowBuilder.MergeAttribute("data-toggle", "dropdown");
            buttonArrowBuilder.InnerHtml = this.Builder.Name + "<span class=\"caret\"></span>";

            buttonGroupBuilder.InnerHtml += buttonArrowBuilder.ToString();


            var containerList = new TagBuilder("ul");
            containerList.AddCssClass("dropdown-menu");
            containerList.MergeAttribute("role", "menu");

            foreach (var item in this.Builder.Actions)
            {
                var itemGroupAction = item as BsToolbarItemGroupSeparator;
                if (itemGroupAction != null)
                {
                    var itemContainerBuilder = new TagBuilder("li");
                    itemContainerBuilder.AddCssClass(itemGroupAction.descriptorClass);
                    containerList.InnerHtml += itemContainerBuilder.ToString();
                }
                else
                {
                    var itemContainerBuilder = new TagBuilder("li") {InnerHtml = item.ToString()};
                    containerList.InnerHtml += itemContainerBuilder.ToString();
                }
            }

            buttonGroupBuilder.InnerHtml += containerList.ToString();

            return buttonGroupBuilder.ToString();
        }
    }
}
