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
    /// <summary>
    /// For toolbar with forms
    /// </summary>
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

            toolbarBuilder.AddCssClass(this.Builder.Theme.GetDescription());

            var toolbarHeaderBuilder = new TagBuilder("div");
            toolbarHeaderBuilder.AddCssClass("grid_toolbar_header navbar");

            var headerContainer = new TagBuilder("div");
            headerContainer.AddCssClass("navbar-header");

            var toggleButton = new TagBuilder("button");
            toggleButton.AddCssClass("navbar-toggle");
            toggleButton.MergeAttribute("type", "button");
            toggleButton.MergeAttribute("data-toggle", "collapse");
            toggleButton.MergeAttribute("data-target", ".grid_toolbar_responsive_navbar");

            var toggleSpan = new TagBuilder("span");
            toggleSpan.AddCssClass("icon-bar");

            toggleButton.InnerHtml += toggleSpan.ToString() + toggleSpan + toggleSpan;

            headerContainer.InnerHtml += toggleButton;

            var navbarBrand = new TagBuilder("span");
            navbarBrand.AddCssClass("navbar-brand");
            navbarBrand.InnerHtml += this.Builder.displayName;

            headerContainer.InnerHtml += navbarBrand;

            toolbarHeaderBuilder.InnerHtml += headerContainer;

            string tabs = string.Empty;

            if (this.Builder.ActionsFactory != null)
            {
                int tabNr = 0;

                var controlsContainer = new TagBuilder("div");
                controlsContainer.AddCssClass("collapse navbar-collapse grid_toolbar_responsive_navbar");

                var controlsBuilder = new TagBuilder("ul");
                controlsBuilder.AddCssClass("grid_toolbar_controls nav navbar-nav pull-right");


                // Render tab within ButtonGroup
                if (this.Builder.ActionsFactory.ButtonGroups != null)
                {
                    foreach (var buttonGroup in this.Builder.ActionsFactory.ButtonGroups)
                    {
                        if (buttonGroup.Actions.Any())
                        {
                            foreach (var action in buttonGroup.Actions)
                            {
                                var defaultAction = action as BsToolbarAction<TToolbar>;

                                // renders tab content if any
                                if (defaultAction != null && defaultAction.TabDelegate != null)
                                {
                                    tabs += RenderTab(defaultAction, tabNr).ToString();
                                    tabNr++;
                                }
                            }
                        }
                        controlsBuilder.InnerHtml += buttonGroup.ToString();
                    }
                }

                // Render actions inside the toolbar
                foreach (var action in this.Builder.ActionsFactory.Actions)
                {
                    // check if action is default
                    var defaultAction = action as BsToolbarAction<TToolbar>;

                    // renders tab content if any
                    if (defaultAction != null && defaultAction.TabDelegate != null)
                    {
                        tabs += RenderTab(defaultAction, tabNr).ToString();
                        tabNr++;
                    }

                    if (action is BsToolbarButtonGroupAction<TToolbar>)
                    {
                        var buttonGroup = (action as BsToolbarButtonGroupAction<TToolbar>).ButtonGroup;

                        foreach (var controlAction in buttonGroup.Actions)
                        {
                            var inlineAction = controlAction as BsToolbarAction<TToolbar>;
                            if (inlineAction == null || inlineAction.TabDelegate == null) continue;
                            tabs += RenderTab(inlineAction, tabNr).ToString();
                            tabNr++;
                        }

                        controlsBuilder.InnerHtml += buttonGroup.ToString();
                    }
                    else
                    {
                        controlsBuilder.InnerHtml += action.ToString();
                    }
                }

                controlsContainer.InnerHtml += controlsBuilder;

                toolbarHeaderBuilder.InnerHtml += controlsContainer.ToString();
            }

            toolbarBuilder.InnerHtml += toolbarHeaderBuilder.ToString();


            toolbarBuilder.InnerHtml += tabs;

            return toolbarBuilder.ToString();
        }

        public TagBuilder RenderTab(BsToolbarAction<TToolbar> defaultAction, int tabNr)
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

            //sets tab container id for tab - button correlation
            defaultAction.SetTabId(tabId);
            return tabBuilder;
        }
    }

    /// <summary>
    /// For toolbar without forms
    /// </summary>
    public class BsToolbarBaseRenderer : BsBaseRenderer<BsToolbarHtmlBuilder>
    {
        public BsToolbarBaseRenderer()
        {

        }

        public BsToolbarBaseRenderer(BsToolbarHtmlBuilder builder)
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

            toolbarBuilder.AddCssClass(this.Builder.Theme.GetDescription());

            var toolbarHeaderBuilder = new TagBuilder("div");
            toolbarHeaderBuilder.AddCssClass("grid_toolbar_header");

            var headerBulder = new TagBuilder("h1");
            headerBulder.InnerHtml += this.Builder.displayName;
            toolbarHeaderBuilder.InnerHtml += headerBulder.ToString();

            string tabs = string.Empty;

            if (this.Builder.ActionsFactory != null)
            {
                int tabNr = 0;
                var controlsBuilder = new TagBuilder("div");
                controlsBuilder.AddCssClass("grid_toolbar_controls");

                // Render actions inside the toolbar
                foreach (var action in this.Builder.ActionsFactory.Actions)
                {
                    // check if action is default
                    var defaultAction = action as BsToolbarAction;

                    controlsBuilder.InnerHtml += action.ToString();
                }

                // Render tab within ButtonGroup
                if (this.Builder.ActionsFactory.ButtonGroups != null)
                {
                    foreach (var buttonGroup in this.Builder.ActionsFactory.ButtonGroups)
                    {
                        if (buttonGroup.Actions.Any())
                        {
                            foreach (var action in buttonGroup.Actions)
                            {

                                var defaultAction = action as BsToolbarAction;
                                // renders tab content if any
                                if (defaultAction != null)
                                {

                                    tabs += RenderTab(defaultAction, tabNr).ToString();
                                    tabNr++;
                                }
                            }
                        }
                        controlsBuilder.InnerHtml += buttonGroup.ToString();
                    }
                }

                toolbarHeaderBuilder.InnerHtml += controlsBuilder.ToString();
            }

            toolbarBuilder.InnerHtml += toolbarHeaderBuilder.ToString();
            toolbarBuilder.InnerHtml += tabs;

            return toolbarBuilder.ToString();
        }

        public TagBuilder RenderTab(BsToolbarAction defaultAction, int tabNr)
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

            //sets tab container id for tab - button correlation
            defaultAction.SetTabId(tabId);
            return tabBuilder;
        }

    }

}
