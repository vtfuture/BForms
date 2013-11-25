using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BForms.Mvc
{
    public class BsBaseConfigurator
    {
        internal ViewContext viewContext { get; set; }

        public BsBaseConfigurator()
        {
        }

        public BsBaseConfigurator(ViewContext viewContext)
        {
            this.viewContext = viewContext;
        }

        internal bool IsAjaxRequest()
        {
            if (this.viewContext == null)
                throw new Exception("View context is null");

            return this.viewContext.RequestContext.HttpContext.Request.IsAjaxRequest();
        }
    }
}
