using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using BForms.Mvc;


namespace BForms.Docs
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //register BForms validation provider
            ModelValidatorProviders.Providers.Add(new BsModelValidatorProvider());

            BForms.Grid.BsGridResourceManager.Register(typeof(Resources.Resource));
        }
    }
}
