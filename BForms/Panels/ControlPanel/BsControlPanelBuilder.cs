using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BForms.Models;
using BForms.Mvc;
using BForms.Renderers;

namespace BForms.Html
{
    public class BsControlPanelBuilder : BsBaseComponent<BsControlPanelBuilder>
    {
        #region Properties and constructors

        protected BsControlPanelActionsFactory ActionsFactory;
        protected BsControlPanelTabsFactory TabsFactory;
        protected string HeaderTitle;
        protected Glyphicon HeaderGlyphicon;
        protected string PanelId;
        protected BsTheme PanelTheme;
        protected string PanelQuicksearchPlaceholder;
        protected string PanelContent;
        protected bool HasGlobalQuicksearch;

        public BsControlPanelBuilder(ViewContext viewContext)
            : base(viewContext)
        {
            ActionsFactory = new BsControlPanelActionsFactory();
            TabsFactory = new BsControlPanelTabsFactory();

            PanelTheme = BsTheme.Default;

            this.renderer = new BsControlPanelBaseRenderer(this);
        }

        #endregion

        #region Fluent API

        public BsControlPanelBuilder Title(string title)
        {
            HeaderTitle = title;

            return this;
        }

        public BsControlPanelBuilder Glyphicon(Glyphicon glyphicon)
        {
            HeaderGlyphicon = glyphicon;

            return this;
        }

        public BsControlPanelBuilder Theme(BsTheme theme)
        {
            PanelTheme = theme;

            return this;
        }

        public BsControlPanelBuilder Content(string content)
        {
            PanelContent = content;

            return this;
        }

        public BsControlPanelBuilder Id(string id)
        {
            PanelId = id;

            if (htmlAttributes == null)
            {
                HtmlAttributes(new { id = id });
            }
            else
            {
                if (htmlAttributes.ContainsKey("id"))
                {
                    htmlAttributes["id"] = id;
                }
                else
                {
                    htmlAttributes.Add("id", id);
                }
            }

            return this;
        }

        public BsControlPanelBuilder ConfigureActions(Action<BsControlPanelActionsFactory> action)
        {
            action.Invoke(ActionsFactory);

            return this;
        }

        public BsControlPanelBuilder ConfigureTabs(Action<BsControlPanelTabsFactory> action)
        {
            action.Invoke(TabsFactory);

            return this;
        }

        public BsControlPanelBuilder Renderer(BsControlPanelBaseRenderer renderer)
        {
            this.renderer = renderer;

            return this;
        }

        public BsControlPanelBuilder QuicksearchPlaceholder(string placeholder)
        {
            PanelQuicksearchPlaceholder = placeholder;

            return this;
        }

        public BsControlPanelBuilder HasQuicksearch(bool hasQuicksearch = true)
        {
            HasGlobalQuicksearch = hasQuicksearch;

            return this;
        }

        #endregion

        #region Public methods

        public ControlPanelRenderingOptions GetRenderingOptions()
        {
            var renderingOptions = new ControlPanelRenderingOptions
            {
                Tabs = TabsFactory.GetTabs(),
                HeaderTitle = HeaderTitle,
                PanelTheme = PanelTheme,
                Actions = ActionsFactory.GetActions(),
                HeaderGlyphicon = HeaderGlyphicon,
                PanelId = PanelId,
                QuicksearchPlaceholder = PanelQuicksearchPlaceholder,
                Content = PanelContent,
                HasGlobalQuickSearch = HasGlobalQuicksearch
            };

            if (!renderingOptions.Tabs.Any(x => x.IsActive))
            {
                var firstTab = renderingOptions.Tabs.FirstOrDefault();

                if (firstTab != null)
                {
                    firstTab.Selected();
                }
            }

            return renderingOptions;
        }

        #endregion
    }

    #region Helpers

    public class ControlPanelRenderingOptions
    {
        public string HeaderTitle { get; set; }
        public Glyphicon HeaderGlyphicon { get; set; }
        public string PanelId { get; set; }
        public BsTheme PanelTheme { get; set; }
        public string QuicksearchPlaceholder { get; set; }
        public IEnumerable<ControlPanelTab> Tabs { get; set; }
        public IEnumerable<ControlPanelAction> Actions { get; set; }
        public string Content { get; set; }
        public bool HasGlobalQuickSearch { get; set; }
    }

    #endregion

}
