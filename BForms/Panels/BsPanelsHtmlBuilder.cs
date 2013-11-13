using System;
using System.Collections.Generic;
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

        public BsPanelsHtmlBuilder(TModel model, ViewContext viewContext)
            : base(viewContext)
        {
            this.viewContext = viewContext;
            //this.tabConfigurator = new BsEditorTabConfigurator();
            var type = typeof(TModel);

            //foreach (var prop in type.GetProperties())
            //{
            //    BsPanelAttribute attr = null;

            //    if (ReflectionHelpers.TryGetAttribute(prop, out attr))
            //    {
            //        tabConfigurator.AddTab(attr.Name, ((int)attr.Id).ToString());

            //        var value = prop.GetValue(model);

            //        if (value != null)
            //        {
            //            InvokeAddTabConfig(value, prop, attr);
            //        }
            //    }
            //}
        }

        public override string Render()
        {
            throw new NotImplementedException();
        }
    }
}
