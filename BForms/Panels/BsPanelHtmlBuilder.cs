using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;
using BForms.Mvc;
using BForms.Renderers;

namespace BForms.Panels
{
    public class BsPanelHtmlBuilder : BsBaseComponent
    {
        internal string name;
        internal bool isEditable;
        internal bool isExpanded;
        internal bool isExpandable = true;
        internal bool isLoaded;
        internal string readonlyUrl;
        internal string editableUrl;
        internal string saveUrl;
        internal string content;
        internal object id;

        /// <summary>
        /// Sets the ViewContext property for the BaseComponent
        /// </summary>
        public BsPanelHtmlBuilder(ViewContext context)
            : base(context)
        {
            this.renderer = new BsPanelBaseRenderer(this);
        }

        /// <summary>
        /// Sets the display name
        /// </summary>
        public BsPanelHtmlBuilder Name(string name)
        {
            this.name = name;
            return this;
        }

        public BsPanelHtmlBuilder Id(object id)
        {
            this.id = id;
            return this;
        }

        /// <summary>
        /// Specify if the box form has an editable component
        /// </summary>
        public BsPanelHtmlBuilder Editable(bool isEditable)
        {
            this.isEditable = isEditable;
            return this;
        }

        /// <summary>
        /// Specify if the box form is already expanded
        /// </summary>
        public BsPanelHtmlBuilder Expanded(bool isExpanded)
        {
            this.isExpanded = isExpanded;
            return this;
        }

        /// <summary>
        /// Specify if the box form is expandable or static
        /// </summary>
        public BsPanelHtmlBuilder Expandable(bool isExpandable)
        {
            this.isExpandable = isExpandable;
            return this;
        }

        /// <summary>
        /// Specify url from where the readonly form will be loaded
        /// </summary>
        public BsPanelHtmlBuilder ReadonlyUrl(string url)
        {
            this.readonlyUrl = url;
            return this;
        }

        /// <summary>
        /// Specify url from where the editable form will be loaded
        /// It will assume that the box form is editable
        /// </summary>
        public BsPanelHtmlBuilder EditableUrl(string url)
        {
            this.editableUrl = url;
            this.isEditable = true;

            return this;
        }

        public BsPanelHtmlBuilder SaveUrl(string url)
        {
            this.saveUrl = url;

            return this;
        }

        /// <summary>
        /// Sets the content of the box form
        /// It will also set the box as expanded and loaded
        /// </summary>
        public BsPanelHtmlBuilder Content(string content)
        {
            this.content = content;
            this.isExpanded = true;
            this.isLoaded = true;

            return this;
        }

        /// <summary>
        /// Appends html attributes to panel div element
        /// </summary>
        public BsPanelHtmlBuilder HtmlAttributes(Dictionary<string, object> htmlAttributes)
        {
            base.HtmlAttributes(htmlAttributes);

            return this;
        }

        /// <summary>
        /// Appends html attributes to panel div element
        /// </summary>
        public BsPanelHtmlBuilder HtmlAttributes(object htmlAttributes)
        {
            base.HtmlAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));

            return this;
        }

        public BsPanelHtmlBuilder Theme(BsPanelBaseRenderer renderer)
        {
            this.renderer = renderer;

            return this;
        }
    }
}
