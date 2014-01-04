using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using BForms.Models;
using BForms.Mvc;
using BForms.Renderers;

namespace BForms.Grid
{
    /// <summary>
    /// Grid toolbar default control component
    /// </summary>
    public class BsToolbarAction<TToolbar> : BsBaseComponent<BsToolbarAction<TToolbar>>
    {
        #region Properties and constructors
        internal string descriptorClass;

        internal string styleClasses;

        internal string title;

        internal string text;

        internal Glyphicon? glyphIcon;

        internal string href;

        internal string tabId;

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

        public BsToolbarAction()
        {
            this.renderer = new BsToolbarActionRenderer<TToolbar>(this);
        }

        public BsToolbarAction(ViewContext viewContext)
            : base(viewContext) 
        {
            this.renderer = new BsToolbarActionRenderer<TToolbar>(this);
        }

        public BsToolbarAction(string descriptorClass, ViewContext viewContext)
            : base(viewContext)
        {
            this.renderer = new BsToolbarActionRenderer<TToolbar>(this);
            this.descriptorClass = descriptorClass;
        }

        public BsToolbarAction(BsToolbarActionType type, ViewContext viewContext)
            : base(viewContext)
        {
            this.renderer = new BsToolbarActionRenderer<TToolbar>(this);

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
                case BsToolbarActionType.Order:
                    {
                        this.descriptorClass = "btn btn_order hidden-xs bs-show_order";
                        this.title = "Order";
                        this.glyphIcon = Glyphicon.Sort;
                        break;
                    }
            }
        }

        public string GetDescriptorClass()
        {
            return this.descriptorClass;
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

        internal void SetTabId(string tabId)
        {
            this.tabId = tabId;
        }
    }

    /// <summary>
    /// For toolbar without forms
    /// </summary>
    public class BsToolbarAction: BsBaseComponent<BsToolbarAction>
    {
        #region Properties and constructors
        internal string descriptorClass;

        internal string styleClasses;

        internal string title;

        internal string text;

        internal Glyphicon? glyphIcon;

        internal string href;

        internal string tabId;

        internal IDictionary<string, object> HtmlAttr
        {
            get
            {
                return this.htmlAttributes;
            }
        }

        public BsToolbarAction()
        {
            this.renderer = new BsToolbarActionRenderer(this);
        }

        public BsToolbarAction(ViewContext viewContext)
            : base(viewContext)
        {
            this.renderer = new BsToolbarActionRenderer(this);
        }

        public BsToolbarAction(string descriptorClass, ViewContext viewContext)
            : base(viewContext)
        {
            this.renderer = new BsToolbarActionRenderer(this);
            this.descriptorClass = descriptorClass;
        }

        public string GetDescriptorClass()
        {
            return this.descriptorClass;
        }

        #endregion

        /// <summary>
        /// Sets control descriptor class
        /// </summary>
        /// <returns>BsToolbarAction</returns>
        public BsToolbarAction DescriptorClass(string descriptorClass)
        {
            this.descriptorClass = descriptorClass;
            return this;
        }

        /// <summary>
        /// Sets control style
        /// </summary>
        /// <returns>BsToolbarAction</returns>
        public BsToolbarAction StyleClasses(string styleClasses)
        {
            this.styleClasses = styleClasses;
            return this;
        }

        /// <summary>
        /// Sets control button title
        /// </summary>
        /// <returns>BsToolbarAction</returns>
        public BsToolbarAction Title(string title)
        {
            this.title = title;
            return this;
        }

        /// <summary>
        /// Sets control button text
        /// </summary>
        /// <returns>BsToolbarAction</returns>
        public BsToolbarAction Text(string text)
        {
            this.text = text;
            return this;
        }

        /// <summary>
        /// Sets control GlyphIcon
        /// </summary>
        /// <returns>BsToolbarAction</returns>
        public BsToolbarAction GlyphIcon(Glyphicon icon)
        {
            this.glyphIcon = icon;
            return this;
        }

        /// <summary>
        /// Set html attributes
        /// </summary>
        public BsToolbarAction HtmlAttributes(IDictionary<string, object> htmlAttributes)
        {
            this.htmlAttributes = htmlAttributes;
            return this;
        }

        /// <summary>
        /// Set html attributes
        /// </summary>
        public BsToolbarAction HtmlAttributes(object htmlAttributes)
        {
            return HtmlAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }


        /// <summary>
        /// Sets href attribute
        /// </summary>
        /// <returns>BsToolbarAction</returns>
        public BsToolbarAction Action(string action)
        {
            this.href = action;
            return this;
        }

        internal void SetTabId(string tabId)
        {
            this.tabId = tabId;
        }
    }

}