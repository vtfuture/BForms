using BForms.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BForms.Utilities;

namespace BForms.Renderers
{
    public class BsToolbarBaseRenderer<TToolbar> : BsBaseRenderer<BsToolbarHtmlBuilder<TToolbar>>
    {
        public BsToolbarBaseRenderer()
        {

        }

        public BsToolbarBaseRenderer(BsToolbarHtmlBuilder<TToolbar> builder)
            : base(builder)
        { 

        }

        /// <summary>
        /// Renders toolbar
        /// </summary>
        public override string Render()
        {
            var toolbarBuilder = new TagBuilder("div");
            toolbarBuilder.MergeAttribute("id", this.Builder.id);
            toolbarBuilder.MergeClassAttribute("grid_toolbar", this.Builder.htmlAttributes);
            toolbarBuilder.MergeAttributes(this.Builder.htmlAttributes, true);

            toolbarBuilder.AddCssClass(this.Builder.theme.GetDescription());

            var toolbarHeaderBuilder = new TagBuilder("div");
            toolbarHeaderBuilder.AddCssClass("grid_toolbar_header");

            var headerBulder = new TagBuilder("h1");
            headerBulder.InnerHtml += this.Builder.displayName;
            toolbarHeaderBuilder.InnerHtml += headerBulder.ToString();

            string tabs = string.Empty;

            if (this.Builder.ActionsFactory != null)
            {
                var controlsBuilder = new TagBuilder("div");
                controlsBuilder.AddCssClass("grid_toolbar_controls");

                int tabNr = 0;

                foreach (var action in this.Builder.ActionsFactory.Actions)
                {
                    // check if action is default
                    var defaultAction = action as BsToolbarAction<TToolbar>;

                    // renders tab content if any
                    if (defaultAction != null && defaultAction.TabDelegate != null)
                    {
                        var tabId = this.Builder.id + "_tab_" + tabNr;

                        var tabBuilder = new TagBuilder("div");
                        if (defaultAction.HtmlAttr != null)
                        {
                            if (defaultAction.HtmlAttr.ContainsKey("class"))
                            {
                                tabBuilder.AddCssClass(defaultAction.HtmlAttr["class"] as string);
                            }
                            tabBuilder.MergeAttributes(defaultAction.HtmlAttr);
                        }
                        tabBuilder.AddCssClass("grid_toolbar_form");
                        tabBuilder.MergeAttribute("style", "display:none;");
                        tabBuilder.MergeAttribute("id", tabId);
                        tabBuilder.InnerHtml += defaultAction.TabDelegate(this.Builder.model);

                        tabs += tabBuilder.ToString();

                        //sets tab container id for tab - button correlation
                        defaultAction.SetTabId(tabId);

                        tabNr++;
                    }

                    controlsBuilder.InnerHtml += action.ToString();
                }
                toolbarHeaderBuilder.InnerHtml += controlsBuilder.ToString();
            }

            toolbarBuilder.InnerHtml += toolbarHeaderBuilder.ToString();
            toolbarBuilder.InnerHtml += tabs;

            return toolbarBuilder.ToString();
        }
    }
}
