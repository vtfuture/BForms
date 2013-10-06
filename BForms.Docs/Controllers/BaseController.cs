using BForms.Docs.Areas.Demo.Mock;
using RequireJS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BForms.Docs.Controllers
{
    public class BaseController : RequireJS.RequireJsController
    {
        public BFormsContext db = new BFormsContext();

        public override void RegisterGlobalOptions()
        {
            RequireJsOptions.Add(
                "homeUrl",
                Url.Action("Index", "Home", new { area = "" }),
                RequireJsOptionsScope.Global);
        }
    }
}