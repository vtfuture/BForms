using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BForms.Html;
using BForms.Mvc;
using BForms.Utilities;
using BForms.Renderers;

namespace BForms.Panels
{
    public class BsPanelsHtmlBuilder<TModel> : BsBaseComponent
    {
        internal BsPanelsConfigurator<TModel> panelsConfig;

        public BsPanelsHtmlBuilder(TModel model, ViewContext viewContext)
            : base(viewContext)
        {
            this.viewContext = viewContext;
            this.renderer = new BsPanelsBaseRenderer<TModel>(this);

            this.panelsConfig = new BsPanelsConfigurator<TModel>(viewContext);
            var type = typeof(TModel);

            foreach (var prop in type.GetProperties())
            {
                BsPanelAttribute attr = null;

                if (ReflectionHelpers.TryGetAttribute(prop, out attr))
                {
                    DisplayAttribute displayAttr = null;
                    ReflectionHelpers.TryGetAttribute(prop, out displayAttr);

                    panelsConfig.AddPanel(attr, displayAttr);
                }
            }
        }

        public BsPanelsHtmlBuilder<TModel> ConfigurePanels(Action<BsPanelsConfigurator<TModel>> config)
        {
            config(this.panelsConfig);

            return this;
        }

        public BsPanelsHtmlBuilder<TModel> HtmlAttributes(Dictionary<string, object> htmlAttributes)
        {
            base.HtmlAttributes(htmlAttributes);

            return this;
        }

        public BsPanelsHtmlBuilder<TModel> HtmlAttributes(object htmlAttributes)
        {
            base.HtmlAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));

            return this;
        }
    }
}
