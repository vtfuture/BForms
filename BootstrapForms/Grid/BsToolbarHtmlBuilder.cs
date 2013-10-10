﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BootstrapForms.Utilities;

namespace BootstrapForms.Grid
{
    public class BsToolbarHtmlBuilder<TToolbar> : BaseComponent
    {
        private string displayName;
        private Dictionary<string, object> htmlAttributes;

        private BsToolbarActionsFactory<TToolbar> ActionsFactory { get; set; }

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

        public BsToolbarHtmlBuilder<TToolbar> HtmlAttributes(Dictionary<string, object> htmlAttributes)
        {
            this.htmlAttributes = htmlAttributes;
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
            toolbarBuilder.MergeClassAttribute("grids_header_bar", this.htmlAttributes);
            toolbarBuilder.MergeAttributes(this.htmlAttributes, true);

            var headerBulder = new TagBuilder("h1");
            headerBulder.InnerHtml += this.displayName;
            toolbarBuilder.InnerHtml += headerBulder.ToString();

            var controlsBuilder = new TagBuilder("div");
            controlsBuilder.MergeAttribute("class", "grids_controls");

            string tabs = string.Empty;

            foreach (var action in this.ActionsFactory.Actions)
            {
                controlsBuilder.InnerHtml += action.Render();

                if (action.TabDelegate != null)
                {
                    tabs += action.TabDelegate(this.model);
                }
            }
            toolbarBuilder.InnerHtml += controlsBuilder.ToString();
            toolbarBuilder.InnerHtml += tabs;

            return toolbarBuilder.ToString();
        }
    }
}