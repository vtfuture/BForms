using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BForms.Html;

namespace BForms.Panels.ControlPanel
{
    public static class BsControlPanelExtensions
    {
        public static BsControlPanelBuilder BsControlPanel(this HtmlHelper helper)
        {
            return new BsControlPanelBuilder(helper.ViewContext, helper);
        }
    }
}
