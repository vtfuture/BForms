using BForms.Models;
using BForms.Mvc;
using BForms.Renderers;
using BForms.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BForms.GroupEditor
{
    public class BsEditorHtmlBuilder<TModel> : BsBaseComponent
    {
        #region Properties and Constructor
        internal BsEditorTabConfigurator<TModel> tabConfigurator;

        public BsEditorHtmlBuilder(TModel model)
        {
            this.renderer = new BsEditorBaseRenderer<TModel>(this);
        }

        public BsEditorHtmlBuilder(TModel model, ViewContext viewContext)
            : base(viewContext)
        {
            this.viewContext = viewContext;

            this.renderer = new BsEditorBaseRenderer<TModel>(this);

            this.tabConfigurator = new BsEditorTabConfigurator<TModel>(viewContext);

            var type = typeof(TModel);

            foreach (var prop in type.GetProperties())
            {
                BsGroupEditorAttribute attr = null;

                if (ReflectionHelpers.TryGetAttribute(prop, out attr))
                {
                    tabConfigurator.AddNavTab(attr);

                    var value = prop.GetValue(model);

                    InvokeAddTabConfig(value, prop, attr);
                }
            }
        }
        #endregion

        #region Public Methods
        public BsEditorHtmlBuilder<TModel> HtmlAttributes(Dictionary<string, object> htmlAttributes)
        {
            base.HtmlAttributes(htmlAttributes);

            return this;
        }

        public BsEditorHtmlBuilder<TModel> ConfigureTabs(Action<BsEditorTabConfigurator<TModel>> config)
        {
            config(this.tabConfigurator);

            return this;
        }
        #endregion

        #region Helpers
        private void InvokeAddTabConfig(object value, PropertyInfo prop, BsGroupEditorAttribute attr)
        {
            var propertyType = prop.PropertyType;
            var genericArgs = propertyType.GetGenericArguments();

            var count = genericArgs.Count();
            var bindings = BindingFlags.Default | BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.NonPublic;

            if (count == 0)
            {
                var baseType = propertyType.BaseType;

                genericArgs = baseType.GetGenericArguments();

                count = genericArgs.Count();
            }

            if (count > 0)
            {
                MethodInfo method = null, generic = null;

                Type rowType = genericArgs[0];

                if (count == 1)
                {
                    method = typeof(BsEditorTabConfigurator<TModel>).GetMethod("Add", bindings);
                    generic = method.MakeGenericMethod(rowType);
                }

                if (count == 2)
                {
                    Type searchType = genericArgs[1];

                    method = typeof(BsEditorTabConfigurator<TModel>).GetMethod("AddWithSearch", bindings);
                    generic = method.MakeGenericMethod(rowType, searchType);
                }

                if (count == 3)
                {
                    Type searchType = genericArgs[1];
                    Type newType = genericArgs[2];

                    method = typeof(BsEditorTabConfigurator<TModel>).GetMethod("AddWithNew", bindings);
                    generic = method.MakeGenericMethod(rowType, searchType, newType);
                }

                generic.Invoke(tabConfigurator, new object[] { attr, value });
            }
        }
        #endregion
    }
}
