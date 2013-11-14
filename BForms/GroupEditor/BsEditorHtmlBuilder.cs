using BForms.Models;
using BForms.Mvc;
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
    public class BsEditorHtmlBuilder<TModel> : BaseComponent
    {
        #region Properties and Constructor
        private BsEditorTabConfigurator tabConfigurator;
        private IDictionary<string, object> htmlAttributes;

        public BsEditorHtmlBuilder(TModel model)
        {

        }

        public BsEditorHtmlBuilder(TModel model, ViewContext viewContext)
            : base(viewContext)
        {
            this.viewContext = viewContext;
            this.tabConfigurator = new BsEditorTabConfigurator();
            var type = typeof(TModel);

            foreach (var prop in type.GetProperties())
            {
                BsGroupEditorAttribute attr = null;

                if (ReflectionHelpers.TryGetAttribute(prop, out attr))
                {
                    tabConfigurator.AddTab(attr.Name, ((int)attr.Id).ToString());

                    var value = prop.GetValue(model);

                    if (value != null)
                    {
                        InvokeAddTabConfig(value, prop, attr);
                    }
                }
            }
        }
        #endregion

        #region Render
        public override string Render()
        {
            var result = this.viewContext.RequestContext.HttpContext.Request.IsAjaxRequest() ?
                this.RenderAjax() :
                this.RenderIndex();
            return result;
        }

        public string RenderAjax()
        {
            return tabConfigurator.Render();
        }

        public string RenderIndex()
        {
            var container = new TagBuilder("div");

            container.AddCssClass("group_editor");

            #region Left
            var left = new TagBuilder("div");

            left.AddCssClass("left");

            left.InnerHtml += tabConfigurator.Render();

            container.InnerHtml += left;
            #endregion

            #region Right
            var right = new TagBuilder("div");

            right.AddCssClass("right");

            container.InnerHtml += right;
            #endregion


            return container.ToString();
        }
        #endregion

        #region Config
        public BsEditorHtmlBuilder<TModel> HtmlAttributes(Dictionary<string, object> htmlAttributes)
        {
            this.htmlAttributes = htmlAttributes;
            return this;
        }

        public BsEditorHtmlBuilder<TModel> ConfigureTabs(Action<BsEditorTabConfigurator> config)
        {
            var factory = new BsEditorTabConfigurator();

            config(factory);

            return this;
        }
        #endregion

        #region Helpers
        private void InvokeAddTabConfig(object value, PropertyInfo prop, BsGroupEditorAttribute attr)
        {
            Type rowType = prop.PropertyType.GetGenericArguments()[0];
            MethodInfo method = typeof(BsEditorTabConfigurator).GetMethod("Add");
            MethodInfo generic = method.MakeGenericMethod(rowType);
            generic.Invoke(tabConfigurator, new object[] { attr, value });
        }
        #endregion
    }
}
