using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BForms.Panels;
using DocumentFormat.OpenXml.Office2010.PowerPoint;

namespace BForms.Html
{
    public static class BsPanelExtensions
    {
        public static BsPanelHtmlBuilder BsPanel(this HtmlHelper html)
        {
            return new BsPanelHtmlBuilder(html.ViewContext);
        }

        public static BsPanelsHtmlBuilder<TModel> BsPanelsFor<TModel>(this HtmlHelper<TModel> htmlHelper,
             TModel model)
        {
            var baseBuilder = new BsPanelsHtmlBuilder<TModel>(model, htmlHelper.ViewContext);

            return baseBuilder;
        }
    }
}
