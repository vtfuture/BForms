using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;
using BForms.Mvc;
using BForms.Renderers;

namespace BForms.Html
{
    public class BsPanelHtmlBuilder : BaseComponent
    {
        protected BsPanelBaseRenderer renderer;
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
        internal IDictionary<string, object> htmlAttributes;

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
            this.htmlAttributes = htmlAttributes;
            return this;
        }

        /// <summary>
        /// Appends html attributes to panel div element
        /// </summary>
        public BsPanelHtmlBuilder HtmlAttributes(object htmlAttributes)
        {
            this.htmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            return this;
        }

        public BsPanelHtmlBuilder Theme(BsPanelBaseRenderer renderer)
        {
            this.renderer = renderer;

            return this;
        }

        //public TagBuilder GetContainer()
        //{
        //    var container = new TagBuilder("div");

        //    container.MergeAttributes(this._htmlAttributes, true);

        //    if (!String.IsNullOrEmpty(this.editableUrl))
        //    {
        //        container.MergeAttribute("data-editableurl", this.editableUrl);
        //    }

        //    if (!String.IsNullOrEmpty(this.readonlyUrl))
        //    {
        //        container.MergeAttribute("data-readonlyurl", this.readonlyUrl);
        //    }

        //    if (!String.IsNullOrEmpty(this.saveUrl))
        //    {
        //        container.MergeAttribute("data-saveurl", this.saveUrl);
        //    }

        //    container.MergeAttribute("data-component", this.id.ToString());

        //    container.AddCssClass("panel panel-default");

        //    return container;
        //}

        public virtual string RenderHeader()
        {
            return this.renderer.RenderHeader();
        }

        public string RenderContent()
        {
            return this.renderer.RenderContent();
        }

        /// <summary>
        /// Renders the component
        /// </summary>
        public override string Render()
        {
            TagBuilder result;

            var container = renderer.GetContainer(out result);

            container.InnerHtml += this.RenderHeader();

            container.InnerHtml += this.RenderContent();

            return result.ToString();
        }
    }
}
