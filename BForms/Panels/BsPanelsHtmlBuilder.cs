using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BForms.Mvc;
using BForms.Utilities;

namespace BForms.Panels
{
    public class BsPanelsHtmlBuilder<TModel> : BaseComponent
    {
        private IDictionary<string, object> _htmlAttributes;
        private BsPanelsConfigurator<TModel> _panelsConfigurator; 

        public BsPanelsHtmlBuilder(TModel model, ViewContext viewContext)
            : base(viewContext)
        {
            this.viewContext = viewContext;
            this._panelsConfigurator = new BsPanelsConfigurator<TModel>(viewContext);
            var type = typeof(TModel);

            foreach (var prop in type.GetProperties())
            {
                BsPanelAttribute attr = null;

                if (ReflectionHelpers.TryGetAttribute(prop, out attr))
                {
                    DisplayAttribute displayAttr = null;
                    ReflectionHelpers.TryGetAttribute(prop, out displayAttr);

                    _panelsConfigurator.AddPanel(attr, displayAttr);
                   
                }
            }
        }

        public BsPanelsHtmlBuilder<TModel> ConfigurePanels(Action<BsPanelsConfigurator<TModel>> config)
        {
            config(this._panelsConfigurator);
            return this;
        }

        public TagBuilder GetContainer()
        {
            var container = new TagBuilder("div");
            container.AddCssClass("panel-group");

            return container;
        }

        public override string Render()
        {
            var container = this.GetContainer();

            container.InnerHtml += this._panelsConfigurator.Render();

            return container.ToString();
        }
    }
}
