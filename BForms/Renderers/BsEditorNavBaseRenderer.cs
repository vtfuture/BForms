using BForms.GroupEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BForms.Renderers
{
    public class BsEditorNavBaseRenderer : BsBaseRenderer<BsEditorNavBuilder>
    {
        public BsEditorNavBaseRenderer(){}

        public BsEditorNavBaseRenderer(BsEditorNavBuilder builder)
            : base(builder)
        { 
        }

        public override string Render()
        {
            var nav = new TagBuilder("nav");

            nav.AddCssClass("navbar navbar-default");
            nav.MergeAttribute("role", "navigation");

            var navHeader = new TagBuilder("div");

            navHeader.AddCssClass("navbar-header");

            var navToggle = new TagBuilder("button");
            navToggle.AddCssClass("navbar-toggle");
            navToggle.MergeAttributes(new Dictionary<string, object>
            {
                { "type", "button" }, 
                { "data-toggle", "collapse" },
                { "data-target", ".navbar-ex1-collapse" },
            });

            var span = new TagBuilder("span");
            span.AddCssClass("sr-only");

            navToggle.InnerHtml += span;

            span = new TagBuilder("span");
            span.AddCssClass("icon-bar");

            navToggle.InnerHtml += span;

            navToggle.InnerHtml += span;

            navToggle.InnerHtml += span;

            navHeader.InnerHtml += navToggle;

            nav.InnerHtml += navHeader.ToString();

            var navbar = new TagBuilder("div");
            navbar.AddCssClass("collapse navbar-collapse navbar-ex1-collapse");

            var list = new TagBuilder("ul");

            list.AddCssClass("nav navbar-nav bs-navbar");

            foreach (var tab in this.Builder.TabsProperties)
            {
                if (!string.IsNullOrEmpty(tab.Name))
                {
                    var item = new TagBuilder("li");

                    if (tab.Selected)
                    {
                        item.AddCssClass("active");
                    }

                    var anchor = new TagBuilder("a");

                    anchor.MergeAttribute("href", "#");
                    anchor.MergeAttribute("data-toggle", "tab");

                    if (!string.IsNullOrEmpty(tab.Id.ToString()))
                    {
                        anchor.MergeAttribute("data-tabid", ((int)tab.Id).ToString());
                    }
                    else
                    {
                        throw new Exception("Tab property Id is not set for tab " + tab.Name);
                    }

                    anchor.InnerHtml += tab.Name;

                    item.InnerHtml += anchor;

                    list.InnerHtml += item;
                }
                else
                {
                    throw new Exception("Tab property Name is not set in BsGroupEditorAttribute");
                }
            }

            navbar.InnerHtml += list;

            nav.InnerHtml += navbar.ToString();

            return nav.ToString();
        }
    }
}
