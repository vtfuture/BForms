using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BForms.Models;
using BForms.Mvc;
using BForms.Utilities;

namespace BForms.Grid
{
    public class BsToolbarHtmlBuilder<TToolbar> : BaseComponent
    {
        /// <summary>
        /// Text that will be displayed in header
        /// </summary>
        private string displayName;

        /// <summary>
        /// Html attributes that will decorate toolbar container
        /// </summary>
        private IDictionary<string, object> htmlAttributes;

        /// <summary>
        /// Utility class used for action 
        /// </summary>
        private BsToolbarActionsFactory<TToolbar> ActionsFactory { get; set; }

        /// <summary>
        /// Theme
        /// </summary>
        private BsTheme theme = BsTheme.Default;

        /// <summary>
        /// Toolbar name based on class hierarchy
        /// </summary>
        private readonly string fullName;

        private string id;

        /// <summary>
        /// Toolbar model
        /// </summary>
        private readonly TToolbar model;

        /// <summary>
        /// Toolbar model metadata
        /// </summary>
        private readonly ModelMetadata metadata;

        /// <summary>
        /// model attributes
        /// </summary>
        private readonly object[] attributes;

        public BsToolbarHtmlBuilder() { }

        public BsToolbarHtmlBuilder(string fullName, TToolbar model, ModelMetadata metadata, object[] attributes, ViewContext viewContext)
            : base(viewContext)
        {
            this.fullName = fullName;
            this.model = model;
            this.metadata = metadata;
            this.id = this.fullName.Split('.').Last().ToLower();

            this.displayName = this.metadata.DisplayName;
            this.attributes = attributes;
            //TODO: refactor
            foreach (var item in this.attributes)
            {
                var attr = item as BsToolbarAttribute;
                if (attr != null && attr.Theme > 0)
                {
                    this.theme = attr.Theme;
                }
            }
        }

        /// <summary>
        /// Sets the display name property
        /// </summary>
        /// <returns>BsToolbarHtmlBuilder</returns>
        public BsToolbarHtmlBuilder<TToolbar> DisplayName(string name)
        {
            this.displayName = name;
            return this;
        }

        /// <summary>
        /// Appends html attributes to grid_toolbar div element
        /// </summary>
        public BsToolbarHtmlBuilder<TToolbar> HtmlAttributes(IDictionary<string, object> htmlAttributes)
        {
            this.htmlAttributes = htmlAttributes;
            SetIdFromHtmlAttributes(htmlAttributes);
            return this;
        }

        /// <summary>
        /// Appends html attributes to grid_toolbar div element
        /// </summary>
        public BsToolbarHtmlBuilder<TToolbar> HtmlAttributes(object htmlAttributes)
        {
            return HtmlAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        private void SetIdFromHtmlAttributes(IDictionary<string, object> htmlAttr)
        {
            if (htmlAttr.Keys.Contains("id"))
            {
                this.id = htmlAttr["id"].ToString(); 
            }
        }

        /// <summary>
        /// Sets toolbar Theme
        /// </summary>
        /// <param name="theme"></param>
        /// <returns>BsToolbarHtmlBuilder</returns>
        public BsToolbarHtmlBuilder<TToolbar> Theme(BsTheme theme)
        {
            this.theme = theme;

            return this;
        }

        /// <summary>
        /// Sets actions factory
        /// </summary>
        /// <returns>BsToolbarHtmlBuilder</returns>
        public BsToolbarHtmlBuilder<TToolbar> ConfigureActions(Action<BsToolbarActionsFactory<TToolbar>> configurator)
        {
            this.ActionsFactory = new BsToolbarActionsFactory<TToolbar>(this.viewContext);
            configurator(this.ActionsFactory);

            return this;
        }

        /// <summary>
        /// Renders toolbar
        /// </summary>
        public override string Render()
        {
            var toolbarBuilder = new TagBuilder("div");
            toolbarBuilder.MergeAttribute("id", this.id);
            toolbarBuilder.MergeClassAttribute("grid_toolbar", this.htmlAttributes);
            toolbarBuilder.MergeAttributes(this.htmlAttributes, true);

            toolbarBuilder.AddCssClass(this.theme.GetDescription());

            var toolbarHeaderBuilder = new TagBuilder("div");
            toolbarHeaderBuilder.AddCssClass("grid_toolbar_header");

            var headerBulder = new TagBuilder("h1");
            headerBulder.InnerHtml += this.displayName;
            toolbarHeaderBuilder.InnerHtml += headerBulder.ToString();
            
            string tabs = string.Empty;

            if (this.ActionsFactory != null)
            {
                var controlsBuilder = new TagBuilder("div");
                controlsBuilder.AddCssClass("grid_toolbar_controls");

                int tabNr = 0;

                foreach (var action in this.ActionsFactory.Actions)
                {
                    // check if action is default
                    var defaultAction = action as BsToolbarAction<TToolbar>;

                    // renders tab content if any
                    if (defaultAction != null && defaultAction.TabDelegate != null)
                    {
                        var tabId = this.id + "_tab_" + tabNr;

                        var tabBuilder = new TagBuilder("div");
                        tabBuilder.AddCssClass("grid_toolbar_form");
                        tabBuilder.MergeAttribute("style", "display:none;");
                        tabBuilder.MergeAttribute("id", tabId);
                        tabBuilder.InnerHtml += defaultAction.TabDelegate(this.model);

                        tabs += tabBuilder.ToString();

                        //sets tab container id for tab - button correlation
                        defaultAction.SetTabId(tabId);

                        tabNr++;
                    }

                    controlsBuilder.InnerHtml += action.Render();
                }
                toolbarHeaderBuilder.InnerHtml += controlsBuilder.ToString();
            }

            toolbarBuilder.InnerHtml += toolbarHeaderBuilder.ToString();
            toolbarBuilder.InnerHtml += tabs;

            return toolbarBuilder.ToString();
        }
    }
}