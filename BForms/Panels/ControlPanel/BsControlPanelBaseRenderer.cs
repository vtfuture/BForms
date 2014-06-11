using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BForms.Html;
using BForms.Models;
using BForms.Utilities;

namespace BForms.Renderers
{
    public class BsControlPanelBaseRenderer : BsBaseRenderer<BsControlPanelBuilder>
    {
        public BsControlPanelBaseRenderer() { }

        public BsControlPanelBaseRenderer(BsControlPanelBuilder builder) : base(builder) { }

        public virtual TagBuilder GetHeaderBuilder(ControlPanelRenderingOptions options)
        {
            var headerBuilder = new TagBuilder("div");

            headerBuilder.AddCssClass("panel-heading " + GetColorClass(options.PanelTheme));

            var titleWrapperBuilder = new TagBuilder("h3");

            titleWrapperBuilder.AddCssClass("panel-title control-panel-title fg-white");

            var titleBuilder = new TagBuilder("a");
            var actionsBuilder = GetActionsBuilder(options);

            titleBuilder.AddCssClass("no-hover");
            titleBuilder.MergeAttribute("href", "#" + options.PanelId);

            var glyphiconBuilder = new TagBuilder("span");
            var titleTextbuilder = new TagBuilder("span");

            glyphiconBuilder.AddCssClass("glyphicon " + options.HeaderGlyphicon.GetDescription());
            titleTextbuilder.AddCssClass("panel-title-text");
            titleTextbuilder.InnerHtml = options.HeaderTitle;

            titleBuilder.InnerHtml = glyphiconBuilder.ToString() + titleTextbuilder.ToString();

            titleWrapperBuilder.InnerHtml = titleBuilder.ToString() + actionsBuilder.ToString();

            headerBuilder.InnerHtml = titleWrapperBuilder.ToString();

            return headerBuilder;
        }

        private string GetColorClass(BsTheme theme)
        {
            switch (theme)
            {
                case BsTheme.Black:
                    {
                        return "bg-black";
                    }
                case BsTheme.Blue:
                    {
                        return "bg-blue";
                    }
                case BsTheme.Default:
                    {
                        return "bg-turquoise";
                    }
                case BsTheme.Green:
                    {
                        return "bg-green";
                    }
                case BsTheme.Orange:
                    {
                        return "bg-orange";
                    }
                case BsTheme.Purple:
                    {
                        return "bg-purple";
                    }
            }

            return String.Empty;
        }

        private TagBuilder GetActionsBuilder(ControlPanelRenderingOptions options)
        {
            var actionsContainerBuilder = new TagBuilder("span");

            actionsContainerBuilder.AddCssClass("pull-right");

            foreach (var action in options.Actions)
            {
                var actionBuilder = new TagBuilder("a");
                var actionGlyphiconBuilder = new TagBuilder("span");

                actionBuilder.MergeAttribute("href", "#");
                actionBuilder.AddCssClass("control-panel-action");
                actionBuilder.MergeAttribute("data-action", action.ActionName);

                actionGlyphiconBuilder.AddCssClass("glyphicon " + action.Glyphicon.GetDescription());

                actionBuilder.InnerHtml = actionGlyphiconBuilder.ToString();

                actionsContainerBuilder.InnerHtml += actionBuilder.ToString();
            }

            return actionsContainerBuilder;
        }

        public virtual TagBuilder GetBodyBuilder(ControlPanelRenderingOptions options)
        {
            var bodyBuilder = new TagBuilder("div");

            bodyBuilder.AddCssClass("panel-body border border-no-top");

            if (!String.IsNullOrEmpty(options.Content))
            {
                var contentContainerBuilder = new TagBuilder("div");
                var controlsContainerBuilder = new TagBuilder("div");

                contentContainerBuilder.AddCssClass("panel-body-content row");        

                controlsContainerBuilder.AddCssClass("row controls-container");

                var quickSearchbuilder = GetQuicksearchContainerBuilder(options);

                quickSearchbuilder.AddCssClass("pull-right");

                var controlsWrapperBuilder = new TagBuilder("div");
                var contentWrapperbuilder = new TagBuilder("div");

                controlsWrapperBuilder.AddCssClass("col-lg-12 col-md-12 col-sm-12");
                contentWrapperbuilder.AddCssClass("col-lg-12 col-md-12 col-sm-12");

                controlsWrapperBuilder.InnerHtml = quickSearchbuilder.ToString();
                contentWrapperbuilder.InnerHtml = options.Content;

                controlsContainerBuilder.InnerHtml = controlsWrapperBuilder.ToString();
                contentContainerBuilder.InnerHtml = contentWrapperbuilder.ToString();

                var separatorBuilder = new TagBuilder("hr");

                if (options.HasGlobalQuickSearch)
                {
                    bodyBuilder.InnerHtml = controlsContainerBuilder.ToString() + separatorBuilder.ToString() +
                                            contentContainerBuilder.ToString();
                }
                else
                {
                    bodyBuilder.InnerHtml = contentContainerBuilder.ToString();
                }

            }
            else
            {
                var navigationTabsBuilder = GetNavigationTabsBuilder(options);
                var tabsContentBuilder = GetTabsContentBuilder(options);

                bodyBuilder.InnerHtml = navigationTabsBuilder.ToString() + tabsContentBuilder.ToString();
            }

            return bodyBuilder;
        }

        private TagBuilder GetNavigationTabsBuilder(ControlPanelRenderingOptions options)
        {
            var tabsContainerBuilder = new TagBuilder("ul");

            tabsContainerBuilder.AddCssClass("nav nav-tabs control-panel-nav-tabs hidden-xs");

            if (options.Tabs != null && options.Tabs.Any())
            {
                foreach (var tab in options.Tabs)
                {
                    var tabBuilder = new TagBuilder("li");

                    var tabAnchorbuilder = new TagBuilder("a");
                    
                    tabAnchorbuilder.AddCssClass("control-panel-nav-tab fg-black");
                    tabAnchorbuilder.MergeAttribute("href", "#");
                    tabAnchorbuilder.MergeAttribute("data-tabid", tab.TabId);
                    tabAnchorbuilder.MergeAttribute("data-showquicksearch", tab.HasQuicksearch.ToString());
                    tabAnchorbuilder.InnerHtml = tab.DisplayName;

                    tabBuilder.InnerHtml = tabAnchorbuilder.ToString();

                    if (tab.IsActive)
                    {
                        tabBuilder.AddCssClass("active");
                    }

                    tabsContainerBuilder.InnerHtml += tabBuilder.ToString();
                }
            }

            var controlsContainerBuilder = new TagBuilder("li");

            controlsContainerBuilder.AddCssClass("control-panel-nav-controls-container pull-right");

            var formContainerBuilder = GetQuicksearchContainerBuilder(options);

            controlsContainerBuilder.InnerHtml = formContainerBuilder.ToString();

            tabsContainerBuilder.InnerHtml += controlsContainerBuilder.ToString();

            return tabsContainerBuilder;
        }

        private TagBuilder GetQuicksearchContainerBuilder(ControlPanelRenderingOptions options)
        {
            var formContainerBuilder = new TagBuilder("div");
            var inputGroupBuilder = new TagBuilder("div");
            var inputbuilder = new TagBuilder("input");

            formContainerBuilder.AddCssClass("form_container " + options.PanelTheme.GetDescription());
            inputGroupBuilder.AddCssClass("input-group");
            
            inputbuilder.AddCssClass("form-control bs-text oval tab-search");
            inputbuilder.MergeAttribute("placeholder", "  " + options.QuicksearchPlaceholder);
            inputbuilder.MergeAttribute("type", "search");

            var searchIsHidden = options.Tabs != null && options.Tabs.Any(x => x.IsActive && !x.HasQuicksearch);

            if (searchIsHidden)
            {
                inputbuilder.MergeAttribute("style", "display:none;");
            }

            inputGroupBuilder.InnerHtml = inputbuilder.ToString();
            formContainerBuilder.InnerHtml = inputGroupBuilder.ToString();

            return formContainerBuilder;
        }

        private TagBuilder GetTabsContentBuilder(ControlPanelRenderingOptions options)
        {
            var tabsContainerBuilder = new TagBuilder("ul");

            foreach (var tab in options.Tabs)
            {
                var tabBuilder = new TagBuilder("li");

                tabBuilder.AddCssClass("control-panel-tab");
                tabBuilder.MergeAttribute("data-tabid", tab.TabId);

                if (!tab.IsActive)
                {
                    tabBuilder.MergeAttribute("style", "display:none;");
                }

                tabBuilder.InnerHtml = tab.Content ?? String.Empty;
                
                tabsContainerBuilder.InnerHtml += tabBuilder.ToString();
            }

            return tabsContainerBuilder;
        }

        public override string Render()
        {
            var panelBuilder = new TagBuilder("div");

            panelBuilder.AddCssClass("panel control-panel");
            panelBuilder.MergeAttributes(this.Builder.htmlAttributes);

            var renderingOptions = this.Builder.GetRenderingOptions();

            var headerBuilder = GetHeaderBuilder(renderingOptions);
            var bodyBuilder = GetBodyBuilder(renderingOptions);

            panelBuilder.InnerHtml = headerBuilder.ToString() + bodyBuilder.ToString();

            return panelBuilder.ToString();
        }
    }
}
