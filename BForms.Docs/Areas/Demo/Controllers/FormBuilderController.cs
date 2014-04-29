using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BForms.Docs.Controllers;

namespace BForms.Docs.Areas.Demo.Controllers
{
    public class FormBuilderController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }
	}
}