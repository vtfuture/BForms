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
        public bool noHeader
        {
            get
            {
                return this._noHeader;
            }
        }

        public string name
        {
            get
            {
                return this._name;
            }
        }

        public bool isEditable
        {
            get
            {
                return this._isEditable;
            }
        }

        public bool hasReadonly
        {
            get
            {
                return this._hasReadonly;
            }
        }

        public bool isExpanded
        {
            get
            {
                return this._isExpanded;
            }
        }

        public bool isExpandable
        {
            get
            {
                return this._isExpandable;
            }
        }

        public bool isLoaded
        {
            get
            {
                return this._isLoaded;
            }
        }

        public string readonlyUrl
        {
            get
            {
                return this._readonlyUrl;
            }
        }

        public string editableUrl
        {
            get
            {
                return this._editableUrl;
            }
        }

        public string saveUrl
        {
            get
            {
                return this._saveUrl;
            }
        }

        public string content
        {
            get
            {
                return this._content;
            }
        }

        public object id
        {
            get
            {
                return this._id;
            }
        }

        public object objId
        {
            get
            {
                return this._objId;
            }
        }

        public Glyphicon? glyphicon
        {
            get
            {
                return this._glyphicon;
            }
        }

        public bool initialReadonly
        {
            get
            {
                return this._initialReadonly;
            }
        }

        public BsPanelTheme theme
        {
            get
            {
                return this._theme;
            }
        }
        public BsPanelMode? mode
        {
            get
            {
                return this._mode;
            }
        }

        #region Properties and Constructor
        internal string _name;
        internal bool _isEditable;
        internal bool _hasReadonly;
        internal bool _isExpanded;
        internal bool _isExpandable = true;
        internal bool _isLoaded;
        internal string _readonlyUrl;
        internal string _editableUrl;
        internal string _saveUrl;
        internal string _content;
        internal object _id;
        internal object _objId;
        internal bool _noHeader;
        internal Glyphicon? _glyphicon;
        internal bool _initialReadonly = true;
        internal BsPanelTheme _theme = BsPanelTheme.Default;
        internal BsPanelMode? _mode;

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
                    this.Name(displayAttr.GetName());
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
            this._name = name;
            return this;
        }

        public BsPanelHtmlBuilder Id(object id)
        {
            this._id = id;
            return this;
        }

        public BsPanelHtmlBuilder ObjId(object objId)
        {
            this._objId = objId;
            return this;
        }

        public BsPanelHtmlBuilder NoHeader()
        {
            this._noHeader = true;
            return this;
        }

        public BsPanelHtmlBuilder Glyphicon(Glyphicon glyphicon)
        {
            this._glyphicon = glyphicon;
            return this;
        }

        public BsPanelHtmlBuilder Theme(BsPanelTheme theme)
        {
            this._theme = theme;
            return this;
        }

        /// <summary>
        /// Specify if the box form has an editable component
        /// </summary>
        public BsPanelHtmlBuilder Editable(bool isEditable)
        {
            this._isEditable = isEditable;
            return this;
        }

        public BsPanelHtmlBuilder InitialEditable()
        {
            this._initialReadonly = false;
            return this;
        }

        /// <summary>
        /// Specify if the box form is already expanded
        /// </summary>
        public BsPanelHtmlBuilder Expanded(bool isExpanded)
        {
            this._isExpanded = isExpanded;
            return this;
        }

        /// <summary>
        /// Specify if the box form is expandable or static
        /// </summary>
        public BsPanelHtmlBuilder Expandable(bool isExpandable)
        {
            this._isExpandable = isExpandable;
          
            return this;
        }

        /// <summary>
        /// Specify url from where the readonly form will be loaded
        /// </summary>
        public BsPanelHtmlBuilder ReadonlyUrl(string url)
        {
            this._readonlyUrl = url;
            this._hasReadonly = true;
            return this;
        }

        /// <summary>
        /// Specify url from where the editable form will be loaded
        /// It will assume that the box form is editable
        /// </summary>
        public BsPanelHtmlBuilder EditableUrl(string url)
        {
            this._editableUrl = url;
            this._isEditable = true;

            return this;
        }

        public BsPanelHtmlBuilder SaveUrl(string url)
        {
            this._saveUrl = url;

            return this;
        }

        /// <summary>
        /// sets panel type, readonly/editable/both, overrides any past and future settings
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public BsPanelHtmlBuilder Mode(BsPanelMode mode)
        {
            this._mode = mode;
            return this;
        }

        /// <summary>
        /// Sets the content of the box form
        /// It will also set the box as expanded and loaded
        /// </summary>
        public BsPanelHtmlBuilder Content(string content)
        {
            this._content = content;
            this._isExpanded = true;
            this._isLoaded = true;

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

    public enum BsPanelTheme
    {
        Default,
        Blue,
        LightGreen,
        LightBlue,
        LightYellow,
        Red
    }

    public enum BsPanelMode
    {
        Readonly,
        Editable,
        Both
    }
}

