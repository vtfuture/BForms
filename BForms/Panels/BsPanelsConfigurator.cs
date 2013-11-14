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
using BForms.Utilities;
using DocumentFormat.OpenXml.EMMA;

namespace BForms.Panels
{
    public class BsPanelsConfigurator<TModel> : BaseComponent
    {
        #region Private properties
        internal List<BsPanelHtmlBuilder> Panels { get; set; }

        private string _readonlyUrl;
        private string _editableUrl;
        private string _saveUrl;

        private bool _isEditable;
        #endregion

        #region Public properties
        public string GetReadonlyUrl
        {
            get
            {
                return _readonlyUrl;
            }
            set
            {
                _readonlyUrl = value;
            }
        }

        public string GetEditableUrl
        {
            get
            {
                return _editableUrl;
            }
            set
            {
                _editableUrl = value;
                _isEditable = true;
            }
        }

        public string UpdateUrl
        {
            get
            {
                return _saveUrl;
            }
            set
            {
                _saveUrl = value;
            }
        }
        #endregion

        public BsPanelsConfigurator(ViewContext viewContext)
        {
            this.Panels = new List<BsPanelHtmlBuilder>();
            this.viewContext = viewContext;
        }

        #region Public methods
        public BsPanelHtmlBuilder AddPanel(BsPanelAttribute panelAttr, DisplayAttribute displayAttr)
        {
            var newPanel = new BsPanelHtmlBuilder(viewContext);

            if (panelAttr != null)
            {
                newPanel.Id((int)panelAttr.Id);
                newPanel.Expandable(panelAttr.Expandable);
                newPanel.Editable(panelAttr.Editable);
            }

            if (displayAttr != null)
            {
                newPanel.Name(displayAttr.Name);
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
            return Panels.FirstOrDefault(p => (int)p._id == (int)id);
        }
        #endregion

        public override string Render()
        {
            var panelsHtml = string.Empty;

            foreach (var panel in Panels)
            {

                if (string.IsNullOrEmpty(panel.readonlyUrl) && !string.IsNullOrEmpty(_readonlyUrl))
                {
                    panel.ReadonlyUrl(_readonlyUrl);
                }

                if (string.IsNullOrEmpty(panel.editableUrl) && !string.IsNullOrEmpty(_editableUrl) && panel.isEditable)
                {
                    panel.EditableUrl(_editableUrl);
                }

                if (string.IsNullOrEmpty(panel.saveUrl) && !string.IsNullOrEmpty(_saveUrl))
                {
                    panel.SaveUrl(_saveUrl);
                }

                panelsHtml += panel.Render();
            }

            return panelsHtml;
        }
    }
}
