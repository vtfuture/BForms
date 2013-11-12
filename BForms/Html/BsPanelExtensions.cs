using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BForms.Html
{
    public static class BsPanelExtensions
    {
        public static BsPanelHtmlBuilder BsPanel(this HtmlHelper html)
        {
            return new BsPanelHtmlBuilder(html.ViewContext);
        }
    }
}
