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
        protected ViewContext viewContext { get; set; }

        public BsBaseConfigurator(ViewContext viewContext)
        {
            this.viewContext = viewContext;
        }
    }
}
