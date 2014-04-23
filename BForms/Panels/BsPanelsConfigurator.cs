using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using BForms.Html;
using BForms.Mvc;
using BForms.Renderers;
using BForms.Utilities;
using DocumentFormat.OpenXml.EMMA;

namespace BForms.Panels
{
    public class BsPanelsConfigurator<TModel> : BsBaseConfigurator
    {
        #region Private properties
        internal List<BsPanelHtmlBuilder> Panels { get; set; }
        internal string readonlyUrl;
        internal string editableUrl;
        internal string saveUrl;

        private bool _isEditable;
        #endregion

        #region Public properties
        public string GetReadonlyUrl
        {
            get
            {
                return readonlyUrl;
            }
            set
            {
                readonlyUrl = value;
            }
        }

        public string GetEditableUrl
        {
            get
            {
                return editableUrl;
            }
            set
            {
                editableUrl = value;
                _isEditable = true;
            }
        }

        public string UpdateUrl
        {
            get
            {
                return saveUrl;
            }
            set
            {
                saveUrl = value;
            }
        }
        #endregion

        public BsPanelsConfigurator(ViewContext viewContext) : base(viewContext)
        {
            this.viewContext = viewContext;
            this.Panels = new List<BsPanelHtmlBuilder>();
        }

        #region Public methods
        public BsPanelHtmlBuilder AddPanel(BsPanelAttribute panelAttr, DisplayAttribute displayAttr)
        {
            var newPanel = new BsPanelHtmlBuilder(this.viewContext);

            if (panelAttr != null)
            {
                newPanel.Id(panelAttr.Id);
                newPanel.Expandable(panelAttr.Expandable);
                newPanel.Editable(panelAttr.Editable);
            }

            if (displayAttr != null)
            {
                newPanel.Name(displayAttr.GetName());
            }

            this.Panels.Add(newPanel);

            return newPanel;
        }

        public BsPanelHtmlBuilder For<TValue>(Expression<Func<TModel, TValue>> expression)
        {
            var prop = expression.GetPropertyInfo();
            BsPanelAttribute attr;

            if (ReflectionHelpers.TryGetAttribute(prop, out attr))
            {
                var panel = this.GetPanel(attr.Id);

                if (panel == null)
                    throw new Exception(String.Format("Property {0} was not found", prop.Name));


                return panel;
            }

            throw new Exception(String.Format("Property {0} is not decorated with BsPanelAttribute", prop.Name));
        }
        #endregion

        #region Internal methods
        public BsPanelHtmlBuilder GetPanel(object id)
        {
            return Panels.FirstOrDefault(p => p._id.Equals(id));
        }

        public BsPanelsConfigurator<TModel> Renderer<TPanel>() where TPanel : BsPanelBaseRenderer, new()
        {
            this.Panels.ForEach(x =>
            {
                var panelRenderer = new TPanel();

                x.Renderer(panelRenderer);
            });

            return this;
        }
        #endregion
    }
}
