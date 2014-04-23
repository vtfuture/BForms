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
    public class BsToolbarItemGroupActionLink<TToolbar> : BsBaseComponent<BsToolbarItemGroupActionLink<TToolbar>>
    {
        #region Properties and constructors
        internal string descriptorClass;

        internal string styleClasses;

        internal string title;

        internal string text;

        internal Glyphicon? glyphIcon;

        internal string href;

        internal IDictionary<string, object> HtmlAttr
        {
            get
            {
                return this.htmlAttributes;
            }
        }

        public BsToolbarItemGroupActionLink()
        {
            this.renderer = new BsToolbarItemGroupActionRenderer<TToolbar>(this);
        }

        public BsToolbarItemGroupActionLink(ViewContext viewContext)
            : base(viewContext)
        {
            this.renderer = new BsToolbarItemGroupActionRenderer<TToolbar>(this);
            // Set default control action properties
            this.href = "#";
            this.text = "Link";
        }

        public BsToolbarItemGroupActionLink(string descriptorClass, ViewContext viewContext)
            : base(viewContext)
        {
            this.renderer = new BsToolbarItemGroupActionRenderer<TToolbar>(this);
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
        public BsToolbarItemGroupActionLink<TToolbar> DescriptorClass(string descriptorClass)
        {
            this.descriptorClass = descriptorClass;
            return this;
        }

        /// <summary>
        /// Sets control style
        /// </summary>
        /// <returns>BsToolbarAction</returns>
        public BsToolbarItemGroupActionLink<TToolbar> StyleClasses(string styleClasses)
        {
            this.styleClasses = styleClasses;
            return this;
        }

        /// <summary>
        /// Sets control button text
        /// </summary>
        /// <returns>BsToolbarAction</returns>
        public BsToolbarItemGroupActionLink<TToolbar> Text(string text)
        {
            this.text = text;
            return this;
        }

        /// <summary>
        /// Sets control GlyphIcon
        /// </summary>
        /// <returns>BsToolbarAction</returns>
        public BsToolbarItemGroupActionLink<TToolbar> GlyphIcon(Glyphicon icon)
        {
            this.glyphIcon = icon;
            return this;
        }

        /// <summary>
        /// Set html attributes
        /// </summary>
        public BsToolbarItemGroupActionLink<TToolbar> HtmlAttributes(IDictionary<string, object> htmlAttributes)
        {
            this._htmlAttributes = htmlAttributes;
            return this;
        }

        /// <summary>
        /// Set html attributes
        /// </summary>
        public BsToolbarItemGroupActionLink<TToolbar> HtmlAttributes(object htmlAttributes)
        {
            return HtmlAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        /// Sets href attribute
        /// </summary>
        /// <returns>BsToolbarAction</returns>
        public BsToolbarItemGroupActionLink<TToolbar> Action(string action)
        {
            this.href = action;
            return this;
        }
    }

    /// <summary>
    /// For toolbar without forms
    /// </summary>
    public class BsToolbarItemGroupActionLink : BsBaseComponent<BsToolbarItemGroupActionLink>
    {
        #region Properties and constructors
        internal string descriptorClass;

        internal string styleClasses;

        internal string title;

        internal string text;

        internal Glyphicon? glyphIcon;

        internal string href;

        internal IDictionary<string, object> HtmlAttr
        {
            get
            {
                return this.htmlAttributes;
            }
        }

        public BsToolbarItemGroupActionLink()
        {
            this.renderer = new BsToolbarItemGroupActionRenderer(this);
        }

        public BsToolbarItemGroupActionLink(ViewContext viewContext)
            : base(viewContext)
        {
            this.renderer = new BsToolbarItemGroupActionRenderer(this);
            // Set default control action properties
            this.href = "#";
            this.text = "Link";
        }

        public BsToolbarItemGroupActionLink(string descriptorClass, ViewContext viewContext)
            : base(viewContext)
        {
            this.renderer = new BsToolbarItemGroupActionRenderer(this);
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
        public BsToolbarItemGroupActionLink DescriptorClass(string descriptorClass)
        {
            this.descriptorClass = descriptorClass;
            return this;
        }

        /// <summary>
        /// Sets control style
        /// </summary>
        /// <returns>BsToolbarAction</returns>
        public BsToolbarItemGroupActionLink StyleClasses(string styleClasses)
        {
            this.styleClasses = styleClasses;
            return this;
        }

        /// <summary>
        /// Sets control button text
        /// </summary>
        /// <returns>BsToolbarAction</returns>
        public BsToolbarItemGroupActionLink Text(string text)
        {
            this.text = text;
            return this;
        }

        /// <summary>
        /// Sets control GlyphIcon
        /// </summary>
        /// <returns>BsToolbarAction</returns>
        public BsToolbarItemGroupActionLink GlyphIcon(Glyphicon icon)
        {
            this.glyphIcon = icon;
            return this;
        }

        /// <summary>
        /// Set html attributes
        /// </summary>
        public BsToolbarItemGroupActionLink HtmlAttributes(IDictionary<string, object> htmlAttributes)
        {
            this._htmlAttributes = htmlAttributes;
            return this;
        }

        /// <summary>
        /// Set html attributes
        /// </summary>
        public BsToolbarItemGroupActionLink HtmlAttributes(object htmlAttributes)
        {
            return HtmlAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        /// Sets href attribute
        /// </summary>
        /// <returns>BsToolbarAction</returns>
        public BsToolbarItemGroupActionLink Action(string action)
        {
            this.href = action;
            return this;
        }
    }
}