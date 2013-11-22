using System.Collections.Generic;
using System.Web.Mvc;
using BForms.Models;
using BForms.Renderers;

namespace BForms.Grid
{
    public class BsToolbarButtonGroup<TToolbar> : BsToolbarActionsBaseFactory<TToolbar>
    {
        internal string name;

        internal string title;

        internal Glyphicon? glyphIcon;

        internal string descriptorClass;

        internal string styleClasses;


        internal IDictionary<string, object> HtmlAttr
        {
            get
            {
                return this.htmlAttributes;
            }
        }

        public BsToolbarButtonGroup(ViewContext viewContext) : base(viewContext)
        {
            this.renderer = new BsToolbarButtonGroupRenderer<TToolbar>(this);
        } 

        /// <summary>
        /// Sets control descriptor class
        /// </summary>
        /// <returns>BsToolbarAction</returns>
        public BsToolbarButtonGroup<TToolbar> DescriptorClass(string descriptorClass)
        {
            this.descriptorClass = descriptorClass;
            return this;
        }

        /// <summary>
        /// Sets control style
        /// </summary>
        /// <returns>BsToolbarAction</returns>
        public BsToolbarButtonGroup<TToolbar> StyleClasses(string styleClasses)
        {
            this.styleClasses = styleClasses;
            return this;
        }

        /// <summary>
        /// Set display name
        /// </summary>
        public BsToolbarButtonGroup<TToolbar> DisplayName(string name)
        {
            this.name = name;

            return this;
        }

        /// <summary>
        /// Sets control GlyphIcon
        /// </summary>
        /// <returns>BsToolbarAction</returns>
        public BsToolbarButtonGroup<TToolbar> GlyphIcon(Glyphicon icon)
        {
            this.glyphIcon = icon;

            return this;
        }

        /// <summary>
        /// Set html attributes
        /// </summary>
        public BsToolbarButtonGroup<TToolbar> HtmlAttributes(IDictionary<string, object> htmlAttributes)
        {
            this.htmlAttributes = htmlAttributes;
            return this;
        }

        /// <summary>
        /// Set html attributes
        /// </summary>
        public BsToolbarButtonGroup<TToolbar> HtmlAttributes(object htmlAttributes)
        {
            return HtmlAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        /// Add link to button group
        /// </summary>
        public BsToolbarItemGroupActionLink<TToolbar> AddActionLink()
        {
            var toolbarAction = new BsToolbarItemGroupActionLink<TToolbar>(this.viewContext);
            actions.Add(toolbarAction);

            return toolbarAction;
        }
    }
}
