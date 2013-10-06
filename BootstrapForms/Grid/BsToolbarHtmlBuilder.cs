using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BootstrapForms.Grid
{
    public class BsToolbarHtmlBuilder<TToolbar> : IHtmlBuilder
    {
        private string displayName;
        private string classes { get; set; }
        private BsToolbarActionsFactory ActionsFactory { get; set; }

        private readonly string fullName;
        private readonly TToolbar model;
        private readonly ModelMetadata metadata;

        public BsToolbarHtmlBuilder() { }

        public BsToolbarHtmlBuilder(string fullName, TToolbar model, ModelMetadata metadata)
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

        public BsToolbarHtmlBuilder<TToolbar> Classes(string classes)
        {
            this.classes = classes;
            return this;
        }

        public BsToolbarHtmlBuilder<TToolbar> ConfigureActions(Action<BsToolbarActionsFactory> configurator)
        {
            this.ActionsFactory = new BsToolbarActionsFactory();
            configurator(this.ActionsFactory);

            return this;
        }

        public MvcHtmlString Render()
        {
            var toolbarBuilder = new TagBuilder("div");
            toolbarBuilder.MergeAttribute("id", this.fullName.Split('.').Last().ToLower());
            toolbarBuilder.MergeAttribute("class", "grids_header_bar");

            var headerBulder = new TagBuilder("h1");
            headerBulder.InnerHtml += this.displayName;
            toolbarBuilder.InnerHtml += headerBulder.ToString();

            var controlsBuilder = new TagBuilder("div");
            controlsBuilder.MergeAttribute("class", "grids_controls");

            string tabs = string.Empty;

            foreach (var action in this.ActionsFactory.Actions)
            {
                controlsBuilder.InnerHtml += action.Render();

                tabs += action.TabHtml;
            }
            toolbarBuilder.InnerHtml += controlsBuilder.ToString();
            toolbarBuilder.InnerHtml += tabs;

            return new MvcHtmlString(toolbarBuilder.ToString());
        }
    }
}