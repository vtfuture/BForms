using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BForms.Panels;
using BForms.Utilities;
using DocumentFormat.OpenXml.Office2010.PowerPoint;

namespace BForms.Html
{
    public static class BsPanelExtensions
    {
        public static BsPanelHtmlBuilder BsPanel(this HtmlHelper html)
        {
            return new BsPanelHtmlBuilder(html.ViewContext);
        }

        public static BsPanelHtmlBuilder BsPanelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel,TValue>> expression)
        {
            return new BsPanelHtmlBuilder(html.ViewContext).SetPropertiesFromModel(expression.GetPropertyInfo());
        }

        public static BsPanelsHtmlBuilder<TModel> BsPanelsFor<TModel>(this HtmlHelper<TModel> htmlHelper,
             TModel model)
        {
            var baseBuilder = new BsPanelsHtmlBuilder<TModel>(model, htmlHelper.ViewContext);

            return baseBuilder;
        }
    }
}
