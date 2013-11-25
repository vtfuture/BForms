using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.WebPages.Scope;
using BForms.Models;
using BForms.Mvc;
using BForms.Renderers;
using BForms.Utilities;

namespace BForms.Panels
{
    public class BsPanelHtmlBuilder : BsBaseComponent<BsPanelHtmlBuilder>
    {

        #region Properties and Constructor
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
        internal object objId;
        internal bool noHeader;
        internal Glyphicon? glyphicon;

        /// <summary>
        /// Sets the ViewContext property for the BaseComponent
        /// </summary>
        public BsPanelHtmlBuilder(ViewContext context)
            : base(context)
        {
            this.renderer = new BsPanelBaseRenderer(this);
        }
        #endregion

        #region Internal methods
        public BsPanelHtmlBuilder SetPropertiesFromModel(PropertyInfo propertyInfo)
        {
            BsPanelAttribute panelAttr = null;

            if (ReflectionHelpers.TryGetAttribute(propertyInfo, out panelAttr))
            {
                DisplayAttribute displayAttr = null;

                ReflectionHelpers.TryGetAttribute(propertyInfo, out displayAttr);

                if (panelAttr != null)
                {
                    this.Id(panelAttr.Id);
                    this.Expandable(panelAttr.Expandable);
                    this.Editable(panelAttr.Editable);
                }

                if (displayAttr != null)
                {
                    this.Name(displayAttr.Name);
                }
            }

            return this;
        }
        #endregion

        #region Public methods

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

        public BsPanelHtmlBuilder ObjId(object objId)
        {
            this.objId = objId;
            return this;
        }

        public BsPanelHtmlBuilder NoHeader()
        {
            this.noHeader = true;
            return this;
        }

        public BsPanelHtmlBuilder Glyphicon(Glyphicon glyphicon)
        {
            this.glyphicon = glyphicon;
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

        public BsPanelHtmlBuilder Renderer(BsPanelBaseRenderer renderer)
        {
            renderer.Register(this);
            this.renderer = renderer;

            return this;
        }
        #endregion
    }
}

