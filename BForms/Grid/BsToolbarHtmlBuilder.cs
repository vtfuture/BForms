using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BForms.Models;
using BForms.Mvc;
using BForms.Utilities;
using BForms.Renderers;

namespace BForms.Grid
{
    public class BsToolbarHtmlBuilder<TToolbar> : BsBaseComponent<BsToolbarHtmlBuilder<TToolbar>>
    {
        /// <summary>
        /// Text that will be displayed in header
        /// </summary>
        internal string displayName;

        /// <summary>
        /// Utility class used for action 
        /// </summary>
        internal BsToolbarActionsFactory<TToolbar> ActionsFactory { get; set; }

        /// <summary>
        /// Theme
        /// </summary>
        internal BsTheme theme = BsTheme.Default;

        /// <summary>
        /// Toolbar name based on class hierarchy
        /// </summary>
        private readonly string fullName;

        internal string id;

        /// <summary>
        /// Toolbar model
        /// </summary>
        internal readonly TToolbar model;

        /// <summary>
        /// Toolbar model metadata
        /// </summary>
        private readonly ModelMetadata metadata;

        /// <summary>
        /// model attributes
        /// </summary>
        private readonly object[] attributes;

        public BsToolbarHtmlBuilder()
        {
            this.renderer = new BsToolbarBaseRenderer<TToolbar>(this);
        }

        public BsToolbarHtmlBuilder(string fullName, TToolbar model, ModelMetadata metadata, object[] attributes, ViewContext viewContext)
            : base(viewContext)
        {
            this.renderer = new BsToolbarBaseRenderer<TToolbar>(this);
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
        public override BsToolbarHtmlBuilder<TToolbar> HtmlAttributes(IDictionary<string, object> htmlAttributes)
        {
            base.HtmlAttributes(htmlAttributes);

            SetIdFromHtmlAttributes(htmlAttributes);

            return this;
        }

        /// <summary>
        /// Appends html attributes to grid_toolbar div element
        /// </summary>
        public override BsToolbarHtmlBuilder<TToolbar> HtmlAttributes(object htmlAttributes)
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


    }

    /// <summary>
    /// For toolbar without forms
    /// </summary>
    public class BsToolbarHtmlBuilder : BsBaseComponent<BsToolbarHtmlBuilder>
    {
        /// <summary>
        /// Text that will be displayed in header
        /// </summary>
        internal string displayName;

        /// <summary>
        /// Utility class used for action 
        /// </summary>
        internal BsToolbarActionsFactory ActionsFactory { get; set; }

        /// <summary>
        /// Theme
        /// </summary>
        internal BsTheme theme = BsTheme.Default;

        /// <summary>
        /// Toolbar name based on class hierarchy
        /// </summary>
        private readonly string fullName;

        internal string id;


        /// <summary>
        /// model attributes
        /// </summary>
        private readonly object[] attributes;

        public BsToolbarHtmlBuilder(ViewContext context) :
            base(context)
        {
            this.renderer = new BsToolbarBaseRenderer(this);
        }

        /// <summary>
        /// Sets the display name property
        /// </summary>
        /// <returns>BsToolbarHtmlBuilder</returns>
        public BsToolbarHtmlBuilder DisplayName(string name)
        {
            this.displayName = name;
            return this;
        }

        /// <summary>
        /// Appends html attributes to grid_toolbar div element
        /// </summary>
        public override BsToolbarHtmlBuilder HtmlAttributes(IDictionary<string, object> htmlAttributes)
        {
            base.HtmlAttributes(htmlAttributes);

            SetIdFromHtmlAttributes(htmlAttributes);

            return this;
        }

        /// <summary>
        /// Appends html attributes to grid_toolbar div element
        /// </summary>
        public override BsToolbarHtmlBuilder HtmlAttributes(object htmlAttributes)
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
        public BsToolbarHtmlBuilder Theme(BsTheme theme)
        {
            this.theme = theme;

            return this;
        }

        /// <summary>
        /// Sets actions factory
        /// </summary>
        /// <returns>BsToolbarHtmlBuilder</returns>
        public BsToolbarHtmlBuilder ConfigureActions(Action<BsToolbarActionsFactory> configurator)
        {
            this.ActionsFactory = new BsToolbarActionsFactory(this.viewContext);
            configurator(this.ActionsFactory);

            return this;
        }


    }
}