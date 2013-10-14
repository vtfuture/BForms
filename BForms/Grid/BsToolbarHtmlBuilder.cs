using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BForms.Mvc;
using BForms.Utilities;

namespace BForms.Grid
{
    public class BsToolbarHtmlBuilder<TToolbar> : BaseComponent
    {
        private string displayName;
        private IDictionary<string, object> htmlAttributes;

        private BsToolbarActionsFactory<TToolbar> ActionsFactory { get; set; }

        private BsTheme theme = BsTheme.Default;
        private readonly string fullName;
        private readonly TToolbar model;
        private readonly ModelMetadata metadata;

        public BsToolbarHtmlBuilder() { }

        public BsToolbarHtmlBuilder(string fullName, TToolbar model, ModelMetadata metadata, ViewContext viewContext): base(viewContext)
        {
            this.fullName = fullName;
            this.model = model;
            this.metadata = metadata;

            this.displayName = this.metadata.DisplayName;
        }

        public BsToolbarHtmlBuilder<TToolbar> DisplayName(string name)
        {
            this.displayName = name;
            return this;
        }

        /// <summary>
        /// Appends html attributes to grid_toolbar div element
        /// </summary>
        public BsToolbarHtmlBuilder<TToolbar> HtmlAttributes(Dictionary<string, object> htmlAttributes)
        {
            this.htmlAttributes = htmlAttributes;
            return this;
        }

        /// <summary>
        /// Appends html attributes to grid_toolbar div element
        /// </summary>
        public BsToolbarHtmlBuilder<TToolbar> HtmlAttributes(object htmlAttributes)
        {
            this.htmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            return this;
        }

        public BsToolbarHtmlBuilder<TToolbar> Theme(BsTheme theme)
        {
            this.theme = theme;

            return this;
        }

        public BsToolbarHtmlBuilder<TToolbar> ConfigureActions(Action<BsToolbarActionsFactory<TToolbar>> configurator)
        {
            this.ActionsFactory = new BsToolbarActionsFactory<TToolbar>(this.viewContext);
            configurator(this.ActionsFactory);

            return this;
        }

        public override string Render()
        {
            var toolbarBuilder = new TagBuilder("div");
            toolbarBuilder.MergeAttribute("id", this.fullName.Split('.').Last().ToLower());
            toolbarBuilder.MergeClassAttribute("grid_toolbar", this.htmlAttributes);
            toolbarBuilder.MergeAttributes(this.htmlAttributes, true);

            toolbarBuilder.AddCssClass(this.theme.GetDescription());

            var toolbarHeaderBuilder = new TagBuilder("div");
            toolbarHeaderBuilder.AddCssClass("grid_toolbar_header");
            
            var headerBulder = new TagBuilder("h1");
            headerBulder.InnerHtml += this.displayName;
            toolbarHeaderBuilder.InnerHtml += headerBulder.ToString();

            var controlsBuilder = new TagBuilder("div");
            controlsBuilder.AddCssClass("grid_toolbar_controls");

            string tabs = string.Empty;

            foreach (var action in this.ActionsFactory.Actions)
            {
                controlsBuilder.InnerHtml += action.Render();

                var normalAction = action as BsToolbarAction<TToolbar>;

                if (normalAction != null && normalAction.TabDelegate != null)
                {
                    tabs += normalAction.TabDelegate(this.model);
                }
            }
            toolbarHeaderBuilder.InnerHtml += controlsBuilder.ToString();

            toolbarBuilder.InnerHtml += toolbarHeaderBuilder.ToString();
            toolbarBuilder.InnerHtml += tabs;

            return toolbarBuilder.ToString();
        }
    }
}