using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using BForms.Models;
using BForms.Mvc;

namespace BForms.Grid
{
    /// <summary>
    /// Grid toolbar default control component
    /// </summary>
    public class BsToolbarAction<TToolbar> : BaseComponent
    {
        #region Properties and constructors
        private string descriptorClass;

        private IDictionary<string, object> htmlAttributes;

        private string styleClasses;

        private string title;

        private string text;

        private Glyphicon? glyphIcon;

        private string href;

        private Func<TToolbar, MvcHtmlString> tabDelegate;
        public Func<TToolbar, MvcHtmlString> TabDelegate
        {
            get
            {
                return this.tabDelegate;
            }
        }

        internal IDictionary<string, object> HtmlAttr
        {
            get
            {
                return this.htmlAttributes;
            }
        }

        private string tabId;

        public BsToolbarAction(ViewContext viewContext)
            :base(viewContext) { }

        public BsToolbarAction(BsToolbarActionType type, ViewContext viewContext)
            : base(viewContext)
        {
            // Set default control action properties
            switch (type)
            {
                case BsToolbarActionType.Add:
                    {
                        this.descriptorClass = "btn btn-add bs-show_add";
                        this.glyphIcon = Glyphicon.Plus;
                        this.text = "Add";
                        break;
                    }
                case BsToolbarActionType.Refresh:
                    {
                        this.descriptorClass = "btn btn-refresh";
                        this.title = "Refresh";
                        this.glyphIcon = Glyphicon.Refresh;
                        break;
                    }
                case BsToolbarActionType.AdvancedSearch:
                    {
                        this.descriptorClass = "btn btn_advanced_search bs-show_advanced_search";
                        this.title = "Advanced Search";
                        this.glyphIcon = Glyphicon.Filter;
                        break;
                    }
            }
        }

        public string GetDescriptorClass()
        {
            return this.descriptorClass;
        }

        public BsToolbarAction(string descriptorClass, ViewContext viewContext)
            : base(viewContext)
        {
            this.descriptorClass = descriptorClass;
        }
        #endregion

        /// <summary>
        /// Sets control descriptor class
        /// </summary>
        /// <returns>BsToolbarAction</returns>
        public BsToolbarAction<TToolbar> DescriptorClass(string descriptorClass)
        {
            this.descriptorClass = descriptorClass;
            return this;
        }

        /// <summary>
        /// Sets control style
        /// </summary>
        /// <returns>BsToolbarAction</returns>
        public BsToolbarAction<TToolbar> StyleClasses(string styleClasses)
        {
            this.styleClasses = styleClasses;
            return this;
        }

        /// <summary>
        /// Sets control button title
        /// </summary>
        /// <returns>BsToolbarAction</returns>
        public BsToolbarAction<TToolbar> Title(string title)
        {
            this.title = title;
            return this;
        }

        /// <summary>
        /// Sets control button text
        /// </summary>
        /// <returns>BsToolbarAction</returns>
        public BsToolbarAction<TToolbar> Text(string text)
        {
            this.text = text;
            return this;
        }

        /// <summary>
        /// Sets control GlyphIcon
        /// </summary>
        /// <returns>BsToolbarAction</returns>
        public BsToolbarAction<TToolbar> GlyphIcon(Glyphicon icon)
        {
            this.glyphIcon = icon;
            return this;
        }

        /// <summary>
        /// Sets tab content
        /// </summary>
        /// <returns>BsToolbarAction</returns>
        public BsToolbarAction<TToolbar> Tab(Func<TToolbar, MvcHtmlString> tabDelegate)
        {
            this.tabDelegate = tabDelegate;
            return this;
        }

        /// <summary>
        /// Set html attributes
        /// </summary>
        public BsToolbarAction<TToolbar> HtmlAttributes(IDictionary<string, object> htmlAttributes)
        {
            this.htmlAttributes = htmlAttributes;
            return this;
        }

        /// <summary>
        /// Set html attributes
        /// </summary>
        public BsToolbarAction<TToolbar> HtmlAttributes(object htmlAttributes)
        {
            return HtmlAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        /// Sets html attributes
        /// </summary>
        /// <returns>BsToolbarAction</returns>
        public BsToolbarAction<TToolbar> HtmlAttributes(Func<TToolbar, MvcHtmlString> tabDelegate)
        {
            this.tabDelegate = tabDelegate;
            return this;
        }

        /// <summary>
        /// Sets href attribute
        /// </summary>
        /// <returns>BsToolbarAction</returns>
        public BsToolbarAction<TToolbar> Action(string action)
        {
            this.href = action;
            return this;
        }

        /// <summary>
        /// Renders component
        /// </summary>
        public override string Render()
        {
            var actionBuilder = new TagBuilder("a");
            actionBuilder.AddCssClass(descriptorClass);
            actionBuilder.AddCssClass(this.styleClasses);
            actionBuilder.MergeAttribute("href", this.href ?? "#");

            if (!string.IsNullOrEmpty(this.tabId))
            {
                actionBuilder.MergeAttribute("data-tabid", this.tabId);
            }

            if (!string.IsNullOrEmpty(this.title))
            {
                actionBuilder.MergeAttribute("title", this.title);
            }

            actionBuilder.InnerHtml += (this.glyphIcon.HasValue ? GetGlyphcon(this.glyphIcon.Value) + " " : "") + this.text;

            return actionBuilder.ToString();
        }

        internal void SetTabId(string tabId)
        {
            this.tabId = tabId;
        }
    }
}